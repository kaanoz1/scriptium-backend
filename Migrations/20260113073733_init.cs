using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;

#nullable disable

namespace ScriptiumBackend.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "a_chapter",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_a_chapter", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "a_root",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_a_root", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "a_verse",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_a_verse", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "a_word",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_a_word", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "s_language",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    english_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_language", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "s_scripture",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<char>(type: "character(1)", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_scripture", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "u_cache",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_u_cache", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "u_searchable_item",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    embedding = table.Column<Vector>(type: "vector(768)", nullable: true),
                    last_embedded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_u_searchable_item", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "i_q_chapter",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_chapter", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_chapter_a_chapter_id",
                        column: x => x.id,
                        principalTable: "a_chapter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "i_q_root",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    latin = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_root", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_root_a_root_id",
                        column: x => x.id,
                        principalTable: "a_root",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "a_translation",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    published_at = table.Column<DateOnly>(type: "date", nullable: false),
                    s_language_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_a_translation", x => x.id);
                    table.ForeignKey(
                        name: "FK_a_translation_s_language_s_language_id",
                        column: x => x.s_language_id,
                        principalTable: "s_language",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "s_author",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    s_language_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_author", x => x.id);
                    table.ForeignKey(
                        name: "FK_s_author_s_language_s_language_id",
                        column: x => x.s_language_id,
                        principalTable: "s_language",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "s_meaning",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    s_language_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_meaning", x => x.id);
                    table.ForeignKey(
                        name: "FK_s_meaning_s_language_s_language_id",
                        column: x => x.s_language_id,
                        principalTable: "s_language",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "s_transliteration",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "character varying(2500)", maxLength: 2500, nullable: false),
                    s_language_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_transliteration", x => x.id);
                    table.ForeignKey(
                        name: "FK_s_transliteration_s_language_s_language_id",
                        column: x => x.s_language_id,
                        principalTable: "s_language",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "u_cache_record",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cache_id = table.Column<long>(type: "bigint", nullable: false),
                    fetched_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_u_cache_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_u_cache_record_u_cache_cache_id",
                        column: x => x.cache_id,
                        principalTable: "u_cache",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "a_translationUnit",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    text = table.Column<string>(type: "character varying(2500)", maxLength: 2500, nullable: false),
                    s_language_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_a_translationUnit", x => x.id);
                    table.ForeignKey(
                        name: "FK_a_translationUnit_s_language_s_language_id",
                        column: x => x.s_language_id,
                        principalTable: "s_language",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_a_translationUnit_u_searchable_item_id",
                        column: x => x.id,
                        principalTable: "u_searchable_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "i_q_verse",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    i_q_chapter_id = table.Column<short>(type: "smallint", nullable: false),
                    simple = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    simple_plain = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    simple_minimal = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    simple_clean = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    uthmani = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    uthmani_minimal = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_verse", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_verse_a_verse_id",
                        column: x => x.id,
                        principalTable: "a_verse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_q_verse_i_q_chapter_i_q_chapter_id",
                        column: x => x.i_q_chapter_id,
                        principalTable: "i_q_chapter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "i_q_translation",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_translation", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_translation_a_translation_id",
                        column: x => x.id,
                        principalTable: "a_translation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mm_s_translation__s_author",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    s_author_id = table.Column<long>(type: "bigint", nullable: false),
                    s_translation_id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mm_s_translation__s_author", x => x.id);
                    table.ForeignKey(
                        name: "FK_mm_s_translation__s_author_a_translation_s_translation_id",
                        column: x => x.s_translation_id,
                        principalTable: "a_translation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mm_s_translation__s_author_s_author_s_author_id",
                        column: x => x.s_author_id,
                        principalTable: "s_author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mm_i_q_chapter__s_meanings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    i_q_chapter_id = table.Column<short>(type: "smallint", nullable: false),
                    s_meaning_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mm_i_q_chapter__s_meanings", x => x.id);
                    table.ForeignKey(
                        name: "FK_mm_i_q_chapter__s_meanings_i_q_chapter_i_q_chapter_id",
                        column: x => x.i_q_chapter_id,
                        principalTable: "i_q_chapter",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_mm_i_q_chapter__s_meanings_s_meaning_s_meaning_id",
                        column: x => x.s_meaning_id,
                        principalTable: "s_meaning",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mm_s_authorName__a_translations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    s_author_id = table.Column<long>(type: "bigint", nullable: false),
                    s_meaning_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mm_s_authorName__a_translations", x => x.id);
                    table.ForeignKey(
                        name: "FK_mm_s_authorName__a_translations_s_author_s_author_id",
                        column: x => x.s_author_id,
                        principalTable: "s_author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mm_s_authorName__a_translations_s_meaning_s_meaning_id",
                        column: x => x.s_meaning_id,
                        principalTable: "s_meaning",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mm_s_scripture__s_meanings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    s_meaning_id = table.Column<long>(type: "bigint", nullable: false),
                    s_scripture_id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mm_s_scripture__s_meanings", x => x.id);
                    table.ForeignKey(
                        name: "FK_mm_s_scripture__s_meanings_s_meaning_s_meaning_id",
                        column: x => x.s_meaning_id,
                        principalTable: "s_meaning",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_mm_s_scripture__s_meanings_s_scripture_s_scripture_id",
                        column: x => x.s_scripture_id,
                        principalTable: "s_scripture",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "s_footnote",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    indicator = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    index = table.Column<long>(type: "bigint", nullable: false),
                    a_translationUnit_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_footnote", x => x.id);
                    table.ForeignKey(
                        name: "FK_s_footnote_a_translationUnit_a_translationUnit_id",
                        column: x => x.a_translationUnit_id,
                        principalTable: "a_translationUnit",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "i_q_word",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    i_q_verse_id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_word", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_word_a_word_id",
                        column: x => x.id,
                        principalTable: "a_word",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_q_word_i_q_verse_i_q_verse_id",
                        column: x => x.i_q_verse_id,
                        principalTable: "i_q_verse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mm_i_q_verse__s_transliteration",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    s_transliteration_id = table.Column<long>(type: "bigint", nullable: false),
                    i_q_verse_id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mm_i_q_verse__s_transliteration", x => x.id);
                    table.ForeignKey(
                        name: "FK_mm_i_q_verse__s_transliteration_i_q_verse_i_q_verse_id",
                        column: x => x.i_q_verse_id,
                        principalTable: "i_q_verse",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_mm_i_q_verse__s_transliteration_s_transliteration_s_transli~",
                        column: x => x.s_transliteration_id,
                        principalTable: "s_transliteration",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "i_q_verse_translation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    i_q_verse_id = table.Column<short>(type: "smallint", nullable: false),
                    i_q_translation_id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_verse_translation", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_verse_translation_a_translationUnit_id",
                        column: x => x.id,
                        principalTable: "a_translationUnit",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_q_verse_translation_i_q_translation_i_q_translation_id",
                        column: x => x.i_q_translation_id,
                        principalTable: "i_q_translation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_i_q_verse_translation_i_q_verse_i_q_verse_id",
                        column: x => x.i_q_verse_id,
                        principalTable: "i_q_verse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mm_i_q_root__i_q_word",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    i_q_root_id = table.Column<int>(type: "integer", nullable: false),
                    i_q_word_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mm_i_q_root__i_q_word", x => x.id);
                    table.ForeignKey(
                        name: "FK_mm_i_q_root__i_q_word_i_q_root_i_q_root_id",
                        column: x => x.i_q_root_id,
                        principalTable: "i_q_root",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mm_i_q_root__i_q_word_i_q_word_i_q_word_id",
                        column: x => x.i_q_word_id,
                        principalTable: "i_q_word",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mm_i_q_word__s_meanings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    s_meaning_id = table.Column<long>(type: "bigint", nullable: false),
                    i_q_word_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mm_i_q_word__s_meanings", x => x.id);
                    table.ForeignKey(
                        name: "FK_mm_i_q_word__s_meanings_i_q_word_i_q_word_id",
                        column: x => x.i_q_word_id,
                        principalTable: "i_q_word",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mm_i_q_word__s_meanings_s_meaning_s_meaning_id",
                        column: x => x.s_meaning_id,
                        principalTable: "s_meaning",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mm_i_q_word__s_transliterations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    s_transliteration_id = table.Column<long>(type: "bigint", nullable: false),
                    i_q_word_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mm_i_q_word__s_transliterations", x => x.id);
                    table.ForeignKey(
                        name: "FK_mm_i_q_word__s_transliterations_i_q_word_i_q_word_id",
                        column: x => x.i_q_word_id,
                        principalTable: "i_q_word",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mm_i_q_word__s_transliterations_s_transliteration_s_transli~",
                        column: x => x.s_transliteration_id,
                        principalTable: "s_transliteration",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "s_language",
                columns: new[] { "id", "code", "english_name", "name" },
                values: new object[] { 1, "en", "English", "English" });

            migrationBuilder.CreateIndex(
                name: "IX_a_root_text",
                table: "a_root",
                column: "text",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_a_translation_s_language_id",
                table: "a_translation",
                column: "s_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_a_translationUnit_s_language_id",
                table: "a_translationUnit",
                column: "s_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_q_verse_i_q_chapter_id",
                table: "i_q_verse",
                column: "i_q_chapter_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_q_verse_translation_i_q_translation_id",
                table: "i_q_verse_translation",
                column: "i_q_translation_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_q_verse_translation_i_q_verse_id",
                table: "i_q_verse_translation",
                column: "i_q_verse_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_q_word_i_q_verse_id",
                table: "i_q_word",
                column: "i_q_verse_id");

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_chapter__s_meanings_i_q_chapter_id_s_meaning_id",
                table: "mm_i_q_chapter__s_meanings",
                columns: new[] { "i_q_chapter_id", "s_meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_chapter__s_meanings_s_meaning_id",
                table: "mm_i_q_chapter__s_meanings",
                column: "s_meaning_id");

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_root__i_q_word_i_q_root_id_i_q_word_id",
                table: "mm_i_q_root__i_q_word",
                columns: new[] { "i_q_root_id", "i_q_word_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_root__i_q_word_i_q_word_id",
                table: "mm_i_q_root__i_q_word",
                column: "i_q_word_id");

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_verse__s_transliteration_i_q_verse_id",
                table: "mm_i_q_verse__s_transliteration",
                column: "i_q_verse_id");

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_verse__s_transliteration_s_transliteration_id_i_q_ve~",
                table: "mm_i_q_verse__s_transliteration",
                columns: new[] { "s_transliteration_id", "i_q_verse_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_word__s_meanings_i_q_word_id_s_meaning_id",
                table: "mm_i_q_word__s_meanings",
                columns: new[] { "i_q_word_id", "s_meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_word__s_meanings_s_meaning_id",
                table: "mm_i_q_word__s_meanings",
                column: "s_meaning_id");

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_word__s_transliterations_i_q_word_id_s_transliterati~",
                table: "mm_i_q_word__s_transliterations",
                columns: new[] { "i_q_word_id", "s_transliteration_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mm_i_q_word__s_transliterations_s_transliteration_id",
                table: "mm_i_q_word__s_transliterations",
                column: "s_transliteration_id");

            migrationBuilder.CreateIndex(
                name: "IX_mm_s_authorName__a_translations_s_author_id_s_meaning_id",
                table: "mm_s_authorName__a_translations",
                columns: new[] { "s_author_id", "s_meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mm_s_authorName__a_translations_s_meaning_id",
                table: "mm_s_authorName__a_translations",
                column: "s_meaning_id");

            migrationBuilder.CreateIndex(
                name: "IX_mm_s_scripture__s_meanings_s_meaning_id",
                table: "mm_s_scripture__s_meanings",
                column: "s_meaning_id");

            migrationBuilder.CreateIndex(
                name: "IX_mm_s_scripture__s_meanings_s_scripture_id_s_meaning_id",
                table: "mm_s_scripture__s_meanings",
                columns: new[] { "s_scripture_id", "s_meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mm_s_translation__s_author_s_author_id",
                table: "mm_s_translation__s_author",
                column: "s_author_id");

            migrationBuilder.CreateIndex(
                name: "IX_mm_s_translation__s_author_s_translation_id_s_author_id",
                table: "mm_s_translation__s_author",
                columns: new[] { "s_translation_id", "s_author_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_s_author_s_language_id",
                table: "s_author",
                column: "s_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_s_footnote_a_translationUnit_id",
                table: "s_footnote",
                column: "a_translationUnit_id");

            migrationBuilder.CreateIndex(
                name: "IX_s_meaning_s_language_id",
                table: "s_meaning",
                column: "s_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_s_transliteration_s_language_id",
                table: "s_transliteration",
                column: "s_language_id");

            migrationBuilder.CreateIndex(
                name: "IX_u_cache_url",
                table: "u_cache",
                column: "url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_u_cache_record_cache_id",
                table: "u_cache_record",
                column: "cache_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "i_q_verse_translation");

            migrationBuilder.DropTable(
                name: "mm_i_q_chapter__s_meanings");

            migrationBuilder.DropTable(
                name: "mm_i_q_root__i_q_word");

            migrationBuilder.DropTable(
                name: "mm_i_q_verse__s_transliteration");

            migrationBuilder.DropTable(
                name: "mm_i_q_word__s_meanings");

            migrationBuilder.DropTable(
                name: "mm_i_q_word__s_transliterations");

            migrationBuilder.DropTable(
                name: "mm_s_authorName__a_translations");

            migrationBuilder.DropTable(
                name: "mm_s_scripture__s_meanings");

            migrationBuilder.DropTable(
                name: "mm_s_translation__s_author");

            migrationBuilder.DropTable(
                name: "s_footnote");

            migrationBuilder.DropTable(
                name: "u_cache_record");

            migrationBuilder.DropTable(
                name: "i_q_translation");

            migrationBuilder.DropTable(
                name: "i_q_root");

            migrationBuilder.DropTable(
                name: "i_q_word");

            migrationBuilder.DropTable(
                name: "s_transliteration");

            migrationBuilder.DropTable(
                name: "s_meaning");

            migrationBuilder.DropTable(
                name: "s_scripture");

            migrationBuilder.DropTable(
                name: "s_author");

            migrationBuilder.DropTable(
                name: "a_translationUnit");

            migrationBuilder.DropTable(
                name: "u_cache");

            migrationBuilder.DropTable(
                name: "a_translation");

            migrationBuilder.DropTable(
                name: "a_root");

            migrationBuilder.DropTable(
                name: "a_word");

            migrationBuilder.DropTable(
                name: "i_q_verse");

            migrationBuilder.DropTable(
                name: "u_searchable_item");

            migrationBuilder.DropTable(
                name: "s_language");

            migrationBuilder.DropTable(
                name: "a_verse");

            migrationBuilder.DropTable(
                name: "i_q_chapter");

            migrationBuilder.DropTable(
                name: "a_chapter");
        }
    }
}
