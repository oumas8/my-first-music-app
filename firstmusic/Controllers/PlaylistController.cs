using Domain.Constants;
using Domain.DTO;
using Domain.Models.Enteties;
using Domain.Models.VM;
using firstmusic.Tools;
using infrastructure.Helpers;
using infrastructure.Repositories;
using infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tasks;
using YoutubeExplode;
using Utilitaire = infrastructure.Helpers.Utilitaire;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397840

namespace firstmusic.Controllers
{
 
    public class PlaylistController : Controller
    {
        private readonly CancellationService _cancellationService;
        public static string oldkey = "";
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PlaylistController(HttpClient httpClient, CancellationService cancellationService, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _cancellationService = cancellationService;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: api/<PlaylisteController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllPlylist()
        {
            
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<PlaylistVM> _playlistVMs = null;
            PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();

            try
            {
                var IdUser = Utilitaire.getIdUserConnected(Request);
                List<Playlist> Plylists = (await _playlistTask.GetPlaylistMorceauType(IdUser, PlaylistConst.Normale)).ToList();

                _playlistVMs = Plylists.Select(m => new PlaylistVM
                {
                    id=m.id,
                    name = m.name,
                    parametre = m.parametre??"{}",
                    //count = m.MorceauPlaylists.Where(x=>x.Morceau!=null).Count()
                    count = 0
                }).ToList();


            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("playlistVMs", _playlistVMs);
            return Utilitaire.constructResponse(_model);
        }
         [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetQueuelist()
        {
            
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<PlaylistVM> _queuelistVMs = null;
            PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();

            try
            {
                var IdUser = Utilitaire.getIdUserConnected(Request);
                List<Playlist> queuelist = (await _playlistTask.GetPlaylistMorceauType(IdUser, PlaylistConst.Queue)).OrderByDescending(p => p.id)
    .Take(3).ToList();

                _queuelistVMs = queuelist.Select(m => new PlaylistVM
                {
                    id=m.id,
                    name = m.name,
                    parametre = m.parametre??"{}",
                    count = 0
                }).ToList();


            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("queuelistVMs", _queuelistVMs);
            return Utilitaire.constructResponse(_model);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMorceauPlaylist(int id)
        {

            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<MorceauVM> _morceauVMs = null;
            try
            {
                MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
                ICollection<Morceau> _Morcaulist = await _morceauTask.GetPlylisteWithMorceauxAsync((int)id);
                _morceauVMs = _Morcaulist.Select(m => new MorceauVM
                {
                    title = m.title,
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

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddMorceauPlaylist([FromBody] morceauPlylist value)
        {
             Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<MorceauVM> _morceauVMs = null;
            var _youtube = new YoutubeClient();

            try
            {
                MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
                long? idmorceau = null;
                //var video = await _youtube.Videos.GetAsync(value.valuestring);
                var Xmorceau = await _morceauTask.GetMorceauByUrl(value.morceauVM.url);
                if (Xmorceau == null)
                {
                    //var video = await _youtube.Videos.GetAsync(value.valuestring);
                    //var info = Utilitaire.getinfo(video);
                    var morceau = new Morceau()
                    {
                        title = value.morceauVM.title,
                        url = value.morceauVM.url,
                        info = value.morceauVM.info
                    };
                    idmorceau = await _morceauTask.CreateMorceau(morceau);
                }
                else
                if (Xmorceau != null)
                {
                    idmorceau = Xmorceau.id;
                } 

                MorceauPlaylistTask _morceauPlaylistTask = IoCContainer.Resolve<MorceauPlaylistTask>();
                _ = await _morceauPlaylistTask.CreateMorceauPlaylist(new MorceauPlaylist() { idPlaylist = (long)value.idplylist, idMorceau = (long)idmorceau, fl_Active = true, place = 0, dt_creat = DateTime.Now.ToString() });


                
                

            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("morceauVMs", _morceauVMs);
            return Utilitaire.constructResponse(_model);
        }      
        /*public async Task<IActionResult> AddMorceauPlaylist([FromBody] id value)
        {
             Dictionary<String, Object> _model = new Dictionary<String, Object>();
            ICollection<MorceauVM> _morceauVMs = null;
            var _youtube = new YoutubeClient();

            try
            {
                MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
                long? idmorceau = null;
                var video = await _youtube.Videos.GetAsync(value.valuestring);
                var Xmorceau = await _morceauTask.GetMorceauByUrl(value.valuestring);
                if (Xmorceau == null)
                {
                    var morceau = new Morceau()
                    {
                        title = video.Title,
                        url = value.valuestring,
                        info = Utilitaire.getinfo(video)
                    };
                    idmorceau = await _morceauTask.CreateMorceau(morceau);
                }
                else idmorceau = Xmorceau.id;

                MorceauPlaylistTask _morceauPlaylistTask = IoCContainer.Resolve<MorceauPlaylistTask>();
                _ = await _morceauPlaylistTask.CreateMorceauPlaylist(new MorceauPlaylist() { idPlaylist = (long)value.valueint, idMorceau = (long)idmorceau, fl_Active = true, place = 0, dt_creat = DateTime.Now.ToString() });



            }
            catch (Exception e)
            {

            }
            _model.Add("morceauVMs", _morceauVMs);
            return Utilitaire.constructResponse(_model);
        }*/
        // GET api/<PlaylisteController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PlaylisteController>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> addPlaylist([FromBody] NewPlylist value)
        {
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();
            long x = 0;
            try{
                var match = new[] { PlaylistConst.Normale, PlaylistConst.Queue }
                            .FirstOrDefault(p => p.Equals(value.type, StringComparison.OrdinalIgnoreCase));
                object parametre = new
                {
                    type = match?? PlaylistConst.Normale
                };
                var IdUser = Utilitaire.getIdUserConnected(Request);
                var playlist = new Playlist()
                {
                    name = value.name,
                    flActive = true,
                    flshow = true,
                    parametre = JsonConvert.SerializeObject(parametre),
                    idUser = IdUser
                };
                 x = await _playlistTask.CreatePlaylist(playlist);
                if ((value.morceauVMs?.Count()??0)>0 && x!=0)
                {
                    foreach(MorceauVM morceauVM in value.morceauVMs)
                    {
                        _ = AddMorceauPlaylist(new morceauPlylist() { idplylist = (int)x, morceauVM= morceauVM });
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("id", x);
            return Utilitaire.constructResponse(_model);
        }

        // PUT api/<PlaylisteController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PlaylisteController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        
    }
}
