using firstmusic.Tools;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tasks;
using Domain.Models.VM;
using Domain.Models.Enteties;
using System.Net;
using System.IO;
//using YoutubeExplode;
//using YoutubeExplode.Videos.Streams;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using System.Dynamic;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Domain.Constants;
using System.Web;
using infrastructure.Services;
using Microsoft.AspNetCore.Http;
//using YoutubeExplode.Common;
using Google.Apis.YouTube.v3.Data;
//using YoutubeExplode.Search;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using firstmusic.Tools;
using System.Reflection.Metadata;
using Domain.DTO;
//using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog.Parsing;
using Newtonsoft.Json.Linq;
using infrastructure.Helpers;
using Utilitaire = infrastructure.Helpers.Utilitaire;
using YoutubeExplode;
//using YoutubeExplode;

namespace firstmusic.Controllers
{
    public class MorceauController : Controller
    {
        private readonly CancellationService _cancellationService;
        public static string oldkey = "";
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RedisService _redisService;
        public MorceauController(RedisService redisService,HttpClient httpClient, CancellationService cancellationService, IHttpContextAccessor httpContextAccessor)
        {
            _redisService = redisService;
            _httpClient = httpClient;
            _cancellationService = cancellationService;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetlistAudio()
        {
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<MorceauVM> _morceauVMs = null;
            try
            {
                MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
                ICollection<Morceau> _morceaux = await _morceauTask.GetAllMorceaus();
                _morceauVMs = _morceaux.Select(m => new MorceauVM
                {
                    title = HttpUtility.HtmlDecode(m.title),
                    url = m.url,
                    info = m.info
                }).ToList();


            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("morceauVMs", _morceauVMs);
            return Utilitaire.constructResponse(_model);
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetHistory(int pageNumber, int pageSize)
        {
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<MorceauVM> _morceauVMs = null;
            var IdUser = Utilitaire.getIdUserConnected(Request);
            try
            {
                PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();
                long idPlylistHistory = await Utilitaire.getHistoriqueId(IdUser); 
                MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
                ICollection<Morceau> _Morcaulist = await _morceauTask.GetPlylistMorceauAsync("",(int)idPlylistHistory, pageNumber, pageSize);
                _morceauVMs = _Morcaulist.Where(x => x.url != null).Select(m => new MorceauVM
                {
                    title = HttpUtility.HtmlDecode(m.title),
                    url = m.url,
                    info = m.info
                }).ToList();


            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", IdUser.ToString());

            }
            _model.Add("morceauVMs", _morceauVMs);
            return Utilitaire.constructResponse(_model);
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchMorceaux(string input, int idPlylist,int pageNumber, int pageSize)
        {
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<MorceauVM> _morceauVMs = null;
            var IdUser = Utilitaire.getIdUserConnected(Request);
            try
            {
                PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();
                idPlylist=idPlylist == 0 ? await Utilitaire.getHistoriqueId(IdUser): idPlylist;

                MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
                ICollection<Morceau> _Morcaulist = await _morceauTask.GetPlylistMorceauAsync(input,(int)idPlylist, pageNumber, pageSize);
                _morceauVMs = _Morcaulist.Where(x => x.url != null).Select(m => new MorceauVM
                {
                    title = HttpUtility.HtmlDecode(m.title),
                    url = m.url,
                    info = m.info
                }).ToList();


            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", IdUser.ToString());

            }
            _model.Add("morceauVMs", _morceauVMs);
            return Utilitaire.constructResponse(_model);
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        private async Task<ItemsToken> GetMorceauSuggestions(string nextKey = "", string query = "", int max = 18)
        {
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            var _youtube = new YoutubeClient();
            ItemsToken itemsToken = new ItemsToken();
            try
            {
                List<MorceauVM> list = new List<MorceauVM>();
                await foreach (var pagedResult in _youtube.Search.GetVideosAsync(query))
                {

                    var listVM = new MorceauVM()
                    {
                        morceauID = pagedResult.Id,
                        title = HttpUtility.HtmlDecode(pagedResult.Title),
                        url = pagedResult.Url,
                        info = getinfo(pagedResult)
                    };

                    string getinfo(YoutubeExplode.Search.VideoSearchResult item)
                    {
                        dynamic info = new ExpandoObject();
                        info.img = item.Thumbnails.FirstOrDefault()?.Url ?? "";
                        info.duration = item.Duration;
                        info.Author = item.Author;
                        return JsonConvert.SerializeObject(info);
                    }
                    itemsToken.MorceauVMs = list;
                    itemsToken.ContinuationToken = "-1";

                }
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());
            }



            return itemsToken;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        private async Task<ItemsToken> GetSuggestionsPlay(string nextKey = "", string query = "", int max = 20)
        {
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            var _youtube = new YoutubeClient();
            ItemsToken itemsToken = new ItemsToken();

            try
            {
                await foreach (var pagedResult in _youtube.Next.GetVideosAppAsync(query, nextKey))
                 {

                     var list = pagedResult.Items.Select(a => new MorceauVM()
                     {
                         morceauID = a.Id,
                         title = HttpUtility.HtmlDecode(a.Title),
                         url = a.Url,
                         info = getinfo(a)
                     }).ToList();

                     string getinfo(YoutubeExplode.Next.VideoNextResult item)
                     {
                         dynamic info = new ExpandoObject();
                         info.img = item.Thumbnails[1]?.Url ?? "";
                         info.duration = item.Duration;
                         info.Author = item.Author;
                         return JsonConvert.SerializeObject(info);
                     }
                     itemsToken.MorceauVMs = list;
                     itemsToken.ContinuationToken = pagedResult.ContinuationToken;

                 }
 
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());
            }

            return itemsToken;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMorceausug(string input = "", string nextKey = "", int count = 0)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<MorceauVM> _morceauDMs = null;
            string nextToken = "";
            try
            {
                if (!string.IsNullOrEmpty(nextKey))
                {
                    var value = await _redisService.GetValueAsync(nextToken);
                    if (!string.IsNullOrEmpty(value))
                    {
                        input = value;
                        await _redisService.DeleteKeyAsync(nextKey);
                    }


                }
                var itemsToken = await GetMorceauSuggestions(nextKey, query: input);
                _morceauDMs = itemsToken.MorceauVMs;
                nextToken = itemsToken.ContinuationToken;
                await _redisService.SetValueAsync(nextToken, input, TimeSpan.FromHours(1));
            }
            catch (Exception ex)
            {

                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("nextToken", nextToken);
            _model.Add("morceauDMs", _morceauDMs);
            return Utilitaire.constructResponse(_model);
        }

        [HttpGet]
        public async Task<IActionResult> GetMorceau(string input)
        {
            _cancellationService.CancelToken();
            Dictionary<String, Object> _model = new Dictionary<String, Object>();

            var cancellationTokenSource = _cancellationService.GetTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            string url = "";
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    var value = await _redisService.GetValueAsync("URL+"+ input);
                    if (!string.IsNullOrEmpty(value))
                    {
                        url = value;
                        
                    }
                    else
                    {
                        var _youtube = new YoutubeClient();
                        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(input);
                        var streamInfo = streamManifest.GetAudioOnlyStreams()
                                                       .FirstOrDefault();
                        url = streamInfo.Url;
                        await _redisService.SetValueAsync("URL+" + input, url, TimeSpan.FromHours(5));
                    }


                }
               
                //string url = "https://www.youtube.com/watch?v=" + input;
              

                _model.Add("Url", url);
                return Utilitaire.constructResponse(_model);
            }
            catch (OperationCanceledException)
            {
                return BadRequest("Requête annulée");
            }
            catch (HttpRequestException)
            {
                return NotFound("Vidéo introuvable");
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetMorceauold(string input)
        {
            
            try
            {
                string url = "https://www.youtube.com/watch?v=" + input;
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();
                string responseData = await response.Content.ReadAsStringAsync();
                var _youtube = new YoutubeClient();
                
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            return null;

        }

        private async Task<IActionResult> GetMorceaubyurl(string url)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            var bufferedStream = new MemoryStream();
            try
            {
                // Obtenir le flux MP3 depuis YouTube
                var stream = await Getmp3fromYoutube(url, 0);

                if (stream == null || stream.Length == 0)
                {
                    return null;
                }

                await stream.CopyToAsync(bufferedStream);

                if (bufferedStream.Length == 0)
                {
                    return null;
                }

                bufferedStream.Position = 0;
                response.Content = new StreamContent(bufferedStream);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/mpeg");


            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            return File(bufferedStream, "audio/mpeg", "0.mp3");
        }
        private async Task<IActionResult> GetMorceaubyurl1(string url, CancellationToken cancellationToken)
        {

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            Dictionary<String, Object> _model = new Dictionary<String, Object>();


            var bufferedStream = new MemoryStream();
            try
            {
                var stream = await Getmp3fromYoutube(url, 0);

                await stream.CopyToAsync(bufferedStream, cancellationToken);
                bufferedStream.Position = 0;
                response.Content = new StreamContent(bufferedStream);

                response.Content.Headers.ContentType = new
                System.Net.Http.Headers.MediaTypeHeaderValue("audio/mpeg");
            }

            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("Url", "nextToken");

            return new FileStreamResult(bufferedStream, "audio/mpeg");

        }


        public async Task<string[]> GetVideoTagsAsync(string videoId)
        {
            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = oldkey,
                    ApplicationName = "YouTubeAutocomplete"
                });

                var videoRequest = youtubeService.Videos.List("snippet");
                videoRequest.Id = videoId;

                var response = await videoRequest.ExecuteAsync();
                var video = response.Items.FirstOrDefault();

                if (video != null && video.Snippet.Tags != null)
                {
                    return video.Snippet.Tags.ToArray();
                }


            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            return null;
        }

         //get directly from youtubeAPIV3
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        private async Task<List<MorceauVM>> GetMorceauSuggestionsV3(string key = "", string query = "", int max = 20)
        {
            try
            {
                if (oldkey == "")
                {
                    ApiCloudTask _apiCloudTask = IoCContainer.Resolve<ApiCloudTask>();
                    oldkey = (await _apiCloudTask.getActiveApiCloud(oldkey))?.FirstOrDefault()?.cle;

                }
                if (string.IsNullOrEmpty(oldkey))
                {
                    return new List<MorceauVM>();
                }

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = oldkey,
                    ApplicationName = "YouTubeAutocomplete"
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = query;
                searchListRequest.MaxResults = max;  // You can adjust the number of results
                //searchListRequest.VideoCategoryId = "10";
                searchListRequest.Type = "video";
                var searchListResponse = await searchListRequest.ExecuteAsync();

                var list = searchListResponse.Items.Where(a => (a.Id.VideoId != null || (a.Snippet.Thumbnails.Medium.Url.Split('/').Count() > 4 && a.Snippet.Thumbnails.Medium.Url.Split('/')[3] == "vi")))
                    .Select(a => new MorceauVM()
                    {
                        title = HttpUtility.HtmlDecode(a.Snippet.Title),
                        url = "https://www.youtube.com/watch?v=" + a.Id.VideoId ?? a.Snippet.Thumbnails.Medium.Url.Split('/')[4],
                        info = getinfo(a)
                    }).ToList();
                return list.Where(url1 =>
                {
                    Uri uri = new Uri(url1.url);
                    var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    string videoId = queryParams["v"];
                    return !string.IsNullOrEmpty(videoId); // Vérifier si 'v' a une valeur
                }).ToList();
                string getinfo(Google.Apis.YouTube.v3.Data.SearchResult item)
                {
                    dynamic info = new ExpandoObject();
                    info.img = item.Snippet.Thumbnails.Medium.Url;
                    info.duration = "00:00:00";
                    info.Author = "";
                    var x = item.Snippet.Thumbnails.Medium.Url.Split('/');
                    var y = x[4];
                    return JsonConvert.SerializeObject(info);
                }

            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            return new List<MorceauVM>();
        }
       

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetsugPlay(string input = "", string nextKey = "", int count = 0)
        {


            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<MorceauVM> _morceauDMs = null;
            Guid newtKey = new Guid();
            var value = "";
            // string url = "www.youtube.com/watch?v=" + input;
            try
            {
                if (!string.IsNullOrEmpty(nextKey))
                {
                     value = await _redisService.GetValueAsync("NEXT+" + nextKey);
                    if (!string.IsNullOrEmpty(value))
                    {
                        input = await _redisService.GetValueAsync(value);
                        await _redisService.DeleteKeyAsync(nextKey);
                        await _redisService.DeleteKeyAsync(value);
                    }


                }
                var itemsToken = await GetSuggestionsPlay(value, query: input);
                _morceauDMs = itemsToken.MorceauVMs;
                var nextToken = itemsToken.ContinuationToken;
               
                if (!string.IsNullOrEmpty(nextToken)) {
                    string _keyNext = "NEXT+" + newtKey;
                await _redisService.SetValueAsync(_keyNext, nextToken, TimeSpan.FromHours(10));
                await _redisService.SetValueAsync(nextToken, input, TimeSpan.FromHours(10));
                } 
            }
            catch (Exception ex)
            {

                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("nextToken", newtKey);
            _model.Add("morceauDMs", _morceauDMs);
            return Utilitaire.constructResponse(_model);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult AddM()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            Dictionary<String, Object> _model = new Dictionary<String, Object>();

            try
            {
                MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
                var Morceau = new Morceau()
                {
                    id = 72,
                    title = "x",
                    url = "x",
                    info = "{img='x02'}"
                };
                _ = _morceauTask.UpdateMorceau(Morceau);

            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            return Utilitaire.constructResponse(_model);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> afterPlayAsync([FromBody] MorceauVM _morceau)
        {
            _cancellationService.CancelToken();
            Dictionary<String, Object> _model = new Dictionary<String, Object>();

            var cancellationTokenSource = _cancellationService.GetTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            string lastUrl = "";
            MorceauVM nextMorceau = new MorceauVM();
            ICollection<MorceauVM> _morceauDMs = null;
            try
            {
                var IdUser = Utilitaire.getIdUserConnected(Request);
                if (!string.IsNullOrEmpty(_morceau?.url))
                {
                    var id = Tools.Utilitaire.ExtractYouTubeVideoId(_morceau.url);
                    //var itemsToken = await GetSuggestionsPlay("", query: _morceau.morceauID);
                    ItemsToken itemsToken; // Remplace 'ItemType' par le type réel des éléments retournés par GetSuggestionsPlay
                    int c = 0;
                    do
                    {
                        c++;
                        itemsToken = await GetSuggestionsPlay("", query: id);
                    }
                    while ((itemsToken == null || itemsToken.MorceauVMs == null || itemsToken.MorceauVMs.Count == 0) && c <= 5);
                    _morceauDMs = itemsToken.MorceauVMs;


                    lastUrl = await _redisService.GetValueAsync("LastURL");

                    nextMorceau = (_morceauDMs != null && _morceauDMs.Count > 1 && !string.IsNullOrEmpty(lastUrl) && _morceauDMs.First().url == lastUrl) ? _morceauDMs.ElementAt(1) : _morceauDMs.FirstOrDefault();
                    await _redisService.SetValueAsync("LastURL+", _morceau.url, TimeSpan.FromHours(3));
                    Thread myThread = new Thread(async () =>
                    {
                        //await addMorceauAsync(_morceau, IdUser);

                    });
                    myThread.Start();

                }
                else throw new Exception("can't save this morceau in history 'morceau null or url is empty'");
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }

            _model.Add("morceauDMs", _morceauDMs);
            _model.Add("nextMorceau", nextMorceau);
            return Utilitaire.constructResponse(_model);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMorceausugurl(string input = "", int count = 0)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<MorceauVM> _morceauDMs = new List<MorceauVM>();
            var _youtube = new YoutubeClient();

            string url = "www.youtube.com/watch?v=" + input;
            try
            {
                MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();

                var x = await _morceauTask.GetMorceauByUrl(url);
                if (x == null || x.id == 0)
                {
                    var video = await _youtube.Videos.GetAsync(url);

                    dynamic info = new ExpandoObject();
                    info.img = video.Thumbnails[1]?.Url ?? "";
                    info.duration = video.Duration;
                    info.Author = video.Author;

                    x = new Morceau()
                    {
                        title = HttpUtility.HtmlDecode(video.Title),
                        url = url,
                        info = JsonConvert.SerializeObject(info)
                    };
                }
                MorceauVM _morceauVM = new MorceauVM()
                {
                    title = HttpUtility.HtmlDecode(x.title),
                    url = x.url,
                    info = x.info
                };
                _morceauDMs.Add(_morceauVM);

            }
            catch (Exception ex)
            {
                //oldkey = "";
                 LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("morceauDMs", _morceauDMs);
            return Utilitaire.constructResponse(_model);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetlistSug(string input = "", int count = 0)
        {
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            List<string> x = new List<string>();

            try
            {
                x = await GetSearchSuggestionsTitle(oldkey, input, count == 1);
                //_etatRep = new EtatReponse() { Code = EtatReponseCode.SUCCESS, Message = "RETURN OK" };
            }
            catch (Exception ex)
            {
                if (count == 0)
                    return await GetlistSug(input, 1);

                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("titls", x);
            return Utilitaire.constructResponse(_model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task addMorceauAsync([FromBody] MorceauVM _morceau)
        {
            var IdUser = Utilitaire.getIdUserConnected(Request);

            MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
            try
            {
                long? idmorceau = null;
                var Xmorceau = await _morceauTask.GetMorceauByUrl(_morceau.url);
                if (Xmorceau == null)
                {
                    var morceau = new Morceau()
                    {
                        title = _morceau.title,
                        url = _morceau.url,
                        info = _morceau.info
                    };
                    idmorceau = await _morceauTask.CreateMorceau(morceau);
                }
                else idmorceau = Xmorceau.id;
               // await addHistory((long)idmorceau, (long)IdUser);
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
        }
        private static async Task<List<string>> GetSearchSuggestionsTitle(string key = "", string query = "", bool expired = false)
        {

            try
            {
                if (oldkey == "" || expired)
                {
                    ApiCloudTask _apiCloudTask = IoCContainer.Resolve<ApiCloudTask>();
                    oldkey = (await _apiCloudTask.getActiveApiCloud(oldkey))?.FirstOrDefault()?.cle;

                }
                if (string.IsNullOrEmpty(oldkey))
                {
                    return null;
                }
                //oldkey = apiKeyArray[rand.Next(apiKeyArray.Count)];
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = oldkey,
                    ApplicationName = "YouTubeAutocomplete"
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = query;
                searchListRequest.MaxResults = 5;  // You can adjust the number of results
                //searchListRequest.VideoCategoryId = "10";
                var searchListResponse = await searchListRequest.ExecuteAsync();
                oldkey = "";
                return searchListResponse.Items.Select(a => HttpUtility.HtmlDecode(a.Snippet.Title)).ToList();

            }
            catch (Exception ex)
            {
                throw;

            }


        }

        public async Task<Stream> GetMp3FromYoutube1(string lien, long IdUser)
        {
            var _youtube = new YoutubeClient();
            try
            {
                var video = await _youtube.Videos.GetAsync(lien);
                var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(video.Id);
                var streamInfo = streamManifest.GetAudioOnlyStreams().FirstOrDefault();

                if (streamInfo != null)
                {
                    var inputStream = await _youtube.Videos.Streams.GetAsync(streamInfo);

                    var limitedStream = new MemoryStream();
                    using (var cts = new CancellationTokenSource())
                    {
                        cts.CancelAfter(TimeSpan.FromSeconds(40));

                        var buffer = new byte[81920];
                        int bytesRead;

                        try
                        {
                            while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length, cts.Token)) > 0)
                            {
                                await limitedStream.WriteAsync(buffer, 0, bytesRead, cts.Token);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                    }

                    limitedStream.Position = 0;
                    return limitedStream;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return null;
        }

        [HttpGet]
        public async Task<Stream> Getmp3fromYoutube(string lien, long IdUser)
        {

            MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
            var _youtube = new YoutubeClient();
            try
            {
                var video = await _youtube.Videos.GetAsync(lien);
                //var x = await _youtube.Search.GetVideosAsync();
                var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(video.Id);
                var streamInfo = streamManifest.GetAudioOnlyStreams()
                                               .FirstOrDefault();
                if (streamInfo != null)
                {
                    //_ = Task.Run(() => addMorceauAsync(lien, video, IdUser));
                    var inputStream = await _youtube.Videos.Streams.GetAsync(streamInfo);
                    inputStream.Position = 0;
                    return inputStream;
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            return null;
        }
[HttpGet]
        public async Task<string> Getmp3fromYoutube2(string lien, long IdUser)
        {

            MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
            var _youtube = new YoutubeClient();
            try
            {
                var video = await _youtube.Videos.GetAsync(lien);
                //var x = await _youtube.Search.GetVideosAsync();
                var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(video.Id);
                var streamInfo = streamManifest.GetAudioOnlyStreams()
                                               .FirstOrDefault();
                if (streamInfo != null)
                {
                    return streamInfo.Url;
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            return null; // Retourner null en cas d'échec
        }
    public MorceauVM getMorceauVM( JsonElement content)
        {
            MorceauVM morceauVM = new MorceauVM();
            var t = content.GetPropertyOrNull("title")?.GetPropertyOrNull("simpleText")?.GetStringOrNull()
            ?? content
                .GetPropertyOrNull("title")
                ?.GetPropertyOrNull("runs")
                ?.EnumerateArrayOrNull()
                ?.Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
                .WhereNotNull()
                .ConcatToString();
            //var y= GetPropertyOrNull((JsonElement)GetPropertyOrNull("title"), "runs");
            morceauVM.title = HttpUtility.HtmlDecode(t);
            morceauVM.morceauID = content.GetPropertyOrNull("videoId")?.GetStringOrNull();
            morceauVM.url=$"https://www.youtube.com/watch?v={morceauVM.morceauID}";
            morceauVM.info = getinfo(content);
            return morceauVM;
        }
        string getinfo(JsonElement content)
        {
          JsonElement? Details = content.GetPropertyOrNull("videoDetails");
            string imgUrl =
            content
            .GetPropertyOrNull("thumbnail")
            ?.GetPropertyOrNull("thumbnails")
            ?.EnumerateArrayOrNull()
            ?.Select(j => j.GetPropertyOrNull("url")?.GetStringOrNull()).FirstOrDefault()
             ?? "";
            dynamic info = new ExpandoObject();
            info.img = imgUrl;
            info.duration = Details
            ?.GetPropertyOrNull("lengthSeconds")
            ?.GetStringOrNull()
            ?.ParseDoubleOrNull()
            ?.Pipe(TimeSpan.FromSeconds);
            info.Author = "";
            return JsonConvert.SerializeObject(info);
        }
        public static JsonElement Parse(string source)
        {
            using var document = JsonDocument.Parse(source);
            return document.RootElement.Clone();
        }
    

         }
}
