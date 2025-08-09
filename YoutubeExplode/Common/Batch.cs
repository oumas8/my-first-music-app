using System.Collections.Generic;
using YoutubeExplode.Utils.Extensions;

namespace YoutubeExplode.Common;

/// <summary>
/// Generic collection of items returned by a single request.
/// </summary>
public class Batch<T>(IReadOnlyList<T> items)
    where T : IBatchItem
{
    /// <summary>
    /// Items included in the batch.
    /// </summary>
    public IReadOnlyList<T> Items { get; } = items;
}

internal static class Batch
{
    public static Batch<T> Create<T>(IReadOnlyList<T> items)
        where T : IBatchItem => new(items);

    public static TokenizedBatch<T> CreateWithToken<T>(
        IReadOnlyList<T> items,
        string? continuationToken
    )
        where T : IBatchItem => new(items, continuationToken);
}

public class TokenizedBatch<T> : Batch<T>
    where T : IBatchItem
{
    /// <summary>
    /// The continuation token for fetching the next batch.
    /// </summary>
    public string? ContinuationToken { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="TokenizedBatch{T}"/>.
    /// </summary>
    /// <param name="items">The items in the batch.</param>
    /// <param name="continuationToken">The continuation token for the next batch.</param>
    public TokenizedBatch(IReadOnlyList<T> items, string? continuationToken)
        : base(items)
    {
        ContinuationToken = continuationToken;
    }
}

internal static class BatchExtensions
{
    public static IAsyncEnumerable<T> FlattenAsync<T>(this IAsyncEnumerable<Batch<T>> source)
        where T : IBatchItem => source.SelectManyAsync(b => b.Items);
}
