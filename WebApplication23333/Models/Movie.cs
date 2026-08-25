using System.ComponentModel.DataAnnotations;

namespace WebApplication23333.Models;

public class Movie
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Назва фільму")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Оригінальна назва")]
    [StringLength(200)]
    public string? OriginalTitle { get; set; }

    [Required]
    [Display(Name = "Режисер")]
    [StringLength(120)]
    public string Director { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Жанр")]
    [StringLength(80)]
    public string Genre { get; set; } = string.Empty;

    [Display(Name = "Рік випуску")]
    [Range(1888, 2100)]
    public int ReleaseYear { get; set; }

    [Required]
    [Display(Name = "Постер")]
    [StringLength(260)]
    public string PosterPath { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Короткий опис")]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Оцінка")]
    [Range(0, 10)]
    public decimal Rating { get; set; }
}
