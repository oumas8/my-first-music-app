using YoutubeExplode.Common;

namespace YoutubeExplode.Next;

/// <summary>
/// <p>
///     Abstract result returned by a Next query.
///     Use pattern matching to handle specific instances of this type.
/// </p>
/// <p>
///     Can be either one of the following:
///     <list type="bullet">
///         <item><see cref="VideoNextResult" /></item>
///         <item><see cref="PlaylistNextResult" /></item>
///         <item><see cref="ChannelNextResult" /></item>
///     </list>
/// </p>
/// </summary>
public interface INextResult : IBatchItem
{
    /// <summary>
    /// Result URL.
    /// </summary>
    string Url { get; }

    /// <summary>
    /// Result title.
    /// </summary>
    string Title { get; }
}
