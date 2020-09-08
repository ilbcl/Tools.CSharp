using System.Collections.Generic;

namespace Tools
{
    public class CachedItemsSearchResult<T, TK>
    {
        public List<T> CachedItems { get; set; }

        public List<TK> NotCachedItemsIds { get; set; }
    }
}