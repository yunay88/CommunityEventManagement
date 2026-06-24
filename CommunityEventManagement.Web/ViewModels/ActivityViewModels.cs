using System.ComponentModel.DataAnnotations;
using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Enums;

namespace CommunityEventManagement.Web.ViewModels
{
    /// <summary>
    /// Read-only display model for an Activity.
    /// </summary>
    public class ActivityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ActivityType Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string DurationFormatted { get; set; } = string.Empty;

        public static ActivityViewModel FromEntity(Activity a)
        {
            return new ActivityViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Type = a.Type,
                TypeName = a.Type.ToString(),
                DurationMinutes = a.DurationMinutes,
                DurationFormatted = a.GetFormattedDuration()
            };
        }
    }

    /// <summary>
    /// Form model for creating and editing an Activity.
    /// </summary>
    public class CreateActivityModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Activity name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Activity name must be between 2 and 100 characters")]
        [Display(Name = "Activity Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Activity type is required")]
        [Display(Name = "Activity Type")]
        public ActivityType Type { get; set; } = ActivityType.Workshop;

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 480,
            ErrorMessage = "Duration must be between 1 and 480 minutes")]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; } = 60;

        public static CreateActivityModel FromEntity(Activity a)
        {
            return new CreateActivityModel
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Type = a.Type,
                DurationMinutes = a.DurationMinutes
            };
        }
    }
}