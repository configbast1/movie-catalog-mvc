using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Kino.PL.Validation;

/// <summary>
/// Кастомний атрибут для завантаження постера: перевіряє розширення та розмір файлу.
/// На клієнті читає обраний файл через File API, на сервері — сам IFormFile.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class PosterFileAttribute : ValidationAttribute, IClientModelValidator
{
    public string[] Extensions { get; init; } = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    public int MaxMegabytes { get; init; } = 5;

    private long MaxBytes => MaxMegabytes * 1024L * 1024L;

    // "JPG, JPEG, PNG, WEBP, GIF" — підставляється в перекладений текст помилки
    private string FormatsList => string.Join(", ", Extensions.Select(e => e.TrimStart('.').ToUpperInvariant()));

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file || file.Length == 0)
        {
            return ValidationResult.Success; // файл не обов'язковий
        }

        var memberNames = validationContext.MemberName is null
            ? null
            : new[] { validationContext.MemberName };

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!Extensions.Contains(extension))
        {
            return new ValidationResult(
                validationContext.Localize("Validation.PosterExtensions", FormatsList),
                memberNames);
        }

        if (file.Length > MaxBytes)
        {
            return new ValidationResult(
                validationContext.Localize("Validation.PosterSize", MaxMegabytes),
                memberNames);
        }

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        var message = context.Localize("Validation.PosterClient", FormatsList, MaxMegabytes);

        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val", "true");
        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val-posterfile", message);
        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val-posterfile-extensions", string.Join("|", Extensions));
        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val-posterfile-maxbytes", MaxBytes.ToString());
    }
}
