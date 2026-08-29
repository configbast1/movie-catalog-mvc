using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using WebApplication23333.Validation;

namespace WebApplication23333.Models;

// IValidatableObject дозволяє описати правила, які залежать одразу від кількох полів
public class Movie : IValidatableObject
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Вкажіть назву фільму")]
    [Display(Name = "Назва фільму")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Назва має містити від {2} до {1} символів")]
    [ForbiddenWords]                                    // кастомний атрибут
    [UniqueMovieTitle]                                  // кастомний атрибут (перевірка по базі)
    [Remote(action: "CheckTitle", controller: "Movies", AdditionalFields = nameof(Id),
        ErrorMessage = "Фільм з такою назвою вже є в каталозі")]   // та сама перевірка на клієнті
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Оригінальна назва")]
    [StringLength(200, ErrorMessage = "Максимум {1} символів")]
    [ForbiddenWords]
    public string? OriginalTitle { get; set; }

    [Required(ErrorMessage = "Вкажіть режисера")]
    [Display(Name = "Режисер")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "Ім'я режисера має містити від {2} до {1} символів")]
    [RegularExpression(@"^[A-Za-zА-Яа-яЄЇІҐєїіґЁё'’.\- ]+$",
        ErrorMessage = "Ім'я режисера може містити лише літери, пробіли, апостроф, крапку та дефіс")]
    public string Director { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть жанр")]
    [Display(Name = "Жанр")]
    [StringLength(80, ErrorMessage = "Максимум {1} символів")]
    public string Genre { get; set; } = string.Empty;

    [Display(Name = "Рік випуску")]
    [ReleaseYear]                                       // кастомний атрибут замість звичайного Range
    public int ReleaseYear { get; set; }

    [Required(ErrorMessage = "Не вказано шлях до постера")]
    [Display(Name = "Постер")]
    [StringLength(260, ErrorMessage = "Максимум {1} символів")]
    public string PosterPath { get; set; } = string.Empty;

    // файл, який користувач обирає у формі; у базі не зберігається
    [NotMapped]
    [Display(Name = "Постер (файл із пристрою)")]
    [PosterFile(MaxMegabytes = 5)]                      // кастомний атрибут
    public IFormFile? PosterFile { get; set; }

    [Required(ErrorMessage = "Додайте короткий опис")]
    [Display(Name = "Короткий опис")]
    [StringLength(1000, MinimumLength = 20, ErrorMessage = "Опис має містити від {2} до {1} символів")]
    [DataType(DataType.MultilineText)]
    [ForbiddenWords]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Оцінка")]
    [Range(0.0, 10.0, ErrorMessage = "Оцінка має бути від {1} до {2}")]
    public decimal Rating { get; set; }

    // правила рівня всієї моделі: перевіряються після атрибутів окремих полів
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(OriginalTitle) &&
            string.Equals(OriginalTitle.Trim(), Title?.Trim(), StringComparison.CurrentCultureIgnoreCase))
        {
            yield return new ValidationResult(
                "Оригінальна назва дублює основну — залиште поле порожнім або введіть назву мовою оригіналу.",
                new[] { nameof(OriginalTitle) });
        }

        if (ReleaseYear > DateTime.Now.Year && Rating > 0)
        {
            // помилка без імені поля потрапляє у загальний блок (Validation Summary)
            yield return new ValidationResult(
                $"Фільм {ReleaseYear} року ще не вийшов у прокат — виставляти оцінку зарано.");
        }
    }
}
