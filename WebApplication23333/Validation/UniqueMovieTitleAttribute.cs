using System.ComponentModel.DataAnnotations;
using WebApplication23333.Data;
using WebApplication23333.Models;

namespace WebApplication23333.Validation;

/// <summary>
/// Кастомний атрибут: назва фільму має бути унікальною в каталозі.
/// Атрибут звертається до бази через сервіс-провайдер, тому перевірка можлива
/// лише на сервері; на клієнті ту саму перевірку виконує [Remote] через AJAX.
/// </summary>
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

        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context)
        {
            return ValidationResult.Success; // поза HTTP-запитом перевірити нічим
        }

        var currentId = (validationContext.ObjectInstance as Movie)?.Id ?? 0;
        var normalized = title.Trim();

        var duplicateExists = context.Movies
            .Any(m => m.Id != currentId && m.Title == normalized);

        if (!duplicateExists)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            string.Format(ErrorMessageString, normalized),
            validationContext.MemberName is null ? null : new[] { validationContext.MemberName });
    }
}
