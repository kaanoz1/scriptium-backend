# Scriptium Backend
This repository represents backend application of [Scriptium](github.com/scriptium-project)

## Getting Started

### Prerequisites
- .NET 10 SDK
- EF Core .NET Tools (`dotnet tool install --global dotnet-ef`)
- Docker (Optional)

You should already have installed SDK for .NET 10 and EF Core .NET Tools.

### Installation

```bash
git clone https://github.com/kaanoz1/scriptium-backend.git
cd scriptium-backend
```

After that, you can run this project either locally or **Docker**.

### Configuration (For Local Run):
 	
You should create you own appsettings.json.

```bash
cp appsettings.Development.Example.json appsettings.Development.json # For Development
cp appsettings.Development.Example.json appsettings.json # For Production
```

if you use docker, you should pass this environment variables into the container. The program will stop you unless you pass necessary environment variables. You can get a hint from appsettings.json files to understand which variables are necessary.

#### Migrations:

Project automatically detects uncommitted migrations and apply them automatically. Check out `Program.cs`. if you want to do it manual. Run: 
```bash
dotnet ef database update
```

#### Build the project:

```bash
dotnet restore
dotnet build
dotnet run
```

Once running, the API is typically available at http://localhost:5000 (check your console output for the specific port).

You are good to go.

## Hadith corpus (Arabic only)

Migration `AddHadithArabicOnly` adds the `i_h_ao_*` tables ("ao" = Arabic only;
translated collections can be added later as separate corpora). They are
filled by [scriptium-data-loader](../scriptium-data-loader) from its hadith
source database; primary keys are the source's ids. Models live in
`Model/Derived/Islam/Hadith/ArabicOnly`, the EF mapping in
`Db/Configurations/Islam/HadithArabicOnlyConfiguration.cs`.

All endpoints are under `api/islam/hadith/ao` and return `{ data }`:

| Endpoint | |
|---|---|
| `GET overview` | counts, books by category, grade/type/rank distributions, top narrators, root subjects |
| `GET books` | books grouped by category |
| `GET book/{bookId}` · `GET book/{bookId}/hadiths?page&pageSize&grade&type` · `GET book/{bookId}/locate/{number}` | a book, its hadiths, the list page containing a number |
| `GET hadith/{bookId}/{number}` | text + matn span, chains, narrator mentions, Qur'an quotes, meaning group, subjects, other narrations, prev/next |
| `GET hadith/{bookId}/{number}/isnad` | all chains of the hadith merged into one graph |
| `GET topics` · `GET topic/{id}` | subject tree |
| `GET group/{id}` · `.../hadiths?book&grade&page` · `.../isnad` | all narrations of one hadith and their merged isnad tree |
| `GET search?query&mode=Smart\|Exact\|Phrase&book&category&grade&type&page` | Arabic full-text search with match highlights |
| `GET quran/{chapter}/{verse}/hadiths` | hadiths quoting a verse |
| `GET sitemap` · `sitemap/hadiths/{page}` · `sitemap/groups/{page}` | data for the frontend's sitemaps |

### Jarh wa ta'dil (Arabic only)

Narrator criticism is a separate corpus with its own prefix,
`api/islam/jarh-tadil/ao` (`Controllers/Islam/JarhTadil/ArabicOnly`), so that
translated jarh wa ta'dil works can be added next to it. It reads the narrator
and critic tables of the Arabic hadith corpus, which the chains reference.

| Endpoint | |
|---|---|
| `GET overview` | counts, narrator rank distribution, top narrators, top critics |
| `GET narrators?query&rank&generation&bukhari&muslim&page` | narrator browser / search |
| `GET narrator/{id}` · `.../hadiths?book&grade&page` · `.../teachers` · `.../students` | biography, critics' statements, teachers, students, books, hadiths |
| `GET scholars` · `GET scholar/{id}` · `GET scholar/{id}/statements?page` | critics and their statements |
| `GET sitemap` · `sitemap/narrators/{page}` | data for the frontend's sitemaps |

`NarratorRank` reports the critics' verdicts as recorded by the source
(e.g. `Suspected` = متهم, `AttributedWithLying` = كذاب / وضاع); it is not the
site's own judgement of a person.

Codes are enums in `Model/Derived/Islam/Hadith/ArabicOnly/Enums` (stored as
`smallint`, serialised as numbers; query filters accept the number or the name,
e.g. `grade=0` or `grade=Sahih`): `HadithType`, `HadithGrade` (hadith rows),
`ChainGrade` (chains, meaning groups, tarafs), `NarratorRank` (narrators and
critics) and `BookCategoryKind`.

Schema configuration: `ScriptiumDbContext` configures the shared tables and
calls `Db/Configurations/Islam/IslamConfiguration`, which applies
`QuranConfiguration` and `HadithArabicOnlyConfiguration`.

Search runs on `to_tsvector('simple', search_text)` (GIN expression index).
Queries go through `Utils/Text/ArabicNormalizer`, which must match the data
loader's `normalize_arabic`; `Smart` mode also matches attached particles
(و ف ب ل ك ال …) and suffixes. Text offsets (`start`/`end`, `matnStart`/`matnEnd`)
are `[start, end)` character indices into `text`.

## 📄 License

This project is licensed under the [MIT License](LICENCE) on behalf of Scriptium.



