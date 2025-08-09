using Domain.Models.VM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks.contract;
using Tasks;
using Domain.DTO;
using Domain.Constants;
using Domain.Models.Enteties;
using AuthService.Tools;
using Domain.config;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading;
using System.Web;
using infrastructure.Helpers;
using Utilitaire = infrastructure.Helpers.Utilitaire;
using infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AuthService.Controllers
{
    public class AuthController : Controller
    {

        private readonly JwtConf _jwtConf;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RedisService _redisService;
        public AuthController(IOptionsMonitor<JwtConf> jwtConf, IHttpContextAccessor httpContextAccessor, RedisService redisService)
        {
            _redisService = redisService;
            _jwtConf = jwtConf.CurrentValue;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> login([FromBody] Auth _auth)
        {
            UserVM _userVM = null;
            string token = null;
            string RefreshToken = null;
            ICollection<MorceauVM> _morceauVMs = null;
            SettingVM _settingVM = null;
            ICollection<PlaylistVM> _playlistVMs = null;
            ICollection<PlaylistVM> _queuelistVMs = null;
            PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();
            UserTask _userTask = IoCContainer.Resolve<UserTask>();
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            int state = 0;
            try
            {
                User _user = await _userTask.getUserByAuth(_auth.login, _auth.password);
               
                if (_user != null)
                {
                    _userVM = new UserVM()
                    {
                        id = _user.id,
                        name = _user.name
                    };
                    var resulrtoken = await generateJwtToken((int)_user.id);
                    token = resulrtoken[0];
                    RefreshToken = resulrtoken[1];
                    _morceauVMs = await CheckHistoryAsync(_user.id);
                    _settingVM=await CheckSettingAsync(_user.id);
                    await _redisService.SetValueAsync("setting-" + _user.id, JsonSerializer.Serialize(_settingVM));
                    List<Playlist> playlist = (await _playlistTask.GetPlaylistMorceauType((int)_user.id, PlaylistConst.Normale)).ToList();
                    _playlistVMs = playlist.Select(m => new PlaylistVM
                    {
                        id = m.id,
                        name = m.name,
                        parametre = m.parametre ?? "{}",
                        count = 0
                    }).ToList();

                    List<Playlist> queuelistVMs = (await _playlistTask.GetPlaylistMorceauType((int)_user.id, PlaylistConst.Queue)).ToList();
                    _queuelistVMs = queuelistVMs.Select(m => new PlaylistVM
                    {
                        id = m.id,
                        name = m.name,
                        parametre = m.parametre ?? "{}",
                        count = 0
                    }).ToList();
                    state = 1;
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());
                state = -1;
            }
            _model.Add("userVM", _userVM);
            if (!string.IsNullOrEmpty(token)) _model.Add("token", token);
            if (!string.IsNullOrEmpty(token)) _model.Add("RefreshToken", RefreshToken);
            _model.Add("morceauVMs", _morceauVMs);
            _model.Add("playlistVMs", _playlistVMs);
            _model.Add("queuelistVMs", _queuelistVMs);
            _model.Add("settingVM", _settingVM);
            _model.Add("state", state);
            return new OkObjectResult(_model);
        }
        private async Task<SettingVM> CheckSettingAsync(long idUser)
        {
            SettingVM _settingVM = new SettingVM();
            SettingTask _settingTask = IoCContainer.Resolve<SettingTask>();
            try
            {
                Setting s = await _settingTask.GetSettingByUser((int)idUser);
                if (s != null )
                {
                    if(!string.IsNullOrEmpty(s?.jsonSetting))
                    _settingVM = JsonSerializer.Deserialize<SettingVM>(s.jsonSetting);
                }
                else
                {
                    s = new Setting()
                    {
                        idUser = idUser,
                        jsonSetting = JsonSerializer.Serialize(_settingVM)
                    };
                    _ = await _settingTask.CreateSetting(s);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return _settingVM;
        }
        private async Task<ICollection<MorceauVM>> CheckHistoryAsync(long idUser)
        {
            ICollection<MorceauVM> _morceauVMs = null;
            try
            {
                PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();
                long idPlylistHistory = await Utilitaire.getHistoriqueId((int)idUser);
                long idPlylistFavorie = await Utilitaire.getfavorieId((int)idUser);
               
                    MorceauTask _morceauTask = IoCContainer.Resolve<MorceauTask>();
                    ICollection<Morceau> _Morcaulist = await _morceauTask.GetPlylistMorceauAsync("", (int)idPlylistHistory, 1, 20);
                    _morceauVMs = _Morcaulist.Where(x => x.url != null).Select(m => new MorceauVM
                    {
                        title = HttpUtility.HtmlDecode(m.title),
                        url = m.url,
                        info = m.info
                    }).ToList();
                
            }
            catch
            {
                throw;
            }

            return _morceauVMs;
        }
        [HttpPost]
        public async Task<IActionResult> refreshToken([FromBody] RefreshToken _refreshtoken)
        {
            string token = null;
            keyvalidationTask _keyvalidationTask = IoCContainer.Resolve<keyvalidationTask>();
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            string newToken = null;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(_refreshtoken.token);
                var IdUser = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var refreshtokencach = await _redisService.GetValueAsync("refreshToken-" + IdUser);
                var refreshtokenId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (!string.IsNullOrEmpty(refreshtokencach) && refreshtokencach == _refreshtoken.refreshtoken)
                {
                    newToken = generateNewJwtToken(int.Parse(IdUser));

                }
                
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            if (!string.IsNullOrEmpty(newToken)) _model.Add("token", newToken);
            return new OkObjectResult(_model);
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
                    title = m.title,
                    url = m.url,
                    info = m.info
                }).ToList(); ;


            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", Utilitaire.getIdUserConnected(Request).ToString());

            }
            _model.Add("morceauVMs", _morceauVMs);
            return Utilitaire.constructResponse(_model);
        }
        private async Task<List<string>> generateJwtToken(int id)
        {
            var resut = new List<string>();
            try
            {
                var refreshToken = Guid.NewGuid().ToString();
                keyvalidationTask _keyvalidationTask = IoCContainer.Resolve<keyvalidationTask>();
                _ = await _keyvalidationTask.Deletekeyvalidation(null, id);
                await _redisService.SetValueAsync("refreshToken-" + id, refreshToken, TimeSpan.FromDays(5));
                
                var tokenDesc = generateNewJwtToken(id);

                resut.Add(tokenDesc);
                resut.Add(refreshToken);
            }
            catch
            {
                throw;
            }



            return resut;
        }
        private string generateNewJwtToken(int idUser)
        {
            var jwtToken = "";
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtConf.secret);
                var tokenDesc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]{
                    new Claim("id",idUser.ToString()),
                   

                }),
                    Expires = DateTime.Now.AddMinutes(15),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = jwtHandler.CreateToken(tokenDesc);
                jwtToken = jwtHandler.WriteToken(token);
            }
            catch
            {
                throw;
            }

            return jwtToken;
        }
    }
}
