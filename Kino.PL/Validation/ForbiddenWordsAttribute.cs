using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Kino.PL.Validation;

/// <summary>
/// Кастомний атрибут: текст не повинен містити службових слів-заглушок
/// (їх часто лишають випадково при заповненні форми).
/// Працює і на сервері, і на клієнті.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class ForbiddenWordsAttribute : ValidationAttribute, IClientModelValidator
{
    private static readonly string[] DefaultWords = { "lorem", "ipsum", "asdf", "qwerty", "test", "тест", "хз" };

    public string[] Words { get; init; } = DefaultWords;

    // ключ перекладу в Resources/SharedResource.{culture}.json
    private const string MessageKey = "Validation.ForbiddenWords";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return ValidationResult.Success;
        }

        var found = Words.FirstOrDefault(word =>
            text.Contains(word, StringComparison.CurrentCultureIgnoreCase));

        if (found is null)
        {
            return ValidationResult.Success;
        }

        var message = validationContext.Localize(MessageKey, validationContext.DisplayName, found);

        return new ValidationResult(
            message,
            validationContext.MemberName is null ? null : new[] { validationContext.MemberName });
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        var displayName = context.ModelMetadata.GetDisplayName();
        var message = context.Localize(MessageKey, displayName, string.Join(", ", Words));

        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val", "true");
        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val-forbiddenwords", message);
        ValidationAttributeHelpers.MergeAttribute(context.Attributes, "data-val-forbiddenwords-words", string.Join("|", Words));
    }
}
