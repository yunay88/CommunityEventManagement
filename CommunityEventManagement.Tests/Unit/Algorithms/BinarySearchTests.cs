using CommunityEventManagement.Domain.Algorithms;
using FluentAssertions;
using Xunit;

namespace CommunityEventManagement.Tests.Unit.Algorithms
{
    /// <summary>
    /// Unit tests for Binary Search algorithm.
    /// Tests: happy path, edge cases, boundary conditions, not-found cases.
    /// </summary>
    public class BinarySearchTests
    {
        // ─── Integer Key Tests ───────────────────────────────────────

        [Fact]
        public void BinarySearch_Int_FindsItemInMiddle_ReturnsCorrectIndex()
        {
            // Arrange
            var list = new List<int> { 1, 3, 5, 7, 9, 11, 13 };

            // Act
            int result = SearchAlgorithms.BinarySearch(list, 7, x => x);

            // Assert
            result.Should().Be(3); // index 3
        }

        [Fact]
        public void BinarySearch_Int_FindsFirstItem_ReturnsZero()
        {
            // Arrange — boundary: first element
            var list = new List<int> { 2, 4, 6, 8, 10 };

            // Act
            int result = SearchAlgorithms.BinarySearch(list, 2, x => x);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void BinarySearch_Int_FindsLastItem_ReturnsLastIndex()
        {
            // Arrange — boundary: last element
            var list = new List<int> { 2, 4, 6, 8, 10 };

            // Act
            int result = SearchAlgorithms.BinarySearch(list, 10, x => x);

            // Assert
            result.Should().Be(4);
        }

        [Fact]
        public void BinarySearch_Int_ItemNotFound_ReturnsNegativeOne()
        {
            // Arrange — edge case: target not in list
            var list = new List<int> { 1, 3, 5, 7, 9 };

            // Act
            int result = SearchAlgorithms.BinarySearch(list, 4, x => x);

            // Assert
            result.Should().Be(-1);
        }

        [Fact]
        public void BinarySearch_Int_EmptyList_ReturnsNegativeOne()
        {
            // Arrange — edge case: empty list
            var list = new List<int>();

            // Act
            int result = SearchAlgorithms.BinarySearch(list, 5, x => x);

            // Assert
            result.Should().Be(-1);
        }

        [Fact]
        public void BinarySearch_Int_SingleElementFound_ReturnsZero()
        {
            // Arrange — boundary: single element list, element found
            var list = new List<int> { 42 };

            // Act
            int result = SearchAlgorithms.BinarySearch(list, 42, x => x);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void BinarySearch_Int_SingleElementNotFound_ReturnsNegativeOne()
        {
            // Arrange — boundary: single element list, element not found
            var list = new List<int> { 42 };

            // Act
            int result = SearchAlgorithms.BinarySearch(list, 99, x => x);

            // Assert
            result.Should().Be(-1);
        }

        [Fact]
        public void BinarySearch_Int_NullList_ThrowsArgumentNullException()
        {
            // Arrange — edge case: null input
            List<int> nullList = null!;

            // Act
            Action act = () => SearchAlgorithms.BinarySearch(nullList, 5, x => x);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        // ─── Generic Object Tests ─────────────────────────────────────

        [Fact]
        public void BinarySearch_GenericObject_FindsByIdProperty_ReturnsCorrectIndex()
        {
            // Arrange — simulating searching participants by Id
            var items = new List<(int Id, string Name)>
            {
                (1, "Alice"),
                (3, "Bob"),
                (5, "Carol"),
                (7, "David"),
                (9, "Emma")
            };

            // Act
            int result = SearchAlgorithms.BinarySearch(items, 5, x => x.Id);

            // Assert
            result.Should().Be(2); // Carol is at index 2
        }

        [Fact]
        public void BinarySearch_GenericObject_NotFound_ReturnsNegativeOne()
        {
            var items = new List<(int Id, string Name)>
            {
                (1, "Alice"),
                (3, "Bob"),
                (5, "Carol")
            };

            int result = SearchAlgorithms.BinarySearch(items, 4, x => x.Id);

            result.Should().Be(-1);
        }

        // ─── String Key Tests ─────────────────────────────────────────

        [Fact]
        public void BinarySearch_String_FindsItemByName_ReturnsCorrectIndex()
        {
            // Arrange — sorted alphabetically
            var names = new List<string> { "Alice", "Bob", "Carol", "David", "Emma" };

            // Act
            int result = SearchAlgorithms.BinarySearch(names, "Carol", x => x);

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public void BinarySearch_String_CaseInsensitive_FindsItem()
        {
            var names = new List<string> { "Alice", "Bob", "Carol", "David" };

            int result = SearchAlgorithms.BinarySearch(
                names, "carol", x => x, StringComparison.OrdinalIgnoreCase);

            result.Should().Be(2);
        }

        [Fact]
        public void BinarySearch_String_NotFound_ReturnsNegativeOne()
        {
            var names = new List<string> { "Alice", "Bob", "Carol" };

            int result = SearchAlgorithms.BinarySearch(names, "Zara", x => x);

            result.Should().Be(-1);
        }

        // ─── FindById Tests ───────────────────────────────────────────

        [Fact]
        public void FindById_ItemExists_ReturnsItem()
        {
            // Arrange
            var items = new List<(int Id, string Name)>
            {
                (1, "Alice"),
                (2, "Bob"),
                (3, "Carol")
            };

            // Act
            var result = SearchAlgorithms.FindById(items, 2, x => x.Id);

            // Assert
            result.Should().Be((2, "Bob"));
        }

        [Fact]
        public void FindById_ItemNotExists_ReturnsDefault()
        {
            var items = new List<(int Id, string Name)>
            {
                (1, "Alice"),
                (2, "Bob")
            };

            var result = SearchAlgorithms.FindById(items, 99, x => x.Id);

            result.Should().Be(default);
        }

        // ─── Linear Search Comparison Tests ───────────────────────────

        [Fact]
        public void LinearSearch_FindsItem_ReturnsCorrectIndex()
        {
            // Unsorted list — linear search works on unsorted
            var list = new List<int> { 9, 3, 7, 1, 5 };

            int result = SearchAlgorithms.LinearSearch(list, 7, x => x);

            result.Should().Be(2);
        }

        [Fact]
        public void LinearSearch_ItemNotFound_ReturnsNegativeOne()
        {
            var list = new List<int> { 9, 3, 7, 1, 5 };

            int result = SearchAlgorithms.LinearSearch(list, 99, x => x);

            result.Should().Be(-1);
        }

        [Theory]
        [InlineData(1, 0)]   // first element
        [InlineData(5, 2)]   // middle element
        [InlineData(9, 4)]   // last element
        public void BinarySearch_VariousPositions_ReturnsCorrectIndex(
            int target, int expectedIndex)
        {
            // Arrange
            var list = new List<int> { 1, 3, 5, 7, 9 };

            // Act
            int result = SearchAlgorithms.BinarySearch(list, target, x => x);

            // Assert
            result.Should().Be(expectedIndex);
        }
    }
}