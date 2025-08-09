using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;

namespace YoutubeExplode.Next;

/// <summary>
/// Metadata associated with a YouTube channel returned by a Next query.
/// </summary>
public class ChannelNextResult(ChannelId id, string title, IReadOnlyList<Thumbnail> thumbnails)
    : INextResult,
        IChannel
{
    /// <inheritdoc />
    public ChannelId Id { get; } = id;

    /// <inheritdoc cref="IChannel.Url" />
    public string Url => $"https://www.youtube.com/channel/{Id}";

    /// <inheritdoc cref="IChannel.Title" />
    public string Title { get; } = title;

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; } = thumbnails;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Channel ({Title})";
}
