using CommunityEventManagement.Infrastructure.DataStructures;
using FluentAssertions;
using Xunit;

namespace CommunityEventManagement.Tests.Unit.DataStructures
{
    /// <summary>
    /// Tests for NavigationStack — Stack(T) data structure.
    /// </summary>
    public class NavigationStackTests
    {
        private readonly NavigationStack _stack;

        public NavigationStackTests()
        {
            _stack = new NavigationStack(maxDepth: 10);
        }

        [Fact]
        public void NewStack_IsEmpty()
        {
            _stack.IsEmpty.Should().BeTrue();
            _stack.HistoryDepth.Should().Be(0);
            _stack.CanGoBack.Should().BeFalse();
        }

        [Fact]
        public void NavigateTo_AddsToStack()
        {
            _stack.NavigateTo("/events", "Events");

            _stack.HistoryDepth.Should().Be(1);
            _stack.IsEmpty.Should().BeFalse();
        }

        [Fact]
        public void GetCurrentPage_ReturnsLastNavigated()
        {
            _stack.NavigateTo("/events", "Events");
            _stack.NavigateTo("/events/1", "Event Detail");

            var current = _stack.GetCurrentPage();

            current!.Url.Should().Be("/events/1");
            current.PageTitle.Should().Be("Event Detail");
        }

        [Fact]
        public void GetCurrentPage_EmptyStack_ReturnsNull()
        {
            _stack.GetCurrentPage().Should().BeNull();
        }

        [Fact]
        public void GoBack_ReturnssPreviousPage()
        {
            _stack.NavigateTo("/events", "Events");
            _stack.NavigateTo("/events/1", "Event Detail");

            var previous = _stack.GoBack();

            previous!.Url.Should().Be("/events");
            previous.PageTitle.Should().Be("Events");
        }

        [Fact]
        public void GoBack_SingleEntry_ReturnsNull()
        {
            _stack.NavigateTo("/home", "Home");

            var result = _stack.GoBack();

            result.Should().BeNull();
        }

        [Fact]
        public void GoBack_EmptyStack_ReturnsNull()
        {
            var result = _stack.GoBack();
            result.Should().BeNull();
        }

        [Fact]
        public void CanGoBack_WithHistory_ReturnsTrue()
        {
            _stack.NavigateTo("/home", "Home");
            _stack.NavigateTo("/events", "Events");

            _stack.CanGoBack.Should().BeTrue();
        }

        [Fact]
        public void CanGoBack_SinglePage_ReturnsFalse()
        {
            _stack.NavigateTo("/home", "Home");

            _stack.CanGoBack.Should().BeFalse();
        }

        [Fact]
        public void Clear_EmptiesStack()
        {
            _stack.NavigateTo("/home", "Home");
            _stack.NavigateTo("/events", "Events");

            _stack.Clear();

            _stack.IsEmpty.Should().BeTrue();
            _stack.HistoryDepth.Should().Be(0);
        }

        [Fact]
        public void GetBreadcrumbs_ReturnsOldestFirst()
        {
            _stack.NavigateTo("/home", "Home");
            _stack.NavigateTo("/events", "Events");
            _stack.NavigateTo("/events/1", "Event Detail");

            var breadcrumbs = _stack.GetBreadcrumbs().ToList();

            breadcrumbs[0].Url.Should().Be("/home");
            breadcrumbs[1].Url.Should().Be("/events");
            breadcrumbs[2].Url.Should().Be("/events/1");
        }

        [Fact]
        public void GetHistoryNewestFirst_ReturnsMostRecentFirst()
        {
            _stack.NavigateTo("/home", "Home");
            _stack.NavigateTo("/events", "Events");

            var history = _stack.GetHistoryNewestFirst().ToList();

            history[0].Url.Should().Be("/events");
            history[1].Url.Should().Be("/home");
        }

        [Fact]
        public void NavigateTo_EmptyUrl_ThrowsArgumentException()
        {
            Action act = () => _stack.NavigateTo("", "Title");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void NavigateTo_EmptyTitle_ThrowsArgumentException()
        {
            Action act = () => _stack.NavigateTo("/url", "");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_ZeroMaxDepth_ThrowsArgumentException()
        {
            Action act = () => new NavigationStack(0);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void NavigationEntry_Record_EqualityWorks()
        {
            var entry1 = new NavigationEntry("/home", "Home", DateTime.Now);
            var entry2 = new NavigationEntry("/home", "Home", entry1.NavigatedAt);

            entry1.Should().Be(entry2);
        }

        [Fact]
        public void LIFO_Behaviour_LastInFirstOut()
        {
            // Demonstrates LIFO: last pushed = first retrieved
            _stack.NavigateTo("/page1", "Page 1");
            _stack.NavigateTo("/page2", "Page 2");
            _stack.NavigateTo("/page3", "Page 3");

            // Current should be Page 3 (last pushed)
            _stack.GetCurrentPage()!.Url.Should().Be("/page3");

            // GoBack once → Page 2
            _stack.GoBack()!.Url.Should().Be("/page2");

            // GoBack again → Page 1
            _stack.GoBack()!.Url.Should().Be("/page1");
        }
    }
}