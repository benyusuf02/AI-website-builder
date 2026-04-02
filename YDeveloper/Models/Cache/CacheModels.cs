namespace YDeveloper.Models.Cache
{
    public class CacheEntry<T>
    {
        public T Value { get; set; } = default!;
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
        public TimeSpan Duration { get; set; }
        public bool IsExpired => DateTime.UtcNow > CachedAt.Add(Duration);
    }

    public class CacheStatistics
    {
        public long TotalHits { get; set; }
        public long TotalMisses { get; set; }
        public double HitRate => TotalHits + TotalMisses > 0 ? (double)TotalHits / (TotalHits + TotalMisses) : 0;
    }
}
