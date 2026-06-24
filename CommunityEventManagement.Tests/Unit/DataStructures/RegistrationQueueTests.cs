using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Infrastructure.DataStructures;
using FluentAssertions;
using Xunit;

namespace CommunityEventManagement.Tests.Unit.DataStructures
{
    /// <summary>
    /// Tests for RegistrationQueue — Queue(T) data structure.
    /// </summary>
    public class RegistrationQueueTests
    {
        private readonly RegistrationQueue _queue;

        public RegistrationQueueTests()
        {
            _queue = new RegistrationQueue(1, "Test Event");
        }

        [Fact]
        public void Queue_NewQueue_IsEmpty()
        {
            _queue.WaitingCount.Should().Be(0);
            _queue.HasWaiting.Should().BeFalse();
        }

        [Fact]
        public void Enqueue_AddOneRegistration_CountIsOne()
        {
            var reg = new Registration(1, 1);
            _queue.AddToWaitingList(reg);

            _queue.WaitingCount.Should().Be(1);
            _queue.HasWaiting.Should().BeTrue();
        }

        [Fact]
        public void Enqueue_AddMultiple_FIFO_OrderPreserved()
        {
            // Arrange — add 3 registrations
            var reg1 = new Registration(1, 1);
            var reg2 = new Registration(1, 2);
            var reg3 = new Registration(1, 3);

            _queue.AddToWaitingList(reg1);
            _queue.AddToWaitingList(reg2);
            _queue.AddToWaitingList(reg3);

            // Act — process one
            var processed = _queue.ProcessNext();

            // Assert — first in = first out (FIFO)
            processed!.ParticipantId.Should().Be(1);
            _queue.WaitingCount.Should().Be(2);
        }

        [Fact]
        public void Dequeue_EmptyQueue_ReturnsNull()
        {
            var result = _queue.ProcessNext();
            result.Should().BeNull();
        }

        [Fact]
        public void Peek_DoesNotRemoveItem()
        {
            var reg = new Registration(1, 1);
            _queue.AddToWaitingList(reg);

            // Peek twice — should not change count
            var peek1 = _queue.PeekNext();
            var peek2 = _queue.PeekNext();

            peek1.Should().NotBeNull();
            peek2.Should().NotBeNull();
            _queue.WaitingCount.Should().Be(1); // still 1
        }

        [Fact]
        public void IsParticipantWaiting_ParticipantInQueue_ReturnsTrue()
        {
            _queue.AddToWaitingList(new Registration(1, 42));

            _queue.IsParticipantWaiting(42).Should().BeTrue();
        }

        [Fact]
        public void IsParticipantWaiting_ParticipantNotInQueue_ReturnsFalse()
        {
            _queue.IsParticipantWaiting(99).Should().BeFalse();
        }

        [Fact]
        public void GetWaitingPosition_FirstInQueue_ReturnsOne()
        {
            _queue.AddToWaitingList(new Registration(1, 10));
            _queue.AddToWaitingList(new Registration(1, 20));

            _queue.GetWaitingPosition(10).Should().Be(1);
        }

        [Fact]
        public void GetWaitingPosition_SecondInQueue_ReturnsTwo()
        {
            _queue.AddToWaitingList(new Registration(1, 10));
            _queue.AddToWaitingList(new Registration(1, 20));

            _queue.GetWaitingPosition(20).Should().Be(2);
        }

        [Fact]
        public void GetWaitingPosition_NotInQueue_ReturnsNegativeOne()
        {
            _queue.GetWaitingPosition(999).Should().Be(-1);
        }

        [Fact]
        public void ProcessAll_ReturnsAllInFIFOOrder_QueueEmpty()
        {
            _queue.AddToWaitingList(new Registration(1, 1));
            _queue.AddToWaitingList(new Registration(1, 2));
            _queue.AddToWaitingList(new Registration(1, 3));

            var all = _queue.ProcessAll().ToList();

            all.Should().HaveCount(3);
            all[0].ParticipantId.Should().Be(1); // FIFO
            all[1].ParticipantId.Should().Be(2);
            all[2].ParticipantId.Should().Be(3);
            _queue.WaitingCount.Should().Be(0);
        }

        [Fact]
        public void AddToWaitingList_NullRegistration_ThrowsArgumentNullException()
        {
            Action act = () => _queue.AddToWaitingList(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_InvalidEventId_ThrowsArgumentException()
        {
            Action act = () => new RegistrationQueue(0, "Test");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_EmptyEventName_ThrowsArgumentException()
        {
            Action act = () => new RegistrationQueue(1, "");
            act.Should().Throw<ArgumentException>();
        }
    }
}