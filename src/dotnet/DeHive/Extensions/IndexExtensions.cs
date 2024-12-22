namespace DeHive.Abstractions;

internal static class IndexExtensions
{
    public static IEnumerator<int> GetEnumerator(this Range r) => Enumerable.Range(r.Start.Value, r.End.Value).GetEnumerator();
}