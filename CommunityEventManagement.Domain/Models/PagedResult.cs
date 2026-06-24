namespace CommunityEventManagement.Domain.Models
{
    /// <summary>
    /// Generic wrapper for paginated results.
    /// Demonstrates: generic class with type parameter,
    /// encapsulation of pagination metadata.
    /// </summary>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        /// <summary>Computed: total number of pages.</summary>
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>Computed: whether a previous page exists.</summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>Computed: whether a next page exists.</summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Factory method — creates a PagedResult from a full list.
        /// Demonstrates: static factory method pattern.
        /// </summary>
        public static PagedResult<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var list = source.ToList();
            var items = list
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = list.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}