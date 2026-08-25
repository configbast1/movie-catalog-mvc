using Microsoft.EntityFrameworkCore;
using WebApplication23333.Models;

namespace WebApplication23333.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("Movies");
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
            entity.Property(m => m.OriginalTitle).HasMaxLength(200);
            entity.Property(m => m.Director).IsRequired().HasMaxLength(120);
            entity.Property(m => m.Genre).IsRequired().HasMaxLength(80);
            entity.Property(m => m.PosterPath).IsRequired().HasMaxLength(260);
            entity.Property(m => m.Description).IsRequired().HasMaxLength(1000);
            entity.Property(m => m.Rating).HasColumnType("decimal(3,1)");

            entity.HasIndex(m => m.Rating);
        });

        modelBuilder.Entity<Movie>().HasData(SeedMovies);
    }

    public static readonly Movie[] SeedMovies =
    {
        new Movie
        {
            Id = 1,
            Title = "Втеча з Шоушенка",
            OriginalTitle = "The Shawshank Redemption",
            Director = "Френк Дарабонт",
            Genre = "Драма",
            ReleaseYear = 1994,
            PosterPath = "/images/posters/shawshank.jpg",
            Description = "Банкір Енді Дюфрейн отримує довічне ув'язнення за вбивство, якого не скоював. За роки у Шоушенку він знаходить друга і не втрачає головного — надії.",
            Rating = 9.3m
        },
        new Movie
        {
            Id = 2,
            Title = "Хрещений батько",
            OriginalTitle = "The Godfather",
            Director = "Френсіс Форд Коппола",
            Genre = "Кримінальна драма",
            ReleaseYear = 1972,
            PosterPath = "/images/posters/godfather.jpg",
            Description = "Історія родини Корлеоне: старіючий дон передає владу над імперією синові, який не хотів мати з нею нічого спільного.",
            Rating = 9.2m
        },
        new Movie
        {
            Id = 3,
            Title = "Володар перснів: Повернення короля",
            OriginalTitle = "The Lord of the Rings: The Return of the King",
            Director = "Пітер Джексон",
            Genre = "Фентезі, пригоди",
            ReleaseYear = 2003,
            PosterPath = "/images/posters/lotr-rotk.jpg",
            Description = "Фродо і Сем завершують шлях до Мордору, поки армії Заходу дають останній бій, аби відвернути погляд Саурона від Персня.",
            Rating = 9.0m
        },
        new Movie
        {
            Id = 4,
            Title = "Темний лицар",
            OriginalTitle = "The Dark Knight",
            Director = "Крістофер Нолан",
            Genre = "Бойовик, трилер",
            ReleaseYear = 2008,
            PosterPath = "/images/posters/dark-knight.jpg",
            Description = "Бетмен протистоїть Джокеру — злочинцю без мотиву й правил, який хоче довести, що будь-яка людина зламається.",
            Rating = 9.0m
        },
        new Movie
        {
            Id = 5,
            Title = "Кримінальне чтиво",
            OriginalTitle = "Pulp Fiction",
            Director = "Квентін Тарантіно",
            Genre = "Кримінальна комедія",
            ReleaseYear = 1994,
            PosterPath = "/images/posters/pulp-fiction.jpg",
            Description = "Кілька історій із життя лос-анджелеських бандитів, боксера й пари грабіжників, розказані у нелінійному порядку.",
            Rating = 8.9m
        },
        new Movie
        {
            Id = 6,
            Title = "Форрест Ґамп",
            OriginalTitle = "Forrest Gump",
            Director = "Роберт Земекіс",
            Genre = "Драма, комедія",
            ReleaseYear = 1994,
            PosterPath = "/images/posters/forrest-gump.jpg",
            Description = "Простодушний чоловік з Алабами випадково опиняється у центрі ключових подій американської історії, зберігаючи вірність одному коханню.",
            Rating = 8.8m
        },
        new Movie
        {
            Id = 7,
            Title = "Початок",
            OriginalTitle = "Inception",
            Director = "Крістофер Нолан",
            Genre = "Наукова фантастика, трилер",
            ReleaseYear = 2010,
            PosterPath = "/images/posters/inception.jpg",
            Description = "Команда «викрадачів снів» отримує зворотне завдання — не вкрасти ідею, а непомітно посадити її у чужу підсвідомість.",
            Rating = 8.8m
        },
        new Movie
        {
            Id = 8,
            Title = "Інтерстеллар",
            OriginalTitle = "Interstellar",
            Director = "Крістофер Нолан",
            Genre = "Наукова фантастика, драма",
            ReleaseYear = 2014,
            PosterPath = "/images/posters/interstellar.jpg",
            Description = "Земля вмирає, і колишній пілот вирушає крізь червоточину шукати нову домівку для людства, жертвуючи роками життя своїх дітей.",
            Rating = 8.7m
        },
        new Movie
        {
            Id = 9,
            Title = "Матриця",
            OriginalTitle = "The Matrix",
            Director = "Лана та Ліллі Вачовскі",
            Genre = "Наукова фантастика",
            ReleaseYear = 1999,
            PosterPath = "/images/posters/matrix.jpg",
            Description = "Програміст Томас Андерсон дізнається, що звичний світ — симуляція, і приєднується до повстання проти машин.",
            Rating = 8.7m
        },
        new Movie
        {
            Id = 10,
            Title = "Паразити",
            OriginalTitle = "기생충",
            Director = "Пон Джун Хо",
            Genre = "Трилер, драма",
            ReleaseYear = 2019,
            PosterPath = "/images/posters/parasite.jpg",
            Description = "Бідна родина Кімів крок за кроком влаштовується на роботу до багатого дому Паків — доки підвал не відкриває свою таємницю.",
            Rating = 8.5m
        }
    };
}
