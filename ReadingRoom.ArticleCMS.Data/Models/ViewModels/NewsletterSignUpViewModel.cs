using System.ComponentModel.DataAnnotations;

namespace ReadingRoom.ArticleCMS.Data.Models.ViewModels;

/// <summary>
/// The data posted by the newsletter sign-up form.
/// </summary>
public sealed class NewsletterSignUpViewModel
{
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;
}
