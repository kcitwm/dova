using System; 

namespace Dova
{
    public class CacheKey
    {
        public string Key = string.Empty;  
        public string DependencyFileName = string.Empty;
        public int CachePriority = 1;
        public long CurrentGroupID = 0;
        public long CurrentID = 0;
        public long NextGroupID = 0;
        public long NextID = 0;
        public DateTime NextAbsoluteExpiration = DateTime.Now.AddDays(1);
        public TimeSpan NextSlidSpan = new TimeSpan(1, 0, 0); 

    }
}
