using CommunityEventManagement.Domain.Algorithms;
using FluentAssertions;
using Xunit;

namespace CommunityEventManagement.Tests.Unit.Algorithms
{
    /// <summary>
    /// Unit tests for Insertion Sort and Bubble Sort algorithms.
    /// Tests: ascending, descending, edge cases, boundary conditions,
    /// already sorted, reverse sorted, duplicates.
    /// </summary>
    public class SortAlgorithmTests
    {
        // ─── Insertion Sort — Integer Key Tests ──────────────────────

        [Fact]
        public void InsertionSort_UnsortedIntegers_SortsAscending()
        {
            // Arrange
            var list = new List<int> { 5, 2, 8, 1, 9, 3 };

            // Act
            SortAlgorithms.InsertionSort(list, x => x);

            // Assert
            list.Should().BeInAscendingOrder();
        }

        [Fact]
        public void InsertionSort_UnsortedIntegers_SortsDescending()
        {
            // Arrange
            var list = new List<int> { 5, 2, 8, 1, 9, 3 };

            // Act
            SortAlgorithms.InsertionSort(list, x => x, descending: true);

            // Assert
            list.Should().BeInDescendingOrder();
        }

        [Fact]
        public void InsertionSort_AlreadySorted_RemainsCorrect()
        {
            // Edge case: already sorted input
            var list = new List<int> { 1, 2, 3, 4, 5 };

            SortAlgorithms.InsertionSort(list, x => x);

            list.Should().BeInAscendingOrder();
        }

        [Fact]
        public void InsertionSort_ReverseSorted_SortsCorrectly()
        {
            // Edge case: worst case for insertion sort (reverse sorted)
            var list = new List<int> { 5, 4, 3, 2, 1 };

            SortAlgorithms.InsertionSort(list, x => x);

            list.Should().BeInAscendingOrder();
        }

        [Fact]
        public void InsertionSort_SingleElement_RemainsUnchanged()
        {
            // Boundary: single element
            var list = new List<int> { 42 };

            SortAlgorithms.InsertionSort(list, x => x);

            list.Should().ContainSingle().Which.Should().Be(42);
        }

        [Fact]
        public void InsertionSort_EmptyList_DoesNotThrow()
        {
            // Boundary: empty list
            var list = new List<int>();

            Action act = () => SortAlgorithms.InsertionSort(list, x => x);

            act.Should().NotThrow();
            list.Should().BeEmpty();
        }

        [Fact]
        public void InsertionSort_DuplicateValues_SortsCorrectly()
        {
            // Edge case: duplicate values
            var list = new List<int> { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3 };

            SortAlgorithms.InsertionSort(list, x => x);

            list.Should().BeInAscendingOrder();
        }

        [Fact]
        public void InsertionSort_NullList_ThrowsArgumentNullException()
        {
            // Edge case: null input
            List<int> nullList = null!;

            Action act = () => SortAlgorithms.InsertionSort(nullList, x => x);

            act.Should().Throw<ArgumentNullException>();
        }

        // ─── Insertion Sort — Generic Object Tests ────────────────────

        [Fact]
        public void InsertionSort_ObjectsByIntKey_SortsCorrectly()
        {
            // Arrange — simulating sorting events by a numeric ID
            var items = new List<(int Id, string Name)>
            {
                (5, "Eve"),
                (2, "Bob"),
                (8, "Henry"),
                (1, "Alice"),
                (3, "Carol")
            };

            // Act
            SortAlgorithms.InsertionSort(items, x => x.Id);

            // Assert
            items.Select(x => x.Id).Should().BeInAscendingOrder();
            items[0].Name.Should().Be("Alice");
            items[1].Name.Should().Be("Bob");
            items[2].Name.Should().Be("Carol");
        }

        [Fact]
        public void InsertionSort_ObjectsByStringKey_SortsAlphabetically()
        {
            // Arrange — simulating sorting participants by name
            var items = new List<(int Id, string Name)>
            {
                (3, "Carol"),
                (1, "Alice"),
                (4, "David"),
                (2, "Bob")
            };

            // Act
            SortAlgorithms.InsertionSort(items, x => x.Name);

            // Assert
            items[0].Name.Should().Be("Alice");
            items[1].Name.Should().Be("Bob");
            items[2].Name.Should().Be("Carol");
            items[3].Name.Should().Be("David");
        }

        [Fact]
        public void InsertionSort_ObjectsByDateTime_SortsByDate()
        {
            // Arrange — simulating sorting events by start date
            var today = DateTime.Today;
            var items = new List<(string Name, DateTime Date)>
            {
                ("Event C", today.AddDays(10)),
                ("Event A", today.AddDays(1)),
                ("Event D", today.AddDays(20)),
                ("Event B", today.AddDays(5))
            };

            // Act
            SortAlgorithms.InsertionSort(items, x => x.Date);

            // Assert — should be in date ascending order
            items[0].Name.Should().Be("Event A");
            items[1].Name.Should().Be("Event B");
            items[2].Name.Should().Be("Event C");
            items[3].Name.Should().Be("Event D");
        }

        // ─── Bubble Sort Tests ────────────────────────────────────────

        [Fact]
        public void BubbleSort_UnsortedIntegers_SortsAscending()
        {
            var list = new List<int> { 64, 34, 25, 12, 22, 11, 90 };

            SortAlgorithms.BubbleSort(list, x => x);

            list.Should().BeInAscendingOrder();
        }

        [Fact]
        public void BubbleSort_AlreadySorted_RemainsCorrect()
        {
            // Optimisation test: should exit early
            var list = new List<int> { 1, 2, 3, 4, 5 };

            SortAlgorithms.BubbleSort(list, x => x);

            list.Should().BeInAscendingOrder();
        }

        [Fact]
        public void BubbleSort_SingleElement_DoesNotThrow()
        {
            var list = new List<int> { 1 };

            Action act = () => SortAlgorithms.BubbleSort(list, x => x);

            act.Should().NotThrow();
        }

        [Fact]
        public void BubbleSort_Descending_SortsCorrectly()
        {
            var list = new List<int> { 3, 1, 4, 1, 5, 9, 2, 6 };

            SortAlgorithms.BubbleSort(list, x => x, descending: true);

            list.Should().BeInDescendingOrder();
        }

        // ─── GetSorted Tests (immutable sort) ─────────────────────────

        [Fact]
        public void GetSorted_ReturnsNewSortedList_OriginalUnchanged()
        {
            // Arrange
            var original = new List<int> { 5, 2, 8, 1, 9 };
            var originalCopy = original.ToList();

            // Act
            var sorted = SortAlgorithms.GetSorted(original, x => x);

            // Assert — original unchanged
            original.Should().BeEquivalentTo(originalCopy, options => options.WithStrictOrdering());

            // New list is sorted
            sorted.Should().BeInAscendingOrder();
        }

        [Fact]
        public void GetSortedById_ReturnsSortedByIdAscending()
        {
            var items = new List<(int Id, string Name)>
            {
                (5, "Eve"),
                (1, "Alice"),
                (3, "Carol")
            };

            var sorted = SortAlgorithms.GetSortedById(items, x => x.Id);

            sorted[0].Id.Should().Be(1);
            sorted[1].Id.Should().Be(3);
            sorted[2].Id.Should().Be(5);
        }

        // ─── Theory Tests — multiple inputs ──────────────────────────

        [Theory]
        [InlineData(new[] { 3, 1, 2 }, new[] { 1, 2, 3 })]
        [InlineData(new[] { 5, 5, 5 }, new[] { 5, 5, 5 })]
        [InlineData(new[] { 1 }, new[] { 1 })]
        [InlineData(new[] { 2, 1 }, new[] { 1, 2 })]
        public void InsertionSort_Theory_VariousInputs(int[] input, int[] expected)
        {
            // Arrange
            var list = input.ToList();

            // Act
            SortAlgorithms.InsertionSort(list, x => x);

            // Assert
            list.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        }
    }
}