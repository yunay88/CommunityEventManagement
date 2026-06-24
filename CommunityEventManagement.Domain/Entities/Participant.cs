using CommunityEventManagement.Domain.Enums;
using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces.Domain;
using CommunityEventManagement.Domain.Models;

namespace CommunityEventManagement.Domain.Entities
{
    /// <summary>
    /// Participant — concrete entity at level 4 of inheritance hierarchy.
    /// Represents a user who participates in community events.
    /// 
    /// Inheritance chain: Participant → AppUser → Person → BaseEntity
    /// Implements: IFilterable (multiple interface implementation)
    /// 
    /// Demonstrates:
    ///   - 4-level deep inheritance chain
    ///   - Interface implementation alongside inheritance
    ///   - Polymorphism: GetDisplayName() and GetSummary() override
    ///   - Encapsulation: private collection with controlled access
    /// </summary>
    public class Participant : AppUser, IFilterable
    {
        // Private collection — encapsulated navigation property
        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.AsReadOnly();

        // EF Core needs this for navigation property
        public virtual ICollection<Registration> RegistrationsCollection
        {
            get => _registrations;
            set
            {
                _registrations.Clear();
                if (value != null) _registrations.AddRange(value);
            }
        }

        // EF Core parameterless constructor
        private Participant() { }

        // Public constructor — chain calls up the hierarchy
        public Participant(
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            string? phoneNumber = null)
            : base(firstName, lastName, email, passwordHash, UserRole.Participant, phoneNumber)
        {
        }

        // OVERRIDE abstract method from BaseEntity — POLYMORPHISM
        public override string GetDisplayName() => GetFullName();

        // OVERRIDE virtual method from BaseEntity — POLYMORPHISM
        public override string GetSummary()
        {
            return $"Participant: {GetFullName()} | " +
                   $"Email: {Email} | " +
                   $"Registrations: {_registrations.Count} | " +
                   $"Active: {IsActive}";
        }

        // OVERRIDE from AppUser — participants can register, not manage
        public override bool CanManageEvents() => false;
        public override bool CanRegisterForEvents() => true;

        // IFilterable implementation — Participant filters by name/email
        public bool MatchesFilter(FilterCriteria criteria)
        {
            if (string.IsNullOrWhiteSpace(criteria.SearchTerm)) return true;

            var term = criteria.SearchTerm.ToLowerInvariant();
            return GetFullName().ToLowerInvariant().Contains(term) ||
                   Email.ToLowerInvariant().Contains(term) ||
                   (PhoneNumber != null && PhoneNumber.Contains(term));
        }

        public void UpdateDetails(
            string firstName,
            string lastName,
            string email,
            string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            UpdateContactDetails(email, phoneNumber);
        }

        public int GetActiveRegistrationsCount()
        {
            return _registrations.Count(r => r.Status == RegistrationStatus.Confirmed);
        }
    }
}