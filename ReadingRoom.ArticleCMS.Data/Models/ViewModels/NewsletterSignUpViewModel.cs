using System.ComponentModel.DataAnnotations;
using ReadingRoom.ArticleCMS.Core.Constants;
using ReadingRoom.ArticleCMS.Core.Newsletter;

namespace ReadingRoom.ArticleCMS.Data.Models.ViewModels;

/// <summary>
/// The data posted by the newsletter sign-up form.
/// </summary>
public sealed class NewsletterSignUpViewModel
{
    [Required(ErrorMessage = DictionaryKeys.NewsletterEmailRequired)]
    [EmailAddress(ErrorMessage = DictionaryKeys.NewsletterEmailInvalid)]
    [StringLength(EmailAddressText.MaximumLength, ErrorMessage = DictionaryKeys.NewsletterEmailInvalid)]
    public string Email { get; set; } = string.Empty;
}
