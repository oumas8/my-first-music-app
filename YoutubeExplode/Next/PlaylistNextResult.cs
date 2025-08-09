using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;

namespace YoutubeExplode.Next;

/// <summary>
/// Metadata associated with a YouTube playlist returned by a Next query.
/// </summary>
public class PlaylistNextResult(
    PlaylistId id,
    string title,
    Author? author,
    IReadOnlyList<Thumbnail> thumbnails
) : INextResult, IPlaylist
{
    /// <inheritdoc />
    public PlaylistId Id { get; } = id;

    /// <inheritdoc cref="IPlaylist.Url" />
    public string Url => $"https://www.youtube.com/playlist?list={Id}";

    /// <inheritdoc cref="IPlaylist.Title" />
    public string Title { get; } = title;

    /// <inheritdoc />
    public Author? Author { get; } = author;

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; } = thumbnails;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Playlist ({Title})";
}
