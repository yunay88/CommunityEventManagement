namespace CommunityEventManagement.Domain.Algorithms
{
    /// <summary>
    /// Implements searching algorithms for use throughout the application.
    /// 
    /// BINARY SEARCH:
    ///   - Time Complexity: O(log n) — much faster than linear O(n) for large lists
    ///   - Requirement: list must be SORTED before searching
    ///   - How it works: repeatedly halves the search space
    ///     1. Look at the middle element
    ///     2. If it matches → found
    ///     3. If target is smaller → search left half only
    ///     4. If target is larger → search right half only
    ///     5. Repeat until found or search space is empty
    /// 
    /// Demonstrates:
    ///   - Generic method with type constraint
    ///   - Static utility class
    ///   - Algorithm implementation with documented complexity
    /// </summary>
    public static class SearchAlgorithms
    {
        /// <summary>
        /// Binary Search — finds an item in a SORTED list by integer key.
        /// Generic method works with any type T.
        /// 
        /// Time Complexity: O(log n)
        /// Space Complexity: O(1)
        /// 
        /// Returns: index of found item, or -1 if not found.
        /// </summary>
        /// <typeparam name="T">The type of items in the list</typeparam>
        /// <param name="sortedList">A sorted list to search through</param>
        /// <param name="target">The integer key value to find</param>
        /// <param name="keySelector">Function that extracts the key from an item</param>
        public static int BinarySearch<T>(
            IList<T> sortedList,
            int target,
            Func<T, int> keySelector)
        {
            if (sortedList == null)
                throw new ArgumentNullException(nameof(sortedList));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            int left = 0;
            int right = sortedList.Count - 1;

            while (left <= right)
            {
                // Calculate middle index — avoids integer overflow
                int middle = left + (right - left) / 2;
                int middleKey = keySelector(sortedList[middle]);

                if (middleKey == target)
                {
                    // Found — return the index
                    return middle;
                }
                else if (middleKey < target)
                {
                    // Target is in the RIGHT half — discard left half
                    left = middle + 1;
                }
                else
                {
                    // Target is in the LEFT half — discard right half
                    right = middle - 1;
                }
            }

            // Not found
            return -1;
        }

        /// <summary>
        /// Binary Search overload — searches by string key.
        /// Demonstrates: method overloading on algorithms.
        /// 
        /// Time Complexity: O(log n)
        /// </summary>
        public static int BinarySearch<T>(
            IList<T> sortedList,
            string target,
            Func<T, string> keySelector,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (sortedList == null)
                throw new ArgumentNullException(nameof(sortedList));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            int left = 0;
            int right = sortedList.Count - 1;

            while (left <= right)
            {
                int middle = left + (right - left) / 2;
                string middleKey = keySelector(sortedList[middle]);

                int compareResult = string.Compare(middleKey, target, comparison);

                if (compareResult == 0)
                {
                    return middle;
                }
                else if (compareResult < 0)
                {
                    left = middle + 1;
                }
                else
                {
                    right = middle - 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Linear Search — fallback for unsorted lists.
        /// Time Complexity: O(n)
        /// Included for comparison with Binary Search performance.
        /// </summary>
        public static int LinearSearch<T>(
            IList<T> list,
            int target,
            Func<T, int> keySelector)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            for (int i = 0; i < list.Count; i++)
            {
                if (keySelector(list[i]) == target)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Finds an item directly by its integer key using Binary Search.
        /// Returns the item itself (not the index).
        /// </summary>
        public static T? FindById<T>(
            IList<T> sortedList,
            int id,
            Func<T, int> idSelector)
        {
            int index = BinarySearch(sortedList, id, idSelector);
            return index >= 0 ? sortedList[index] : default;
        }
    }
}