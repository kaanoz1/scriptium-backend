using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ScriptiumBackend.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "c_chapter",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_chapter", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "c_node",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_node", x => x.id);
                    table.ForeignKey(
                        name: "FK_c_node_c_node_parent_id",
                        column: x => x.parent_id,
                        principalTable: "c_node",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "c_root",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    latin = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_root", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "c_scripture",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<char>(type: "character(1)", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_scripture", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "c_verse",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_verse", x => x.id);
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
                name: "i_q_chapter",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    chapter_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_chapter", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_chapter_c_chapter_chapter_id",
                        column: x => x.chapter_id,
                        principalTable: "c_chapter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "c_word",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sequence_number = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    verse_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_word", x => x.id);
                    table.ForeignKey(
                        name: "FK_c_word_c_verse_verse_id",
                        column: x => x.verse_id,
                        principalTable: "c_verse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "s_meaning",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    language_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_meaning", x => x.id);
                    table.ForeignKey(
                        name: "FK_s_meaning_s_language_language_id",
                        column: x => x.language_id,
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
                    content = table.Column<string>(type: "text", nullable: false),
                    language_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_transliteration", x => x.id);
                    table.ForeignKey(
                        name: "FK_s_transliteration_s_language_language_id",
                        column: x => x.language_id,
                        principalTable: "s_language",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "i_q_verse",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    verse_id = table.Column<long>(type: "bigint", nullable: false),
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
                        name: "FK_i_q_verse_c_verse_verse_id",
                        column: x => x.verse_id,
                        principalTable: "c_verse",
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
                name: "c_mm_root_word",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    root_id = table.Column<long>(type: "bigint", nullable: false),
                    word_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_mm_root_word", x => x.id);
                    table.ForeignKey(
                        name: "FK_c_mm_root_word_c_root_root_id",
                        column: x => x.root_id,
                        principalTable: "c_root",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_c_mm_root_word_c_word_word_id",
                        column: x => x.word_id,
                        principalTable: "c_word",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "i_q_word",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    word_c_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_word", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_word_c_word_word_c_id",
                        column: x => x.word_c_id,
                        principalTable: "c_word",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "c_scripture_meanings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    meaning_id = table.Column<long>(type: "bigint", nullable: false),
                    scripture_id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_scripture_meanings", x => x.id);
                    table.ForeignKey(
                        name: "FK_c_scripture_meanings_c_scripture_scripture_id",
                        column: x => x.scripture_id,
                        principalTable: "c_scripture",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_c_scripture_meanings_s_meaning_meaning_id",
                        column: x => x.meaning_id,
                        principalTable: "s_meaning",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "i_q_chapter_meanings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    i_q_chapter_id = table.Column<short>(type: "smallint", nullable: false),
                    meaning_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_chapter_meanings", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_chapter_meanings_i_q_chapter_i_q_chapter_id",
                        column: x => x.i_q_chapter_id,
                        principalTable: "i_q_chapter",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_i_q_chapter_meanings_s_meaning_meaning_id",
                        column: x => x.meaning_id,
                        principalTable: "s_meaning",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "i_q_word_meanings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    meaning_id = table.Column<long>(type: "bigint", nullable: false),
                    i_q_word_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_word_meanings", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_word_meanings_i_q_word_i_q_word_id",
                        column: x => x.i_q_word_id,
                        principalTable: "i_q_word",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_q_word_meanings_s_meaning_meaning_id",
                        column: x => x.meaning_id,
                        principalTable: "s_meaning",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "i_q_word_transliterations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    transliteration_id = table.Column<long>(type: "bigint", nullable: false),
                    i_q_word_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_q_word_transliterations", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_q_word_transliterations_i_q_word_i_q_word_id",
                        column: x => x.i_q_word_id,
                        principalTable: "i_q_word",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_q_word_transliterations_s_transliteration_transliteration~",
                        column: x => x.transliteration_id,
                        principalTable: "s_transliteration",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "s_language",
                columns: new[] { "id", "code", "english_name", "name" },
                values: new object[] { 1, "en", "English", "English" });

            migrationBuilder.CreateIndex(
                name: "IX_c_mm_root_word_root_id_word_id",
                table: "c_mm_root_word",
                columns: new[] { "root_id", "word_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_c_mm_root_word_word_id",
                table: "c_mm_root_word",
                column: "word_id");

            migrationBuilder.CreateIndex(
                name: "IX_c_node_parent_id",
                table: "c_node",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_c_scripture_meanings_meaning_id",
                table: "c_scripture_meanings",
                column: "meaning_id");

            migrationBuilder.CreateIndex(
                name: "IX_c_scripture_meanings_scripture_id_meaning_id",
                table: "c_scripture_meanings",
                columns: new[] { "scripture_id", "meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_c_word_verse_id",
                table: "c_word",
                column: "verse_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_q_chapter_chapter_id",
                table: "i_q_chapter",
                column: "chapter_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_i_q_chapter_meanings_i_q_chapter_id_meaning_id",
                table: "i_q_chapter_meanings",
                columns: new[] { "i_q_chapter_id", "meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_i_q_chapter_meanings_meaning_id",
                table: "i_q_chapter_meanings",
                column: "meaning_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_q_verse_i_q_chapter_id",
                table: "i_q_verse",
                column: "i_q_chapter_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_q_verse_verse_id",
                table: "i_q_verse",
                column: "verse_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_i_q_word_word_c_id",
                table: "i_q_word",
                column: "word_c_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_q_word_meanings_i_q_word_id_meaning_id",
                table: "i_q_word_meanings",
                columns: new[] { "i_q_word_id", "meaning_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_i_q_word_meanings_meaning_id",
                table: "i_q_word_meanings",
                column: "meaning_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_q_word_transliterations_i_q_word_id_transliteration_id",
                table: "i_q_word_transliterations",
                columns: new[] { "i_q_word_id", "transliteration_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_i_q_word_transliterations_transliteration_id",
                table: "i_q_word_transliterations",
                column: "transliteration_id");

            migrationBuilder.CreateIndex(
                name: "IX_s_meaning_language_id",
                table: "s_meaning",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "IX_s_transliteration_language_id",
                table: "s_transliteration",
                column: "language_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "c_mm_root_word");

            migrationBuilder.DropTable(
                name: "c_node");

            migrationBuilder.DropTable(
                name: "c_scripture_meanings");

            migrationBuilder.DropTable(
                name: "i_q_chapter_meanings");

            migrationBuilder.DropTable(
                name: "i_q_verse");

            migrationBuilder.DropTable(
                name: "i_q_word_meanings");

            migrationBuilder.DropTable(
                name: "i_q_word_transliterations");

            migrationBuilder.DropTable(
                name: "c_root");

            migrationBuilder.DropTable(
                name: "c_scripture");

            migrationBuilder.DropTable(
                name: "i_q_chapter");

            migrationBuilder.DropTable(
                name: "s_meaning");

            migrationBuilder.DropTable(
                name: "i_q_word");

            migrationBuilder.DropTable(
                name: "s_transliteration");

            migrationBuilder.DropTable(
                name: "c_chapter");

            migrationBuilder.DropTable(
                name: "c_word");

            migrationBuilder.DropTable(
                name: "s_language");

            migrationBuilder.DropTable(
                name: "c_verse");
        }
    }
}
