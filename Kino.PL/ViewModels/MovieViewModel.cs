using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Kino.PL.Validation;

namespace Kino.PL.ViewModels;

// модель рівня представлення: усе, що стосується форми та валідації вводу.
// саме вона живе у в'юхах, а не сутність з DAL.
// у Display/ErrorMessage стоять не тексти, а ключі з Resources/SharedResource.{culture}.json —
// їх перекладає AddDataAnnotationsLocalization
public class MovieViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Validation.TitleRequired")]
    [Display(Name = "Fields.Title")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Validation.TitleLength")]
    [ForbiddenWords]
    [UniqueMovieTitle]
    [Remote(action: "CheckTitle", controller: "Movies", AdditionalFields = nameof(Id),
        ErrorMessage = "Validation.TitleExists")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Fields.OriginalTitle")]
    [StringLength(200, ErrorMessage = "Validation.MaxLength")]
    [ForbiddenWords]
    public string? OriginalTitle { get; set; }

    [Required(ErrorMessage = "Validation.DirectorRequired")]
    [Display(Name = "Fields.Director")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "Validation.DirectorLength")]
    // À-ÖØ-öø-ÿ — літери з діакритикою, щоб приймались французькі імена (Jean-Pierre Jeunet, Luc Besson, François Truffaut)
    [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿА-Яа-яЄЇІҐєїіґЁё'’.\- ]+$",
        ErrorMessage = "Validation.DirectorPattern")]
    public string Director { get; set; } = string.Empty;

    [Required(ErrorMessage = "Validation.GenreRequired")]
    [Display(Name = "Fields.Genre")]
    [StringLength(80, ErrorMessage = "Validation.MaxLength")]
    public string Genre { get; set; } = string.Empty;

    [Display(Name = "Fields.ReleaseYear")]
    [ReleaseYear]
    public int ReleaseYear { get; set; }

    [Display(Name = "Fields.PosterPath")]
    [StringLength(260, ErrorMessage = "Validation.MaxLength")]
    public string PosterPath { get; set; } = string.Empty;

    [Display(Name = "Fields.PosterFile")]
    [PosterFile(MaxMegabytes = 5)]
    public IFormFile? PosterFile { get; set; }

    [Required(ErrorMessage = "Validation.DescriptionRequired")]
    [Display(Name = "Fields.Description")]
    [StringLength(1000, MinimumLength = 20, ErrorMessage = "Validation.DescriptionLength")]
    [DataType(DataType.MultilineText)]
    [ForbiddenWords]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Fields.Rating")]
    [Range(0.0, 10.0, ErrorMessage = "Validation.RatingRange")]
    public decimal Rating { get; set; }

    // правила рівня всієї моделі: перевіряються після атрибутів окремих полів
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(OriginalTitle) &&
            string.Equals(OriginalTitle.Trim(), Title?.Trim(), StringComparison.CurrentCultureIgnoreCase))
        {
            yield return new ValidationResult(
                validationContext.Localize("Validation.OriginalDuplicates"),
                new[] { nameof(OriginalTitle) });
        }

        if (ReleaseYear > DateTime.Now.Year && Rating > 0)
        {
            yield return new ValidationResult(
                validationContext.Localize("Validation.RatingTooEarly", ReleaseYear));
        }
    }
}
