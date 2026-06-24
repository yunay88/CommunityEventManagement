using System.ComponentModel.DataAnnotations;
using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces.Domain;

namespace CommunityEventManagement.Domain.Entities
{
    /// <summary>
    /// Venue entity — where events take place.
    /// Inheritance: Venue → BaseEntity
    /// Implements: ILocatable, ICapacityManaged (multiple interfaces)
    /// 
    /// Demonstrates:
    ///   - Multiple interface implementation
    ///   - Method overloading: HasAvailableSpace() has two versions
    ///   - Encapsulation: capacity managed through controlled methods
    /// </summary>
    public class Venue : BaseEntity, ILocatable, ICapacityManaged
    {
        [Required(ErrorMessage = "Venue name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Venue name must be between 2 and 100 characters")]
        public string Name { get; private set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; private set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; private set; } = string.Empty;

        [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100,000")]
        public int MaxCapacity { get; private set; }

        public int CurrentCapacity { get; private set; }

        // Navigation property — events held at this venue
        public ICollection<Event> Events { get; private set; } = new List<Event>();

        // EF Core constructor
        private Venue() { }

        public Venue(string name, string address, string city, int maxCapacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Venue name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty.", nameof(address));
            if (maxCapacity <= 0)
                throw new VenueCapacityException(
                    $"Venue capacity must be greater than zero. Provided: {maxCapacity}");

            Name = name.Trim();
            Address = address.Trim();
            City = city.Trim();
            MaxCapacity = maxCapacity;
            CurrentCapacity = maxCapacity;
        }

        // ILocatable implementation
        public string GetLocation() => $"{Name}, {Address}, {City}";

        // ICapacityManaged implementation
        // METHOD OVERLOADING — Version 1: checks if ANY space available
        public bool HasAvailableSpace() => CurrentCapacity > 0;

        // METHOD OVERLOADING — Version 2: checks if SPECIFIC spaces available
        public bool HasAvailableSpace(int requiredSpaces) => CurrentCapacity >= requiredSpaces;

        public decimal GetOccupancyRate()
        {
            if (MaxCapacity == 0) return 0;
            return Math.Round((decimal)(MaxCapacity - CurrentCapacity) / MaxCapacity * 100, 2);
        }

        // OVERRIDE abstract method — polymorphism
        public override string GetDisplayName() => Name;

        // OVERRIDE virtual method — polymorphism
        public override string GetSummary()
        {
            return $"Venue: {Name} | {Address}, {City} | " +
                   $"Capacity: {CurrentCapacity}/{MaxCapacity} | " +
                   $"Occupancy: {GetOccupancyRate()}%";
        }

        public void UpdateDetails(string name, string address, string city, int maxCapacity)
        {
            if (maxCapacity <= 0)
                throw new VenueCapacityException($"Capacity must be greater than zero.");

            Name = name.Trim();
            Address = address.Trim();
            City = city.Trim();
            MaxCapacity = maxCapacity;
            MarkAsUpdated();
        }

        public void ReserveSpace(int spaces = 1)
        {
            if (!HasAvailableSpace(spaces))
                throw new VenueCapacityException(Id, MaxCapacity);

            CurrentCapacity -= spaces;
            MarkAsUpdated();
        }

        public void ReleaseSpace(int spaces = 1)
        {
            CurrentCapacity = Math.Min(MaxCapacity, CurrentCapacity + spaces);
            MarkAsUpdated();
        }
    }
}