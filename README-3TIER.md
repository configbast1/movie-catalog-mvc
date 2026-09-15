# Кінопошук: розбиття на три рівні

Проєкт `WebApplication23333` розділено на три окремі проєкти в одному рішенні.

```
Kino.sln
├── Kino.PL    ASP.NET Core MVC   - рівень представлення
├── Kino.BLL   Class Library      - бізнес-логіка
└── Kino.DAL   Class Library      - доступ до даних
```

Залежності йдуть строго в один бік: **PL → BLL → DAL**.
PL не посилається на DAL взагалі, тому з контролера фізично неможливо дістатися до `DbContext`.

## Kino.DAL

| Файл | Що робить |
|---|---|
| `Entities/Movie.cs` | чиста сутність без атрибутів валідації та без `IFormFile` |
| `Data/AppDbContext.cs` | контекст, конфігурація таблиці, початкові дані (10 фільмів) |
| `Repositories/IRepository.cs`, `Repository.cs` | узагальнений репозиторій |
| `DependencyInjection.cs` | `AddDataAccessLayer(configuration)` |

Пакети: `Microsoft.EntityFrameworkCore.SqlServer`.

## Kino.BLL

| Файл | Що робить |
|---|---|
| `DTO/MovieDto.cs` | об'єкт передачі даних між рівнями |
| `Interfaces/IMovieService.cs` | контракт бізнес-логіки |
| `Interfaces/IPosterStorage.cs` | абстракція сховища постерів (приймає `Stream`, а не `IFormFile`) |
| `Services/MovieService.cs` | бізнес-правила, робота з постерами, логування |
| `Mapping/MovieMapper.cs` | `Movie` ↔ `MovieDto` |
| `DependencyInjection.cs` | `AddBusinessLogicLayer(configuration)`, всередині викликає `AddDataAccessLayer` |

Посилань на ASP.NET Core немає — тільки `Microsoft.Extensions.*`.

## Kino.PL

| Файл | Що робить |
|---|---|
| `ViewModels/MovieViewModel.cs` | модель форми з усіма атрибутами валідації та `IFormFile PosterFile` |
| `ViewModels/MovieViewModelMapper.cs` | `MovieViewModel` ↔ `MovieDto` |
| `Controllers/MoviesController.cs` | тонкий контролер, знає лише `IMovieService` |
| `Services/PosterStorage.cs` | реалізація `IPosterStorage` — тут є доступ до `wwwroot` |
| `Validation/*` | кастомні атрибути валідації |
| `Views/*`, `wwwroot/*` | без змін, окрім директив `@model` |

## Що саме змінилося порівняно з однопроєктною версією

1. **Сутність розділено на дві моделі.** Раніше клас `Movie` одночасно був і таблицею БД,
   і моделлю форми: мав `[Required]`, `[Remote]` і навіть `IFormFile`. Тепер
   `Kino.DAL.Entities.Movie` — чистий клас для БД, а `Kino.PL.ViewModels.MovieViewModel` —
   модель форми з валідацією.

2. **З'явився DTO.** Сервіс приймає і повертає `MovieDto`, сутність із DAL на рівень
   представлення не потрапляє.

3. **`IPosterStorage` більше не знає про `IFormFile`.** Метод приймає `Stream` та ім'я файлу —
   інакше BLL довелося б посилатися на ASP.NET Core. Сама реалізація лишилась у PL, бо
   залежить від `IWebHostEnvironment`.

4. **Реєстрація сервісів піднімається знизу вгору.** `Program.cs` викликає лише
   `AddBusinessLogicLayer`, а той усередині підключає рівень даних.

## Запуск

1. Відкрити `Kino.sln`.
2. Перевірити рядок підключення в `Kino.PL/appsettings.json`.
3. Стартовий проєкт — `Kino.PL`, далі F5.

База створюється автоматично через `EnsureDatabaseCreated()` разом із початковими даними.
