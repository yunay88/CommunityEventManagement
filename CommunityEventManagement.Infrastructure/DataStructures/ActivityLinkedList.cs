using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Infrastructure.DataStructures
{
    /// <summary>
    /// Activity Linked List — manages the ordered sequence of activities
    /// within a community event.
    /// 
    /// DATA STRUCTURE: LinkedList(T) — doubly linked list
    /// 
    /// Why LinkedList for activities?
    ///   - Activities in an event have a specific ORDER
    ///   - We need efficient INSERT at any position (add between two activities)
    ///   - We need efficient REMOVE from any position
    ///   - LinkedList O(1) insert/remove once position is found
    ///   - Array/List would require shifting elements: O(n)
    /// 
    /// Time Complexity:
    ///   - AddFirst / AddLast: O(1)
    ///   - InsertBefore / InsertAfter: O(1) once node found
    ///   - Remove: O(1) once node found
    ///   - Find by value: O(n)
    /// 
    /// Demonstrates:
    ///   - LinkedList(T) data structure
    ///   - LinkedListNode(T) navigation
    ///   - Bidirectional traversal (forward and backward)
    /// </summary>
    public class ActivityLinkedList
    {
        // The underlying LinkedList(T) data structure
        private readonly LinkedList<Activity> _activities = new();

        public int EventId { get; }
        public string EventName { get; }

        public ActivityLinkedList(int eventId, string eventName)
        {
            EventId = eventId;
            EventName = eventName;
        }

        /// <summary>
        /// Adds an activity to the END of the event schedule.
        /// LinkedList operation: AddLast — O(1)
        /// </summary>
        public void AddActivity(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            _activities.AddLast(activity);
        }

        /// <summary>
        /// Adds an activity to the BEGINNING of the event schedule.
        /// LinkedList operation: AddFirst — O(1)
        /// </summary>
        public void AddActivityFirst(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            _activities.AddFirst(activity);
        }

        /// <summary>
        /// Inserts an activity AFTER a specific existing activity.
        /// LinkedList operation: AddAfter — O(n) to find, O(1) to insert.
        /// This is more efficient than List(T) which requires element shifting.
        /// </summary>
        public bool InsertAfter(int existingActivityId, Activity newActivity)
        {
            if (newActivity == null)
                throw new ArgumentNullException(nameof(newActivity));

            var node = FindNode(existingActivityId);
            if (node == null) return false;

            _activities.AddAfter(node, newActivity);
            return true;
        }

        /// <summary>
        /// Inserts an activity BEFORE a specific existing activity.
        /// LinkedList operation: AddBefore — O(n) to find, O(1) to insert.
        /// </summary>
        public bool InsertBefore(int existingActivityId, Activity newActivity)
        {
            if (newActivity == null)
                throw new ArgumentNullException(nameof(newActivity));

            var node = FindNode(existingActivityId);
            if (node == null) return false;

            _activities.AddBefore(node, newActivity);
            return true;
        }

        /// <summary>
        /// Removes a specific activity from the schedule.
        /// LinkedList operation: Remove — O(n) to find, O(1) to remove.
        /// </summary>
        public bool RemoveActivity(int activityId)
        {
            var node = FindNode(activityId);
            if (node == null) return false;

            _activities.Remove(node);
            return true;
        }

        /// <summary>
        /// Moves an activity one position forward in the schedule.
        /// Demonstrates: LinkedListNode navigation with Previous/Next.
        /// </summary>
        public bool MoveActivityForward(int activityId)
        {
            var node = FindNode(activityId);
            if (node == null || node.Previous == null) return false;

            var activity = node.Value;
            _activities.Remove(node);
            _activities.AddBefore(node.Previous, activity);
            return true;
        }

        /// <summary>
        /// Moves an activity one position backward in the schedule.
        /// </summary>
        public bool MoveActivityBackward(int activityId)
        {
            var node = FindNode(activityId);
            if (node == null || node.Next == null) return false;

            var activity = node.Value;
            var nextNode = node.Next;
            _activities.Remove(node);
            _activities.AddAfter(nextNode, activity);
            return true;
        }

        /// <summary>
        /// Returns activities in schedule order (forward traversal).
        /// LinkedList forward traversal: O(n)
        /// </summary>
        public IEnumerable<Activity> GetActivitiesInOrder()
        {
            var current = _activities.First;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        /// <summary>
        /// Returns activities in REVERSE order (backward traversal).
        /// Demonstrates: LinkedList bidirectional traversal using Previous.
        /// This is efficient with LinkedList — would need reversal with List(T).
        /// </summary>
        public IEnumerable<Activity> GetActivitiesInReverseOrder()
        {
            var current = _activities.Last;
            while (current != null)
            {
                yield return current.Value;
                current = current.Previous;
            }
        }

        /// <summary>
        /// Returns the first activity (opening activity).
        /// LinkedList operation: First — O(1)
        /// </summary>
        public Activity? GetFirstActivity() => _activities.First?.Value;

        /// <summary>
        /// Returns the last activity (closing activity).
        /// LinkedList operation: Last — O(1)
        /// </summary>
        public Activity? GetLastActivity() => _activities.Last?.Value;

        /// <summary>
        /// Returns the activity AFTER a given activity.
        /// Demonstrates: LinkedListNode.Next navigation.
        /// </summary>
        public Activity? GetNextActivity(int activityId)
        {
            var node = FindNode(activityId);
            return node?.Next?.Value;
        }

        /// <summary>
        /// Returns the activity BEFORE a given activity.
        /// Demonstrates: LinkedListNode.Previous navigation.
        /// </summary>
        public Activity? GetPreviousActivity(int activityId)
        {
            var node = FindNode(activityId);
            return node?.Previous?.Value;
        }

        // Find a node by activity ID — O(n)
        private LinkedListNode<Activity>? FindNode(int activityId)
        {
            var current = _activities.First;
            while (current != null)
            {
                if (current.Value.Id == activityId)
                    return current;
                current = current.Next;
            }
            return null;
        }

        public int Count => _activities.Count;
        public bool IsEmpty => _activities.Count == 0;

        public override string ToString()
        {
            var activities = string.Join(" → ",
                GetActivitiesInOrder().Select(a => a.Name));
            return $"Schedule for '{EventName}': {activities}";
        }
    }
}