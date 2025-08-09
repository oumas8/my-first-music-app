using Domain.Constants;
using infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks;
using YoutubeExplode;
using AngleSharp.Dom;
using Domain.Models.Enteties;

namespace infrastructure.Helpers
{
    public class Utilitaire
    {
        private static readonly RedisService _redisService;
        static Utilitaire()
        {
            _redisService = IoCContainer.Resolve<RedisService>();
        }
        public async static Task<int> getHistoriqueId(int idUser)
        {
            int idPlylist;
            var stridPlylist = await _redisService.GetValueAsync("Historique+" + idUser);
            try
            {
                if (!string.IsNullOrEmpty(stridPlylist) && stridPlylist!="0")
                {
                    var result = int.TryParse(stridPlylist, out idPlylist);
                }
                else
                {
                    PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();
                    var _plylist = (await _playlistTask.CheckPlylistAsync(idUser, PlaylistConst.History));
                    if (_plylist?.id == 0)
                    {
                        idPlylist = (int)await _playlistTask.CreatePlaylist(new Playlist() { idUser = idUser, name = PlaylistConst.History, flActive = true, flshow = true });
                    }
                    else idPlylist = (int)_plylist.id;
                    await _redisService.SetValueAsync("Historique+" + idUser, idPlylist.ToString(), TimeSpan.FromDays(30));
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return idPlylist;
        }
        public async static Task<int> getfavorieId(int idUser)
        {
            int idPlylist;
            var stridPlylist = await _redisService.GetValueAsync("Favorie+" + idUser);
            try
            {
                if (!string.IsNullOrEmpty(stridPlylist) && stridPlylist != "0")
                {
                    var result = int.TryParse(stridPlylist, out idPlylist);
                }
                else
                {
                    PlaylistTask _playlistTask = IoCContainer.Resolve<PlaylistTask>();
                    var _plylist = (await _playlistTask.CheckPlylistAsync(idUser, PlaylistConst.Favorie));
                    if ((_plylist?.id ?? 0)== 0)
                    {
                        idPlylist = (int)await _playlistTask.CreatePlaylist(new Playlist() { idUser = idUser, name = PlaylistConst.Favorie, flActive = true, flshow = true });
                    }
                    else idPlylist = (int)_plylist.id;
                    await _redisService.SetValueAsync("Favorie+" + idUser, idPlylist.ToString(), TimeSpan.FromDays(30));
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return idPlylist;
        }
        public static string getinfo(YoutubeExplode.Videos.Video item)
        {
            dynamic info = new ExpandoObject();
            info.img = item.Thumbnails.FirstOrDefault()?.Url ?? "";
            info.duration = item.Duration;
            info.Author = item.Author;
            return JsonConvert.SerializeObject(info);
        }
        public static async Task<string> getUrlenligne(string input)
        {
            var _youtube = new YoutubeExplode.YoutubeClient();
            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(input);
            var streamInfo = streamManifest.GetAudioOnlyStreams()
                            .OrderByDescending(s => s.Bitrate)
                            .FirstOrDefault();
           return streamInfo.Url;
        }
        public static async Task<string> getUrlenligneIntern(string input)
        {
            var _youtube = new YoutubeExplode.YoutubeClient();
            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(input);
            var streamInfo = streamManifest.GetAudioOnlyStreams()
                            .OrderByDescending(s => s.Bitrate) 
                            .FirstOrDefault();
            return streamInfo.Url;
        }
        public static int getIdUserConnected(HttpRequest Request)
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
                throw;
               
            }

        }
        public static IActionResult constructResponse(Object _object)
        {
            return new OkObjectResult(_object);
        }
    }
}
