using System.ComponentModel.DataAnnotations;
using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Web.ViewModels
{
    /// <summary>
    /// Read-only display model for a Participant.
    /// </summary>
    public class ParticipantViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int ActiveRegistrations { get; set; }
        public DateTime CreatedAt { get; set; }

        public static ParticipantViewModel FromEntity(Participant p)
        {
            return new ParticipantViewModel
            {
                Id = p.Id,
                FullName = p.GetFullName(),
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                ActiveRegistrations = p.GetActiveRegistrationsCount(),
                CreatedAt = p.CreatedAt
            };
        }
    }

    /// <summary>
    /// Form model for creating a Participant.
    /// DataAnnotations provide client-side HTML5 and server-side validation.
    /// </summary>
    public class CreateParticipantModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "First name must be between 2 and 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Last name must be between 2 and 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone Number (optional)")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare(nameof(Password),
            ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public static CreateParticipantModel FromEntity(Participant p)
        {
            return new CreateParticipantModel
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber
            };
        }
    }

    /// <summary>
    /// Edit model — no password fields (separate flow for password change).
    /// </summary>
    public class EditParticipantModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public static EditParticipantModel FromEntity(Participant p)
        {
            return new EditParticipantModel
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber
            };
        }
    }
}