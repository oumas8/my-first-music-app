using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Bridge;
using YoutubeExplode.Utils;

namespace YoutubeExplode.Next;

internal class NextAppController(HttpClient http)
{
    public async ValueTask<NextResponseApp> GetNextAppResponseAsync(
        string NextQuery,
        NextFilter NextFilter,
        string? continuationToken,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://www.youtube.com/youtubei/v1/next"
        );

        request.Content = new StringContent(
            // lang=json
            $$"""
            {
              "videoId": {{Json.Serialize(NextQuery)}},
              "params": {{Json.Serialize(NextFilter switch
              {
                  NextFilter.Video => "EgIQAQ%3D%3D",
                 // NextFilter.Playlist => "EgIQAw%3D%3D",
                  //NextFilter.Channel => "EgIQAg%3D%3D",
                  _ => null
              })}},
              "continuation": {{Json.Serialize(continuationToken)}},
              "context": {
                "client": {
                  "clientName": "WEB",
                  "clientVersion": "2.20210408.08.00",
                  "hl": "en",
                  "gl": "US",
                  "utcOffsetMinutes": 0
                }
              }
            }
            """
        );

        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return NextResponseApp.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
    }
}
