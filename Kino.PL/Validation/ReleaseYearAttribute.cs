using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Kino.PL.Validation;

/// <summary>
/// Кастомний атрибут: рік випуску має бути від 1888 (перший фільм в історії)
/// і не більше ніж на рік уперед від поточного.
/// IClientModelValidator дає ту саму перевірку на клієнті без перезавантаження сторінки.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ReleaseYearAttribute : ValidationAttribute, IClientModelValidator
{
    public int MinYear { get; init; } = 1888;

    // на скільки років уперед дозволено анонси
    public int YearsAhead { get; init; } = 1;

    public int MaxYear => DateTime.Now.Year + YearsAhead;

    public ReleaseYearAttribute()
        : base("Рік випуску має бути в межах {1}–{2}.")
    {
    }

    public override string FormatErrorMessage(string name)
        => string.Format(ErrorMessageString, name, MinYear, MaxYear);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not int year || year < MinYear || year > MaxYear)
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                validationContext.MemberName is null ? null : new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }

    // формує data-val-* атрибути, які читає jquery.validate.unobtrusive
    public void AddValidation(ClientModelValidationContext context)
    {
        var message = FormatErrorMessage(context.ModelMetadata.GetDisplayName());

        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val", "true");
        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val-releaseyear", message);
        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val-releaseyear-min", MinYear.ToString());
        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val-releaseyear-max", MaxYear.ToString());
    }
}
