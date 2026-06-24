using System.ComponentModel.DataAnnotations;
using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Enums;

namespace CommunityEventManagement.Web.ViewModels
{
    /// <summary>
    /// Read-only display model for a Registration.
    /// </summary>
    public class RegistrationViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string ParticipantEmail { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public RegistrationStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string StatusColour => Status switch
        {
            RegistrationStatus.Confirmed  => "success",
            RegistrationStatus.Pending    => "warning",
            RegistrationStatus.Cancelled  => "danger",
            RegistrationStatus.Waitlisted => "info",
            _ => "secondary"
        };

        public static RegistrationViewModel FromEntity(Registration r)
        {
            return new RegistrationViewModel
            {
                Id = r.Id,
                EventId = r.EventId,
                EventName = r.Event?.Name ?? $"Event {r.EventId}",
                EventStartDate = r.Event?.StartDate ?? DateTime.MinValue,
                ParticipantId = r.ParticipantId,
                ParticipantName = r.Participant?.GetFullName()
                    ?? $"Participant {r.ParticipantId}",
                ParticipantEmail = r.Participant?.Email ?? string.Empty,
                RegistrationDate = r.RegistrationDate,
                Status = r.Status,
                StatusName = r.Status.ToString(),
                Notes = r.Notes
            };
        }
    }

    /// <summary>
    /// Form model for registering a participant for an event.
    /// </summary>
    public class RegisterForEventModel
    {
        [Required(ErrorMessage = "Please select an event")]
        [Display(Name = "Event")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Please select a participant")]
        [Display(Name = "Participant")]
        public int ParticipantId { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Notes (optional)")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Form model for cancelling a registration.
    /// </summary>
    public class CancelRegistrationModel
    {
        public int RegistrationId { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        [Display(Name = "Reason for cancellation (optional)")]
        public string? Reason { get; set; }
    }
}