using Domain.Constants;
using Domain.Models.Enteties;
using infrastructure.Helpers;
using infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.Web.Http;
using Tasks;

namespace firstmusic.Tools
{
    public static class Utilitaire
    {  
        private static readonly RedisService _redisService;
        static Utilitaire()
        {
            _redisService = IoCContainer.Resolve<RedisService>();
        }
      
        public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> transform) =>
             transform(input);
        public static IActionResult constructResponse1(Object _object)
        {
            return new OkObjectResult(_object);
        }
        public static int getIdUserConnected1(HttpRequest Request)
        {
            try
            {
                var authorizationHeader = Request.Headers["Authorization"].ToString();

                // Extract the token by removing the "Bearer " prefix
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var IdUser = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

                return int.Parse(IdUser);
            }
            catch
            {
                return 0;
            }


        }
        public static string ExtractYouTubeVideoId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            // Regular expression to match the video ID
            var regex = new Regex(@"(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");

            var match = regex.Match(url);

            return match.Success ? match.Groups[1].Value : null;
        }
        public static string getinfo()
        {
            dynamic info = new ExpandoObject();
            
            return JsonConvert.SerializeObject(info);
        }
        public async static Task<int> getHistoriqueId1(int idUser)
        {
            int idPlylist;
            var stridPlylist = await _redisService.GetValueAsync("Historique+" + idUser);
            try
            {
                if (!string.IsNullOrEmpty(stridPlylist))
                {
                    var result = int.TryParse(stridPlylist, out idPlylist);
                }
                else
                {
                    PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();
                    idPlylist = (int)(await _playlistTask.CheckPlylistAsync(idUser, PlaylistConst.History)).id;
                    await _redisService.SetValueAsync("Historique+" + idUser, idPlylist.ToString(), TimeSpan.FromDays(30));
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return idPlylist;
        }
    }
}
