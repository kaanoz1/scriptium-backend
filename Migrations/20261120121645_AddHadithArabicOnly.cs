using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScriptiumBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddHadithArabicOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "i_h_ao_book_category",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_book_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_meaning_group",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    is_qudsi = table.Column<bool>(type: "boolean", nullable: false),
                    grade = table.Column<short>(type: "smallint", nullable: false),
                    companion_count = table.Column<int>(type: "integer", nullable: false),
                    repeat_count = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    hadith_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_meaning_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_narrator",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: false),
                    rank_no = table.Column<short>(type: "smallint", nullable: false),
                    rank = table.Column<string>(type: "text", nullable: true),
                    generation = table.Column<short>(type: "smallint", nullable: false),
                    is_bukhari = table.Column<bool>(type: "boolean", nullable: false),
                    is_muslim = table.Column<bool>(type: "boolean", nullable: false),
                    has_ikhtilat = table.Column<bool>(type: "boolean", nullable: false),
                    has_tadlis = table.Column<bool>(type: "boolean", nullable: false),
                    riwaya_count = table.Column<int>(type: "integer", nullable: false),
                    birth_year = table.Column<int>(type: "integer", nullable: true),
                    birth_year_raw = table.Column<string>(type: "text", nullable: true),
                    death_year = table.Column<int>(type: "integer", nullable: true),
                    death_year_raw = table.Column<string>(type: "text", nullable: true),
                    age_raw = table.Column<string>(type: "text", nullable: true),
                    profession = table.Column<string>(type: "text", nullable: true),
                    lineage = table.Column<string>(type: "text", nullable: true),
                    residence = table.Column<string>(type: "text", nullable: true),
                    death_place = table.Column<string>(type: "text", nullable: true),
                    is_stub = table.Column<bool>(type: "boolean", nullable: false),
                    search_name = table.Column<string>(type: "text", nullable: false),
                    hadith_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_narrator", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_scholar",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: true),
                    known_as = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    generation = table.Column<short>(type: "smallint", nullable: false),
                    death_year = table.Column<int>(type: "integer", nullable: false),
                    rank_no = table.Column<short>(type: "smallint", nullable: false),
                    rank = table.Column<string>(type: "text", nullable: true),
                    statement_count = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_scholar", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_book",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    author_no = table.Column<int>(type: "integer", nullable: false),
                    author_name = table.Column<string>(type: "text", nullable: false),
                    author_death_year = table.Column<int>(type: "integer", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    publisher = table.Column<string>(type: "text", nullable: true),
                    edition = table.Column<string>(type: "text", nullable: true),
                    i_h_ao_book_category_id = table.Column<short>(type: "smallint", nullable: false),
                    hadith_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_book", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_h_ao_book_i_h_ao_book_category_i_h_ao_book_category_id",
                        column: x => x.i_h_ao_book_category_id,
                        principalTable: "i_h_ao_book_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_taraf",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    grade = table.Column<short>(type: "smallint", nullable: false),
                    is_qudsi = table.Column<bool>(type: "boolean", nullable: false),
                    i_h_ao_meaning_group_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_taraf", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_h_ao_taraf_i_h_ao_meaning_group_i_h_ao_meaning_group_id",
                        column: x => x.i_h_ao_meaning_group_id,
                        principalTable: "i_h_ao_meaning_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_topic",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    level = table.Column<short>(type: "smallint", nullable: false),
                    lft = table.Column<int>(type: "integer", nullable: false),
                    rgt = table.Column<int>(type: "integer", nullable: false),
                    tree_id = table.Column<int>(type: "integer", nullable: false),
                    i_h_ao_meaning_group_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_topic", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_h_ao_topic_i_h_ao_meaning_group_i_h_ao_meaning_group_id",
                        column: x => x.i_h_ao_meaning_group_id,
                        principalTable: "i_h_ao_meaning_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_i_h_ao_topic_i_h_ao_topic_parent_id",
                        column: x => x.parent_id,
                        principalTable: "i_h_ao_topic",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_narrator_link",
                columns: table => new
                {
                    teacher_id = table.Column<int>(type: "integer", nullable: false),
                    student_id = table.Column<int>(type: "integer", nullable: false),
                    chain_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_narrator_link", x => new { x.teacher_id, x.student_id });
                    table.ForeignKey(
                        name: "FK_i_h_ao_narrator_link_i_h_ao_narrator_student_id",
                        column: x => x.student_id,
                        principalTable: "i_h_ao_narrator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_h_ao_narrator_link_i_h_ao_narrator_teacher_id",
                        column: x => x.teacher_id,
                        principalTable: "i_h_ao_narrator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_scholar_statement",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    i_h_ao_scholar_id = table.Column<int>(type: "integer", nullable: false),
                    i_h_ao_narrator_id = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_scholar_statement", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_h_ao_scholar_statement_i_h_ao_narrator_i_h_ao_narrator_id",
                        column: x => x.i_h_ao_narrator_id,
                        principalTable: "i_h_ao_narrator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_h_ao_scholar_statement_i_h_ao_scholar_i_h_ao_scholar_id",
                        column: x => x.i_h_ao_scholar_id,
                        principalTable: "i_h_ao_scholar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_hadith",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    i_h_ao_book_id = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    page = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    grade = table.Column<short>(type: "smallint", nullable: false),
                    i_h_ao_meaning_group_id = table.Column<int>(type: "integer", nullable: true),
                    takhrij = table.Column<int>(type: "integer", nullable: true),
                    taraf_text = table.Column<string>(type: "text", nullable: true),
                    text = table.Column<string>(type: "text", nullable: false),
                    matn_start = table.Column<int>(type: "integer", nullable: true),
                    matn_end = table.Column<int>(type: "integer", nullable: true),
                    search_text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_hadith", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_h_ao_hadith_i_h_ao_book_i_h_ao_book_id",
                        column: x => x.i_h_ao_book_id,
                        principalTable: "i_h_ao_book",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_h_ao_hadith_i_h_ao_meaning_group_i_h_ao_meaning_group_id",
                        column: x => x.i_h_ao_meaning_group_id,
                        principalTable: "i_h_ao_meaning_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_hadith_narrator_mention",
                columns: table => new
                {
                    sequence = table.Column<short>(type: "smallint", nullable: false),
                    i_h_ao_hadith_id = table.Column<int>(type: "integer", nullable: false),
                    i_h_ao_narrator_id = table.Column<int>(type: "integer", nullable: true),
                    start = table.Column<int>(type: "integer", nullable: false),
                    end = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_hadith_narrator_mention", x => new { x.i_h_ao_hadith_id, x.sequence });
                    table.ForeignKey(
                        name: "FK_i_h_ao_hadith_narrator_mention_i_h_ao_hadith_i_h_ao_hadith_~",
                        column: x => x.i_h_ao_hadith_id,
                        principalTable: "i_h_ao_hadith",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_h_ao_hadith_narrator_mention_i_h_ao_narrator_i_h_ao_narra~",
                        column: x => x.i_h_ao_narrator_id,
                        principalTable: "i_h_ao_narrator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_hadith_quran_quote",
                columns: table => new
                {
                    sequence = table.Column<short>(type: "smallint", nullable: false),
                    i_h_ao_hadith_id = table.Column<int>(type: "integer", nullable: false),
                    start = table.Column<int>(type: "integer", nullable: false),
                    end = table.Column<int>(type: "integer", nullable: false),
                    chapter = table.Column<short>(type: "smallint", nullable: true),
                    verse_start = table.Column<short>(type: "smallint", nullable: true),
                    verse_end = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_hadith_quran_quote", x => new { x.i_h_ao_hadith_id, x.sequence });
                    table.ForeignKey(
                        name: "FK_i_h_ao_hadith_quran_quote_i_h_ao_hadith_i_h_ao_hadith_id",
                        column: x => x.i_h_ao_hadith_id,
                        principalTable: "i_h_ao_hadith",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_sanad",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    i_h_ao_hadith_id = table.Column<int>(type: "integer", nullable: false),
                    grade = table.Column<short>(type: "smallint", nullable: false),
                    verdict = table.Column<string>(type: "text", nullable: false),
                    i_h_ao_meaning_group_id = table.Column<int>(type: "integer", nullable: true),
                    takhrij = table.Column<int>(type: "integer", nullable: true),
                    max_rank = table.Column<short>(type: "smallint", nullable: true),
                    length = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_sanad", x => x.id);
                    table.ForeignKey(
                        name: "FK_i_h_ao_sanad_i_h_ao_hadith_i_h_ao_hadith_id",
                        column: x => x.i_h_ao_hadith_id,
                        principalTable: "i_h_ao_hadith",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_h_ao_sanad_i_h_ao_meaning_group_i_h_ao_meaning_group_id",
                        column: x => x.i_h_ao_meaning_group_id,
                        principalTable: "i_h_ao_meaning_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "i_h_ao_sanad_narrator",
                columns: table => new
                {
                    position = table.Column<short>(type: "smallint", nullable: false),
                    i_h_ao_sanad_id = table.Column<int>(type: "integer", nullable: false),
                    i_h_ao_narrator_id = table.Column<int>(type: "integer", nullable: false),
                    i_h_ao_hadith_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_i_h_ao_sanad_narrator", x => new { x.i_h_ao_sanad_id, x.position });
                    table.ForeignKey(
                        name: "FK_i_h_ao_sanad_narrator_i_h_ao_hadith_i_h_ao_hadith_id",
                        column: x => x.i_h_ao_hadith_id,
                        principalTable: "i_h_ao_hadith",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_i_h_ao_sanad_narrator_i_h_ao_narrator_i_h_ao_narrator_id",
                        column: x => x.i_h_ao_narrator_id,
                        principalTable: "i_h_ao_narrator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_i_h_ao_sanad_narrator_i_h_ao_sanad_i_h_ao_sanad_id",
                        column: x => x.i_h_ao_sanad_id,
                        principalTable: "i_h_ao_sanad",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_book_i_h_ao_book_category_id",
                table: "i_h_ao_book",
                column: "i_h_ao_book_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_hadith_i_h_ao_book_id_number",
                table: "i_h_ao_hadith",
                columns: new[] { "i_h_ao_book_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_hadith_i_h_ao_meaning_group_id_takhrij",
                table: "i_h_ao_hadith",
                columns: new[] { "i_h_ao_meaning_group_id", "takhrij" });

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_hadith_search_text_fts",
                table: "i_h_ao_hadith",
                column: "search_text")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "simple");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_hadith_narrator_mention_i_h_ao_narrator_id",
                table: "i_h_ao_hadith_narrator_mention",
                column: "i_h_ao_narrator_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_hadith_quran_quote_chapter_verse_start_verse_end",
                table: "i_h_ao_hadith_quran_quote",
                columns: new[] { "chapter", "verse_start", "verse_end" });

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_meaning_group_grade",
                table: "i_h_ao_meaning_group",
                column: "grade");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_narrator_hadith_count",
                table: "i_h_ao_narrator",
                column: "hadith_count");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_narrator_search_name_trgm",
                table: "i_h_ao_narrator",
                column: "search_name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_narrator_link_student_id",
                table: "i_h_ao_narrator_link",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_sanad_i_h_ao_hadith_id",
                table: "i_h_ao_sanad",
                column: "i_h_ao_hadith_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_sanad_i_h_ao_meaning_group_id",
                table: "i_h_ao_sanad",
                column: "i_h_ao_meaning_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_sanad_narrator_i_h_ao_hadith_id",
                table: "i_h_ao_sanad_narrator",
                column: "i_h_ao_hadith_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_sanad_narrator_i_h_ao_narrator_id_i_h_ao_hadith_id",
                table: "i_h_ao_sanad_narrator",
                columns: new[] { "i_h_ao_narrator_id", "i_h_ao_hadith_id" });

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_scholar_statement_i_h_ao_narrator_id",
                table: "i_h_ao_scholar_statement",
                column: "i_h_ao_narrator_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_scholar_statement_i_h_ao_scholar_id",
                table: "i_h_ao_scholar_statement",
                column: "i_h_ao_scholar_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_taraf_i_h_ao_meaning_group_id",
                table: "i_h_ao_taraf",
                column: "i_h_ao_meaning_group_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_topic_i_h_ao_meaning_group_id",
                table: "i_h_ao_topic",
                column: "i_h_ao_meaning_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_topic_parent_id",
                table: "i_h_ao_topic",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_i_h_ao_topic_tree_id_lft",
                table: "i_h_ao_topic",
                columns: new[] { "tree_id", "lft" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "i_h_ao_hadith_narrator_mention");

            migrationBuilder.DropTable(
                name: "i_h_ao_hadith_quran_quote");

            migrationBuilder.DropTable(
                name: "i_h_ao_narrator_link");

            migrationBuilder.DropTable(
                name: "i_h_ao_sanad_narrator");

            migrationBuilder.DropTable(
                name: "i_h_ao_scholar_statement");

            migrationBuilder.DropTable(
                name: "i_h_ao_taraf");

            migrationBuilder.DropTable(
                name: "i_h_ao_topic");

            migrationBuilder.DropTable(
                name: "i_h_ao_sanad");

            migrationBuilder.DropTable(
                name: "i_h_ao_narrator");

            migrationBuilder.DropTable(
                name: "i_h_ao_scholar");

            migrationBuilder.DropTable(
                name: "i_h_ao_hadith");

            migrationBuilder.DropTable(
                name: "i_h_ao_book");

            migrationBuilder.DropTable(
                name: "i_h_ao_meaning_group");

            migrationBuilder.DropTable(
                name: "i_h_ao_book_category");
        }
    }
}
