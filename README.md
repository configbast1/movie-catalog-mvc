# 🎬 Кінокаталог — ASP.NET Core MVC + SQL Server

Навчальний веб-додаток: на стартовій сторінці виводиться **10 найкращих фільмів**
у вигляді адаптивної сітки з постерами. Усі дані зберігаються в **локальній базі
даних SQL Server (LocalDB)** і зчитуються через **Entity Framework Core**.

**Стек:** ASP.NET Core MVC (.NET 10) · Entity Framework Core 10 · SQL Server LocalDB · Razor · CSS Grid · Dependency Injection

---

## Скріншоти

### Головна сторінка — сітка Топ-10

![Головна сторінка](screenshots/01-home-top.png)

### Уся сітка з 10 фільмів

![Уся сітка](screenshots/02-home-full.png)

### Сторінка фільму

![Сторінка фільму](screenshots/03-details.png)

### Мобільна версія

<img src="screenshots/04-home-mobile.png" width="360" alt="Мобільна версія" />

---

## Структура даних — таблиця `dbo.Movies`

| Поле | Тип у БД | Опис |
| --- | --- | --- |
| `Id` | `int` (PK, identity) | Первинний ключ |
| `Title` | `nvarchar(200)` | **Назва фільму** |
| `OriginalTitle` | `nvarchar(200)`, null | Оригінальна назва |
| `Director` | `nvarchar(120)` | **Режисер** |
| `Genre` | `nvarchar(80)` | **Жанр** |
| `ReleaseYear` | `int` | **Рік випуску** |
| `PosterPath` | `nvarchar(260)` | **Постер** — відносний шлях до файлу у `wwwroot/images/posters` або `wwwroot/images/uploads` |
| `Description` | `nvarchar(1000)` | **Короткий опис** |
| `Rating` | `decimal(3,1)` | Оцінка 0–10, за нею сортується Топ-10 |

Постери зберігаються файлами у `wwwroot`, а в базі — лише шлях до них
(щоб не роздувати БД бінарними даними).

---

## Структура репозиторію

```
WebApplication23333/
├── WebApplication23333.slnx
├── README.md
├── .gitignore
├── screenshots/                    # скріншоти інтерфейсу
└── WebApplication23333/
    ├── Program.cs                  # тонкий: лише виклики extension-методів
    ├── appsettings.json            # рядок підключення DefaultConnection
    ├── Extensions/                 # extension-методи (реєстрація в DI + конвеєр)
    │   ├── ServiceCollectionExtensions.cs   # AddApplicationDatabase / AddApplicationServices
    │   └── WebApplicationExtensions.cs      # EnsureDatabaseCreated / UseApplicationLocalization / UseApplicationErrorHandling
    ├── Repositories/              # шар доступу до даних
    │   ├── IRepository.cs          # узагальнений контракт репозиторію
    │   └── Repository.cs           # реалізація над AppDbContext (єдине місце з EF Core)
    ├── Services/                   # сервісний шар — уся бізнес-логіка
    │   ├── IMovieService.cs        # контракт: CRUD + перевірка унікальності назви
    │   ├── MovieService.cs         # реалізація: IRepository<Movie> + IPosterStorage
    │   ├── IPosterStorage.cs       # контракт роботи з файлами постерів
    │   └── PosterStorage.cs        # збереження / видалення файлів у wwwroot
    ├── Controllers/
    │   ├── MoviesController.cs     # тонкий контролер: працює лише через IMovieService
    │   └── HomeController.cs       # редірект на каталог, обробка помилок і 404
    ├── Data/AppDbContext.cs        # DbContext + конфігурація + seed 10 фільмів
    ├── Models/Movie.cs             # Data Annotations + IValidatableObject
    ├── Validation/                 # власні (кастомні) атрибути валідації
    │   ├── ReleaseYearAttribute.cs
    │   ├── ForbiddenWordsAttribute.cs
    │   ├── PosterFileAttribute.cs
    │   └── UniqueMovieTitleAttribute.cs
    ├── Views/
    │   ├── Movies/Index.cshtml     # сітка карток + кнопки дій
    │   ├── Movies/Details.cshtml   # сторінка фільму
    │   ├── Movies/Create.cshtml    # форма додавання
    │   ├── Movies/Edit.cshtml      # форма редагування
    │   ├── Movies/Delete.cshtml    # підтвердження видалення
    │   └── Shared/
    │       ├── _Layout.cshtml
    │       ├── _Message.cshtml     # повідомлення про успішну дію
    │       ├── NotFound.cshtml     # власна сторінка 404
    │       └── Error.cshtml
    └── wwwroot/
        ├── css/site.css            # темна тема, адаптивна сітка, форми, стилі помилок
        ├── js/validation.js        # адаптери кастомних атрибутів для jQuery Validate
        ├── images/posters/*.jpg    # 10 стартових постерів
        └── images/uploads/         # постери, завантажені через веб-інтерфейс
```

---

## Як запустити

### Вимоги

- .NET SDK 10.0 (проєкт націлений на net10.0)
- SQL Server LocalDB (входить до Visual Studio 2022, компонент
  «ASP.NET and web development») або SQL Server Express

### Visual Studio 2022

1. Відкрити `WebApplication23333.slnx`
2. Натиснути **F5**

### Командний рядок

```bash
cd WebApplication23333
dotnet restore
dotnet run
```

Відкрити <https://localhost:7080> (або адресу з консолі).

База даних **створюється автоматично** при першому запуску разом із
початковими даними (`Database.EnsureCreated()` у extension-методі
`EnsureDatabaseCreated()`, файл `Extensions/WebApplicationExtensions.cs`).

> ⚠️ `EnsureCreated()` нічого не робить, якщо база `MovieCatalogDb` уже існує.
> Якщо ви змінили модель `Movie` — видаліть базу (SQL Server Object Explorer →
> ПКМ на `MovieCatalogDb` → Delete, або `sqllocaldb`) і запустіть додаток знову,
> або перейдіть на міграції (див. розділ нижче).

### Рядок підключення

`appsettings.json`:

```json
"Server=(localdb)\\MSSQLLocalDB;Database=MovieCatalogDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Для SQL Server Express:

```json
"Server=.\\SQLEXPRESS;Database=MovieCatalogDb;Trusted_Connection=True;TrustServerCertificate=True"
```

---

## Як це працює

**`Program.cs`** — після рефакторингу містить лише виклики extension-методів:

```csharp
using WebApplication23333.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddApplicationDatabase(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.EnsureDatabaseCreated();
app.UseApplicationLocalization();
app.UseApplicationErrorHandling();
```

**`MoviesController.Index`** — контролер лише просить дані в сервісу:

```csharp
public async Task<IActionResult> Index()
{
    var movies = await _movieService.GetAllAsync();
    return View(movies);
}
```

сервіс формулює правило сортування й передає його в репозиторій:

```csharp
public async Task<IReadOnlyList<Movie>> GetAllAsync()
{
    return await _repository.ListAsync(
        orderBy: query => query
            .OrderByDescending(m => m.Rating)
            .ThenBy(m => m.Title));
}
```

а сам запит до бази виконує `Repository<TEntity>` — єдиний клас, що знає про EF Core:

```csharp
IQueryable<TEntity> query = _set;

if (asNoTracking) query = query.AsNoTracking();
if (predicate is not null) query = query.Where(predicate);
if (orderBy is not null) query = orderBy(query);

return await query.ToListAsync();
```

**`AppDbContext`** — конфігурація моделі та seed:

```csharp
modelBuilder.Entity<Movie>(entity =>
{
    entity.ToTable("Movies");
    entity.HasKey(m => m.Id);
    entity.Property(m => m.Rating).HasColumnType("decimal(3,1)");
    entity.HasIndex(m => m.Rating);
});

modelBuilder.Entity<Movie>().HasData(SeedMovies);
```

---

## CRUD-операції

Усі операції над фільмами зібрані в одному контролері — `MoviesController`.

| Дія | Маршрут | Метод | Опис |
| --- | --- | --- | --- |
| **Read** | `/Movies` | GET | Сітка всіх фільмів, відсортована за оцінкою |
| **Details** | `/Movies/Details/{id}` | GET | Повна картка: рік, режисер, жанр, опис, постер |
| **Create** | `/Movies/Create` | GET / POST | Форма додавання + завантаження постера |
| **Update** | `/Movies/Edit/{id}` | GET / POST | Редагування полів і заміна постера |
| **Delete** | `/Movies/Delete/{id}` | GET / POST | Сторінка підтвердження, потім видалення |

---

## Сервісний шар і Dependency Injection

Раніше `MoviesController` сам звертався до `AppDbContext` і до файлової системи.
Після рефакторингу вся ця робота винесена в **сервісний шар**, а контролер
отримує сервіс через конструктор — **Dependency Injection**.

### Інтерфейси та реалізації — папка `Services/`

| Тип | Призначення |
| --- | --- |
| `IMovieService` / `MovieService` | бізнес-логіка каталогу: `GetAllAsync`, `GetByIdAsync`, `ExistsAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`, `IsTitleUniqueAsync` |
| `IPosterStorage` / `PosterStorage` | робота з файлами постерів: збереження в `wwwroot/images/uploads`, видалення старого файлу, шлях до заглушки |

`MovieService` отримує `IRepository<Movie>` та `IPosterStorage` теж через конструктор —
жоден клас не створює свої залежності сам (`new` немає ніде).

### Extension-методи — папка `Extensions/`

`ServiceCollectionExtensions` — реєстрація в DI-контейнері:

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    services.AddApplicationRepositories();

    services.AddScoped<IPosterStorage, PosterStorage>();
    services.AddScoped<IMovieService, MovieService>();

    return services;
}

public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
{
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    return services;
}

public static IServiceCollection AddApplicationDatabase(
    this IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

    return services;
}
```

`WebApplicationExtensions` — налаштування конвеєра: `EnsureDatabaseCreated()`,
`UseApplicationLocalization()`, `UseApplicationErrorHandling()`.

Усі методи повертають той самий об'єкт, тому виклики можна писати ланцюжком.

### Тонкий контролер

```csharp
public class MoviesController : Controller
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie)
    {
        ModelState.Remove(nameof(Movie.PosterPath));

        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        var created = await _movieService.CreateAsync(movie);

        TempData["Message"] = $"Фільм «{created.Title}» додано.";
        return RedirectToAction(nameof(Index));
    }
}
```

У контролері не залишилось ні `AppDbContext`, ні `IWebHostEnvironment`, ні
`System.IO` — тільки перевірка `ModelState`, виклик сервісу та вибір результату
(`View` / `NotFound` / `RedirectToAction`).

### Що це дає

- **Єдина відповідальність:** контролер відповідає за HTTP, сервіс — за логіку.
- **Повторне використання:** `UniqueMovieTitleAttribute` бере той самий
  `IMovieService` із сервіс-провайдера замість прямого запиту до бази.
- **Заміна реалізації:** можна підставити інший `IPosterStorage`
  (наприклад, хмарне сховище) або інший `IRepository`, не змінюючи ні контролер,
  ні `MovieService`.
- **Тестованість:** сервіс можна підмінити заглушкою в юніт-тестах контролера.

---

## Шар репозиторію

Сервіс не працює з `DbContext` напряму — між ними стоїть **репозиторій**.
Ланцюжок залежностей:

```
MoviesController → IMovieService → IRepository<Movie> → AppDbContext → SQL Server
     (HTTP)         (бізнес-логіка)   (доступ до даних)      (EF Core)
```

Кожна стрілка — це інтерфейс, який приходить через конструктор, тому жоден
шар не знає про конкретну реалізацію наступного (Dependency Inversion Principle).

### `IRepository<TEntity>` — папка `Repositories/`

Узагальнений контракт: підходить для будь-якої сутності, не лише для `Movie`.

```csharp
public interface IRepository<TEntity> where TEntity : class
{
    Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool asNoTracking = true);

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    bool Any(Expression<Func<TEntity, bool>> predicate);

    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<int> SaveChangesAsync();
}
```

`Repository<TEntity>` — єдиний клас, який тримає `AppDbContext` і `DbSet<TEntity>`;
усі виклики EF Core (`ToListAsync`, `AsNoTracking`, `SaveChangesAsync`) живуть тільки тут.

### Реєстрація відкритого узагальненого типу

```csharp
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

Один рядок покриває всі сутності: `IRepository<Movie>` контейнер побудує сам.

### Сервіс після рефакторингу

Сортування Топ-10 — це бізнес-правило, тому воно залишається в сервісі й
передається в репозиторій параметром, а не «зашивається» в доступ до даних:

```csharp
public async Task<IReadOnlyList<Movie>> GetAllAsync()
{
    return await _repository.ListAsync(
        orderBy: query => query
            .OrderByDescending(m => m.Rating)
            .ThenBy(m => m.Title));
}
```

У `MovieService` не залишилось жодної згадки про `AppDbContext` чи простір імен
`Microsoft.EntityFrameworkCore` — замінити EF Core на будь-яке інше сховище
можна, переписавши лише `Repository<TEntity>`.

---

## Валідація форм

Перевірка виконується **двічі** — у браузері (щоб користувач одразу бачив помилку)
і на сервері (бо клієнтську валідацію легко обійти).

### Вбудовані атрибути Data Annotations

`Models/Movie.cs`: `Required`, `StringLength` (з `MinimumLength`), `Range`,
`RegularExpression`, `DataType`, `Display`, `Key`, `NotMapped`.

### Власні (кастомні) атрибути — папка `Validation/`

| Атрибут | Що перевіряє | Клієнт |
| --- | --- | --- |
| `ReleaseYearAttribute` | рік від 1888 до «поточний + 1» (щоб можна було додати анонс) | так, `IClientModelValidator` |
| `ForbiddenWordsAttribute` | текст не містить слів-заглушок (`lorem`, `test`, `asdf`…) | так, `IClientModelValidator` |
| `PosterFileAttribute` | розширення та розмір обраного файлу постера | так, через File API |
| `UniqueMovieTitleAttribute` | назва фільму унікальна в каталозі (звертається до `IMovieService`) | ні — на клієнті цю ж перевірку робить `[Remote]` |

Перші три реалізують `IClientModelValidator`: атрибут сам дописує до `<input>`
теги `data-val-*`, а `wwwroot/js/validation.js` реєструє відповідні методи та
адаптери для `jquery.validate.unobtrusive`.

### Перевірки рівня всієї моделі

`Movie` реалізує `IValidatableObject` — правила, які залежать від кількох полів одразу:

- оригінальна назва не повинна дублювати основну;
- фільму, який ще не вийшов (рік більший за поточний), не можна виставляти оцінку —
  ця помилка не прив'язана до поля і потрапляє у загальний блок.

### Виведення помилок — тег-хелпери

```html
<div asp-validation-summary="All" class="validation-summary"></div>
...
<input asp-for="Title" class="input" />
<span asp-validation-for="Title" class="field-error"></span>
```

### Оформлення

У `site.css` описані стилі та анімації для помилок:

- повідомлення поля — червоний колір, іконка ⚠ через `::before`,
  поява з анімацією `error-pop` (fade + зсув);
- поле з помилкою — червона рамка, м'яке підсвічування і коротке тремтіння `error-shake`;
- загальний блок — червона рамка зліва, напівпрозорий фон, заголовок
  «Форму не збережено — виправте помилки», а при спробі відправити невалідну
  форму блок додатково «тремтить» (клас `is-shaking` з `validation.js`).

Форми захищені `[ValidateAntiForgeryToken]` і відправляються з `novalidate`,
щоб показувати власні стилізовані повідомлення замість стандартних браузерних.

### Завантаження постера

Форми `Create` і `Edit` відправляються як `multipart/form-data`, файл потрапляє
у властивість `Movie.PosterFile` (`[NotMapped]`), а всю роботу з ним виконує
сервіс `PosterStorage`:

- дозволені формати — `.jpg`, `.jpeg`, `.png`, `.webp`, `.gif`;
- максимальний розмір — 5 МБ;
- файл зберігається у `wwwroot/images/uploads` під іменем-`Guid`, у базу пише́ться лише шлях;
- контролер про фізичні шляхи не знає — він викликає лише `IMovieService`;
- якщо файл не обрано під час створення — підставляється заглушка `poster-placeholder.svg`;
- при редагуванні старий завантажений постер видаляється, при видаленні фільму — теж.

### Сторінка 404

У extension-методі `UseApplicationErrorHandling()` підключено
`app.UseStatusCodePagesWithReExecute("/Home/StatusCodeHandler/{0}")`.
`HomeController.StatusCodeHandler` для коду 404 повертає власне подання
`Views/Shared/NotFound.cshtml`, для інших кодів — `Error.cshtml`.
Неіснуючий `id` у `Details` / `Edit` / `Delete` теж призводить до цієї сторінки.

---

## Міграції EF Core (опційно)

Якщо потрібні саме міграції замість `EnsureCreated()`:

```bash
dotnet tool install --global dotnet-ef
cd WebApplication23333
dotnet ef migrations add InitialCreate
dotnet ef database update
```

І в `Extensions/WebApplicationExtensions.cs` замінити `context.Database.EnsureCreated()`
на `context.Database.Migrate()`.

---

## Список фільмів у базі

| # | Фільм | Режисер | Жанр | Рік | Оцінка |
| --- | --- | --- | --- | --- | --- |
| 1 | Втеча з Шоушенка | Френк Дарабонт | Драма | 1994 | 9.3 |
| 2 | Хрещений батько | Френсіс Форд Коппола | Кримінальна драма | 1972 | 9.2 |
| 3 | Володар перснів: Повернення короля | Пітер Джексон | Фентезі, пригоди | 2003 | 9.0 |
| 4 | Темний лицар | Крістофер Нолан | Бойовик, трилер | 2008 | 9.0 |
| 5 | Кримінальне чтиво | Квентін Тарантіно | Кримінальна комедія | 1994 | 8.9 |
| 6 | Форрест Ґамп | Роберт Земекіс | Драма, комедія | 1994 | 8.8 |
| 7 | Початок | Крістофер Нолан | Наукова фантастика, трилер | 2010 | 8.8 |
| 8 | Інтерстеллар | Крістофер Нолан | Наукова фантастика, драма | 2014 | 8.7 |
| 9 | Матриця | Лана та Ліллі Вачовскі | Наукова фантастика | 1999 | 8.7 |
| 10 | Паразити | Пон Джун Хо | Трилер, драма | 2019 | 8.5 |

---

## Про постери

Стартові 10 постерів — **власна згенерована графіка** (градієнт + геометричний
мотив + назва), чужих зображень у репозиторії немає. Постери, які користувач
завантажує через форму, потрапляють у `wwwroot/images/uploads` і в репозиторій
не комітяться.
