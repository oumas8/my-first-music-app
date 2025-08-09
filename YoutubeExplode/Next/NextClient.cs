using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Common;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Utils.Extensions;

namespace YoutubeExplode.Next;

public class NextBatchResult
{
    public List<INextResult> Results { get; set; }
    public string ContinuationToken { get; set; }
}

/// <summary>
/// Operations related to YouTube Next.
/// </summary>
public class NextClient(HttpClient http)
{
    private readonly NextController _controller = new(http);
    private readonly NextAppController _controllerApp = new(http);

    /// <summary>
    /// Enumerates batches of Next results returned by the specified query.
    /// </summary>
    public async IAsyncEnumerable<Batch<INextResult>> GetResultBatchesAsync(
        string NextQuery,
        NextFilter NextFilter,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var encounteredIds = new HashSet<string>(StringComparer.Ordinal);
        var continuationToken = default(string?);

        do
        {
            var resultBatchestokenAsync = await GetResultBatchestokenAsync(
                NextQuery,
                NextFilter,
                continuationToken,
                encounteredIds,
                cancellationToken
            );

            yield return Batch.Create(resultBatchestokenAsync.Results);

            continuationToken = resultBatchestokenAsync.ContinuationToken;
        } while (!string.IsNullOrWhiteSpace(continuationToken));
    }

    //==========================================================================================================================
    public async IAsyncEnumerable<TokenizedBatch<INextResult>> GetResultAppBatchesAsync(
        string NextQuery,
        NextFilter NextFilter,
        string continuationToken = default(string?),
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var encounteredIds = new HashSet<string>(StringComparer.Ordinal);

        var resultBatchestokenAsync = await GetResultBatchestoVideokenAsync(
            NextQuery,
            NextFilter,
            continuationToken,
            encounteredIds,
            cancellationToken
        );

        yield return new TokenizedBatch<INextResult>(
            resultBatchestokenAsync.Results,
            resultBatchestokenAsync.ContinuationToken
        );
    }

    public async Task<NextBatchResult> GetResultBatchestokenAsync(
        string NextQuery,
        NextFilter NextFilter,
        string continuationToken,
        HashSet<string> encounteredIds,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var NextResults = await _controller.GetNextResponseAsync(
            NextQuery,
            NextFilter,
            continuationToken,
            cancellationToken
        );
        var results = new List<INextResult>();
        foreach (var videoData in NextResults.Videos)
        {
            if (NextFilter is not NextFilter.None and not NextFilter.Video)
            {
                Debug.Fail("Did not expect videos in Next results.");
                break;
            }

            var videoId =
                videoData.Id
                ?? throw new YoutubeExplodeException("Failed to extract the video ID.");

            // Don't yield the same result twice
            if (!encounteredIds.Add(videoId))
                continue;

            var videoTitle =
                videoData.Title
                ?? throw new YoutubeExplodeException("Failed to extract the video title.");

            var videoChannelTitle =
                videoData.Author
                ?? throw new YoutubeExplodeException("Failed to extract the video author.");

            var videoChannelId =
                videoData.ChannelId
                ?? throw new YoutubeExplodeException("Failed to extract the video channel ID.");

            var videoThumbnails = videoData
                .Thumbnails.Select(t =>
                {
                    var thumbnailUrl =
                        t.Url
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the video thumbnail URL."
                        );

                    var thumbnailWidth =
                        t.Width
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the video thumbnail width."
                        );

                    var thumbnailHeight =
                        t.Height
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the video thumbnail height."
                        );

                    var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);

                    return new Thumbnail(thumbnailUrl, thumbnailResolution);
                })
                .Concat(Thumbnail.GetDefaultSet(videoId))
                .ToArray();

            var video = new VideoNextResult(
                videoId,
                videoTitle,
                new Author(videoChannelId, videoChannelTitle),
                videoData.Duration,
                videoThumbnails
            );

            results.Add(video);
        }

        // Playlist results
        /*foreach (var playlistData in NextResults.Playlists)
        {
            if (NextFilter is not NextFilter.None and not NextFilter.Playlist)
            {
                Debug.Fail("Did not expect playlists in Next results.");
                break;
            }

            var playlistId =
                playlistData.Id
                ?? throw new YoutubeExplodeException("Failed to extract the playlist ID.");

            // Don't yield the same result twice
            if (!encounteredIds.Add(playlistId))
                continue;

            var playlistTitle =
                playlistData.Title
                ?? throw new YoutubeExplodeException("Failed to extract the playlist title.");

            // System playlists have no author
            var playlistAuthor =
                !string.IsNullOrWhiteSpace(playlistData.ChannelId)
                && !string.IsNullOrWhiteSpace(playlistData.Author)
                    ? new Author(playlistData.ChannelId, playlistData.Author)
                    : null;

            var playlistThumbnails = playlistData
                .Thumbnails.Select(t =>
                {
                    var thumbnailUrl =
                        t.Url
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the playlist thumbnail URL."
                        );

                    var thumbnailWidth =
                        t.Width
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the playlist thumbnail width."
                        );

                    var thumbnailHeight =
                        t.Height
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the playlist thumbnail height."
                        );

                    var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);

                    return new Thumbnail(thumbnailUrl, thumbnailResolution);
                })
                .ToArray();

            var playlist = new PlaylistNextResult(
                playlistId,
                playlistTitle,
                playlistAuthor,
                playlistThumbnails
            );

            results.Add(playlist);
        }
        // Channel results
        foreach (var channelData in NextResults.Channels)
        {
            if (NextFilter is not NextFilter.None and not NextFilter.Channel)
            {
                Debug.Fail("Did not expect channels in Next results.");
                break;
            }

            var channelId =
                channelData.Id
                ?? throw new YoutubeExplodeException("Failed to extract the channel ID.");

            var channelTitle =
                channelData.Title
                ?? throw new YoutubeExplodeException("Failed to extract the channel title.");

            var channelThumbnails = channelData
                .Thumbnails.Select(t =>
                {
                    var thumbnailUrl =
                        t.Url
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the channel thumbnail URL."
                        );

                    var thumbnailWidth =
                        t.Width
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the channel thumbnail width."
                        );

                    var thumbnailHeight =
                        t.Height
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the channel thumbnail height."
                        );

                    var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);

                    return new Thumbnail(thumbnailUrl, thumbnailResolution);
                })
                .ToArray();

            var channel = new ChannelNextResult(channelId, channelTitle, channelThumbnails);

            results.Add(channel);
        }
        */return new NextBatchResult()
        {
            Results = results,
            ContinuationToken = NextResults.ContinuationToken,
        };
    }

    public async Task<NextBatchResult> GetResultBatchestoVideokenAsync(
        string NextQuery,
        NextFilter NextFilter,
        string continuationToken,
        HashSet<string> encounteredIds,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var NextResults = await _controllerApp.GetNextAppResponseAsync(
            NextQuery,
            NextFilter,
            continuationToken,
            cancellationToken
        );
        var results = new List<INextResult>();
        foreach (var videoData in NextResults.Videos)
        {
            if (NextFilter is not NextFilter.None and not NextFilter.Video)
            {
                Debug.Fail("Did not expect videos in Next results.");
                break;
            }

            var videoId =
                videoData.Id
                ?? throw new YoutubeExplodeException("Failed to extract the video ID.");

            // Don't yield the same result twice
            if (!encounteredIds.Add(videoId))
                continue;

            var videoTitle =
                videoData.Title
                ?? throw new YoutubeExplodeException("Failed to extract the video title.");

            var videoChannelTitle =
                videoData.Author
                ?? throw new YoutubeExplodeException("Failed to extract the video author.");

            var videoChannelId =
                videoData.ChannelId
                ?? throw new YoutubeExplodeException("Failed to extract the video channel ID.");

            var videoThumbnails = videoData
                .Thumbnails.Select(t =>
                {
                    var thumbnailUrl =
                        t.Url
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the video thumbnail URL."
                        );

                    var thumbnailWidth =
                        t.Width
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the video thumbnail width."
                        );

                    var thumbnailHeight =
                        t.Height
                        ?? throw new YoutubeExplodeException(
                            "Failed to extract the video thumbnail height."
                        );

                    var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);

                    return new Thumbnail(thumbnailUrl, thumbnailResolution);
                })
                .Concat(Thumbnail.GetDefaultSet(videoId))
                .ToArray();

            var video = new VideoNextResult(
                videoId,
                videoTitle,
                new Author(videoChannelId, videoChannelTitle),
                videoData.Duration,
                videoThumbnails
            );

            results.Add(video);
        }

        // Playlist results
        return new NextBatchResult()
        {
            Results = results,
            ContinuationToken = NextResults.ContinuationToken,
        };
    }

    /// <summary>
    /// Enumerates batches of Next results returned by the specified query.
    /// </summary>
    public IAsyncEnumerable<Batch<INextResult>> GetResultBatchesAsync(
        string NextQuery,
        CancellationToken cancellationToken = default
    ) => GetResultBatchesAsync(NextQuery, NextFilter.None, cancellationToken);

    /// <summary>
    /// Enumerates Next results returned by the specified query.
    /// </summary>
    public IAsyncEnumerable<INextResult> GetResultsAsync(
        string NextQuery,
        CancellationToken cancellationToken = default
    ) => GetResultBatchesAsync(NextQuery, cancellationToken).FlattenAsync();

    /// <summary>
    /// Enumerates video Next results returned by the specified query.
    /// </summary>
    public IAsyncEnumerable<VideoNextResult> GetVideosAsync(
        string NextQuery,
        CancellationToken cancellationToken = default
    ) =>
        GetResultBatchesAsync(NextQuery, NextFilter.Video, cancellationToken)
            .FlattenAsync()
            .OfTypeAsync<VideoNextResult>();

    /// <summary>
    /// Enumerates video Next results returned by the specified query.
    /// </summary>
    public async IAsyncEnumerable<PagedResult<VideoNextResult>> GetVideosAppAsync(
        string NextQuery,
        string continuationToken,
        CancellationToken cancellationToken = default
    )
    {
        await foreach (
            var batch in GetResultAppBatchesAsync(
                NextQuery,
                NextFilter.Video,
                continuationToken,
                cancellationToken
            )
        )
        {
            var videos = batch.Items.OfType<VideoNextResult>().ToList();
            yield return new PagedResult<VideoNextResult>(videos, batch.ContinuationToken);
        }
    }

    /// <summary>
    /// Enumerates playlist Next results returned by the specified query.
    /// </summary>
    public IAsyncEnumerable<PlaylistNextResult> GetPlaylistsAsync(
        string NextQuery,
        CancellationToken cancellationToken = default
    ) =>
        GetResultBatchesAsync(NextQuery, NextFilter.Playlist, cancellationToken)
            .FlattenAsync()
            .OfTypeAsync<PlaylistNextResult>();

    /// <summary>
    /// Enumerates channel Next results returned by the specified query.
    /// </summary>
    public IAsyncEnumerable<ChannelNextResult> GetChannelsAsync(
        string NextQuery,
        CancellationToken cancellationToken = default
    ) =>
        GetResultBatchesAsync(NextQuery, NextFilter.Channel, cancellationToken)
            .FlattenAsync()
            .OfTypeAsync<ChannelNextResult>();
}
