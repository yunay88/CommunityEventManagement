using CommunityEventManagement.Domain.Algorithms;
using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Enums;
using CommunityEventManagement.Infrastructure.DataStructures;
using FluentAssertions;
using Xunit;

namespace CommunityEventManagement.Tests.Unit.DataStructures
{
    /// <summary>
    /// Tests for ActivityLinkedList — LinkedList(T) data structure.
    /// </summary>
    public class ActivityLinkedListTests
    {
        private readonly ActivityLinkedList _list;

        // Helper to create test activities
        private static Activity CreateActivity(string name, ActivityType type = ActivityType.Workshop)
            => new Activity(name, "Test description", type, 60);

        public ActivityLinkedListTests()
        {
            _list = new ActivityLinkedList(1, "Test Event");
        }

        [Fact]
        public void NewList_IsEmpty()
        {
            _list.IsEmpty.Should().BeTrue();
            _list.Count.Should().Be(0);
        }

        [Fact]
        public void AddActivity_AddsToEnd()
        {
            var a1 = CreateActivity("Workshop");
            var a2 = CreateActivity("Talk");

            _list.AddActivity(a1);
            _list.AddActivity(a2);

            var inOrder = _list.GetActivitiesInOrder().ToList();
            inOrder.Should().HaveCount(2);
            inOrder[0].Name.Should().Be("Workshop");
            inOrder[1].Name.Should().Be("Talk");
        }

        [Fact]
        public void AddActivityFirst_AddsToBeginning()
        {
            var a1 = CreateActivity("Talk");
            var a2 = CreateActivity("Opening");

            _list.AddActivity(a1);
            _list.AddActivityFirst(a2);

            _list.GetFirstActivity()!.Name.Should().Be("Opening");
        }

        [Fact]
        public void GetFirstActivity_EmptyList_ReturnsNull()
        {
            _list.GetFirstActivity().Should().BeNull();
        }

        [Fact]
        public void GetLastActivity_EmptyList_ReturnsNull()
        {
            _list.GetLastActivity().Should().BeNull();
        }

        [Fact]
        public void GetActivitiesInReverseOrder_ReturnsReversed()
        {
            _list.AddActivity(CreateActivity("First"));
            _list.AddActivity(CreateActivity("Second"));
            _list.AddActivity(CreateActivity("Third"));

            var reversed = _list.GetActivitiesInReverseOrder().ToList();

            reversed[0].Name.Should().Be("Third");
            reversed[1].Name.Should().Be("Second");
            reversed[2].Name.Should().Be("First");
        }

        [Fact]
        public void AddActivity_Null_ThrowsArgumentNullException()
        {
            Action act = () => _list.AddActivity(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Count_ReflectsCorrectNumber()
        {
            _list.AddActivity(CreateActivity("A"));
            _list.AddActivity(CreateActivity("B"));
            _list.AddActivity(CreateActivity("C"));

            _list.Count.Should().Be(3);
        }

        [Fact]
        public void ToString_ReturnsFormattedSchedule()
        {
            _list.AddActivity(CreateActivity("Workshop"));
            _list.AddActivity(CreateActivity("Talk"));

            var result = _list.ToString();

            result.Should().Contain("Workshop");
            result.Should().Contain("Talk");
            result.Should().Contain("→");
        }
    }
}