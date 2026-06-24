using System.ComponentModel.DataAnnotations;
using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Web.ViewModels
{
    /// <summary>
    /// Read-only display model for a Venue.
    /// </summary>
    public class VenueViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public int CurrentCapacity { get; set; }
        public decimal OccupancyRate { get; set; }
        public bool HasAvailableSpace { get; set; }
        public int EventCount { get; set; }

        public static VenueViewModel FromEntity(Venue v)
        {
            return new VenueViewModel
            {
                Id = v.Id,
                Name = v.Name,
                Address = v.Address,
                City = v.City,
                Location = v.GetLocation(),
                MaxCapacity = v.MaxCapacity,
                CurrentCapacity = v.CurrentCapacity,
                OccupancyRate = v.GetOccupancyRate(),
                HasAvailableSpace = v.HasAvailableSpace(),
                EventCount = v.Events?.Count ?? 0
            };
        }
    }

    /// <summary>
    /// Form model for creating and editing a Venue.
    /// </summary>
    public class CreateVenueModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Venue name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Venue name must be between 2 and 100 characters")]
        [Display(Name = "Venue Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        [Display(Name = "Street Address")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 100000,
            ErrorMessage = "Capacity must be between 1 and 100,000")]
        [Display(Name = "Maximum Capacity")]
        public int MaxCapacity { get; set; } = 100;

        public static CreateVenueModel FromEntity(Venue v)
        {
            return new CreateVenueModel
            {
                Id = v.Id,
                Name = v.Name,
                Address = v.Address,
                City = v.City,
                MaxCapacity = v.MaxCapacity
            };
        }
    }
}