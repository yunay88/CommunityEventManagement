namespace CommunityEventManagement.Infrastructure.DataStructures
{
    /// <summary>
    /// Navigation Stack — tracks page navigation history in the Blazor app.
    /// Enables breadcrumb navigation and back-button functionality.
    /// 
    /// DATA STRUCTURE: Stack(T) — Last In, First Out (LIFO)
    /// 
    /// Why Stack for navigation?
    ///   - Browser history IS a stack — most recent page is on top
    ///   - "Go back" = pop from stack
    ///   - "Navigate to" = push onto stack
    ///   - Stack perfectly models LIFO (Last In, First Out) navigation
    /// 
    /// Time Complexity:
    ///   - Push (navigate to page): O(1)
    ///   - Pop (go back): O(1)
    ///   - Peek (see current page): O(1)
    /// 
    /// Demonstrates:
    ///   - Stack(T) data structure
    ///   - LIFO behaviour
    ///   - Domain-specific use case for Stack
    /// </summary>
    public class NavigationStack
    {
        // The underlying Stack(T) data structure
        private readonly Stack<NavigationEntry> _history = new();

        // Maximum history depth to prevent unbounded growth
        private readonly int _maxDepth;

        public NavigationStack(int maxDepth = 20)
        {
            if (maxDepth <= 0)
                throw new ArgumentException("Max depth must be positive.", nameof(maxDepth));
            _maxDepth = maxDepth;
        }

        /// <summary>
        /// Records navigation to a new page.
        /// Stack operation: Push — O(1)
        /// </summary>
        public void NavigateTo(string url, string pageTitle)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL cannot be empty.", nameof(url));
            if (string.IsNullOrWhiteSpace(pageTitle))
                throw new ArgumentException("Page title cannot be empty.", nameof(pageTitle));

            // Enforce max depth — remove oldest entry if at capacity
            if (_history.Count >= _maxDepth)
            {
                // Stack doesn't support removing from bottom directly
                // Convert to list, trim, rebuild
                var entries = _history.ToList();
                entries.RemoveAt(entries.Count - 1); // remove oldest
                _history.Clear();
                foreach (var entry in entries.AsEnumerable().Reverse())
                    _history.Push(entry);
            }

            _history.Push(new NavigationEntry(url, pageTitle, DateTime.Now));
        }

        /// <summary>
        /// Goes back to the previous page.
        /// Stack operation: Pop — O(1)
        /// Returns the URL to navigate to, or null if no history.
        /// </summary>
        public NavigationEntry? GoBack()
        {
            if (_history.Count <= 1) return null;

            // Pop current page
            _history.Pop();

            // Peek at the now-current page (previous page)
            return _history.Count > 0 ? _history.Peek() : null;
        }

        /// <summary>
        /// Returns current page without changing history.
        /// Stack operation: Peek — O(1)
        /// </summary>
        public NavigationEntry? GetCurrentPage()
        {
            return _history.Count > 0 ? _history.Peek() : null;
        }

        /// <summary>
        /// Clears all navigation history.
        /// Used when user logs out or navigates to home.
        /// </summary>
        public void Clear() => _history.Clear();

        /// <summary>
        /// Returns full breadcrumb trail from oldest to newest.
        /// Stack gives newest-first, so we reverse for breadcrumb display.
        /// </summary>
        public IEnumerable<NavigationEntry> GetBreadcrumbs()
        {
            return _history.ToList().AsEnumerable().Reverse();
        }

        /// <summary>
        /// Returns history in LIFO order (most recent first).
        /// This is the natural Stack enumeration order.
        /// </summary>
        public IEnumerable<NavigationEntry> GetHistoryNewestFirst()
        {
            return _history.ToList();
        }

        public bool CanGoBack => _history.Count > 1;
        public int HistoryDepth => _history.Count;
        public bool IsEmpty => _history.Count == 0;

        public override string ToString()
        {
            var current = GetCurrentPage();
            return $"NavigationStack | " +
                   $"Current: {current?.PageTitle ?? "None"} | " +
                   $"Depth: {HistoryDepth} | " +
                   $"CanGoBack: {CanGoBack}";
        }
    }

    /// <summary>
    /// Represents a single navigation entry in the stack.
    /// Immutable record — cannot be changed after creation.
    /// </summary>
    public record NavigationEntry(
        string Url,
        string PageTitle,
        DateTime NavigatedAt)
    {
        public override string ToString() =>
            $"{PageTitle} ({Url}) at {NavigatedAt:HH:mm:ss}";
    }
}