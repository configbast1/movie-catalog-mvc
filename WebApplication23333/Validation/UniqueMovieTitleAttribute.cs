using System.ComponentModel.DataAnnotations;
using WebApplication23333.Models;
using WebApplication23333.Services;

namespace WebApplication23333.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class UniqueMovieTitleAttribute : ValidationAttribute
{
    public UniqueMovieTitleAttribute()
        : base("Фільм з назвою «{0}» вже є в каталозі.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string title || string.IsNullOrWhiteSpace(title))
        {
            return ValidationResult.Success;
        }

        if (validationContext.GetService(typeof(IMovieService)) is not IMovieService movieService)
        {
            return ValidationResult.Success;
        }

        var currentId = (validationContext.ObjectInstance as Movie)?.Id ?? 0;
        var normalized = title.Trim();

        if (movieService.IsTitleUnique(normalized, currentId))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            string.Format(ErrorMessageString, normalized),
            validationContext.MemberName is null ? null : new[] { validationContext.MemberName });
    }
}
