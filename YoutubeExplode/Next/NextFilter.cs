namespace YoutubeExplode.Next;

/// <summary>
/// Filter applied to a YouTube Next query.
/// </summary>
public enum NextFilter
{
    /// <summary>
    /// No filter applied.
    /// </summary>
    None,

    /// <summary>
    /// Only Next for videos.
    /// </summary>
    Video,

    /// <summary>
    /// Only Next for playlists.
    /// </summary>
    Playlist,

    /// <summary>
    /// Only Next for channels.
    /// </summary>
    Channel,
}
