using Domain.Models.Enteties;
using Domain.Models.VM;
using firstmusic.Tools;
using infrastructure.Helpers;
using infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Tasks;
using Tasks.contract;

namespace firstmusic.Controllers
{
   
    public class SettingController : Controller
    {
        private readonly CancellationService _cancellationService;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RedisService _redisService;
        public SettingController(RedisService redisService, HttpClient httpClient, CancellationService cancellationService, IHttpContextAccessor httpContextAccessor)
        {
            _redisService = redisService;
            _httpClient = httpClient;
            _cancellationService = cancellationService;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET api/values
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult>Get()
        {
            SettingTask _settingTask = IoCContainer.Resolve<SettingTask>();
            var IdUser = 0;
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            SettingVM settingVM = new SettingVM();

            try
            {
                 IdUser = infrastructure.Helpers.Utilitaire.getIdUserConnected(Request);
                Setting s = await _settingTask.GetSettingByUser(IdUser);
                if (s!=null && string.IsNullOrEmpty(s?.jsonSetting)) 
                settingVM = System.Text.Json.JsonSerializer.Deserialize<SettingVM>(s.jsonSetting);
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", IdUser.ToString());
            }
            _model.Add("settingVM", settingVM);
            return infrastructure.Helpers.Utilitaire.constructResponse(_model);
           // return new string[] { "value1", "value2" };
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update([FromBody]  SettingVM settingVM)
        {
            SettingTask _settingTask = IoCContainer.Resolve<SettingTask>();
            var IdUser = 0;
            Dictionary<String, Object> _model = new Dictionary<String, Object>();
            bool result = false;
            try
            {
                IdUser = infrastructure.Helpers.Utilitaire.getIdUserConnected(Request);
                Setting s = await _settingTask.GetSettingByUser(IdUser);
                if (s != null)
                    s.jsonSetting = System.Text.Json.JsonSerializer.Serialize(settingVM);
                result = await _settingTask.UpdateSetting(s);
            }
            catch (Exception ex)
            {
                LogService.LogError(_httpContextAccessor.HttpContext, ex, "Erreur spécifique à cette action", IdUser.ToString());
            }
            _model.Add("result", result);
            return infrastructure.Helpers.Utilitaire.constructResponse(_model);
            // return new string[] { "value1", "value2" };
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
