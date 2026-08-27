using System.ComponentModel.DataAnnotations;

namespace WebApplication23333.Models;

public class Movie
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Вкажіть назву фільму")]
    [Display(Name = "Назва фільму")]
    [StringLength(200, ErrorMessage = "Максимум {1} символів")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Оригінальна назва")]
    [StringLength(200, ErrorMessage = "Максимум {1} символів")]
    public string? OriginalTitle { get; set; }

    [Required(ErrorMessage = "Вкажіть режисера")]
    [Display(Name = "Режисер")]
    [StringLength(120, ErrorMessage = "Максимум {1} символів")]
    public string Director { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть жанр")]
    [Display(Name = "Жанр")]
    [StringLength(80, ErrorMessage = "Максимум {1} символів")]
    public string Genre { get; set; } = string.Empty;

    [Display(Name = "Рік випуску")]
    [Range(1888, 2100, ErrorMessage = "Рік має бути від {1} до {2}")]
    public int ReleaseYear { get; set; }

    [Required(ErrorMessage = "Не вказано шлях до постера")]
    [Display(Name = "Постер")]
    [StringLength(260, ErrorMessage = "Максимум {1} символів")]
    public string PosterPath { get; set; } = string.Empty;

    [Required(ErrorMessage = "Додайте короткий опис")]
    [Display(Name = "Короткий опис")]
    [StringLength(1000, ErrorMessage = "Максимум {1} символів")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Оцінка")]
    [Range(0.0, 10.0, ErrorMessage = "Оцінка має бути від {1} до {2}")]
    public decimal Rating { get; set; }
}
