
using FluentValidation;
using Ganss.Xss;
using HtmlAgilityPack;
using SixLabors.ImageSharp;

public static class Utility
{
    public static bool IsEffectivelyEmpty<T>(T obj) //TODO: Remove
    {
        var props = typeof(T).GetProperties();

        foreach (var prop in props)
        {
            var value = prop.GetValue(obj);

            if (value is string s)
            {
                if (!string.IsNullOrWhiteSpace(s)) return false;
            }
            else if (value is System.Collections.IEnumerable e && !(value is string))
            {
               
                if (e.Cast<object>().Any()) return false;
            }
            else if (value != null)
            {
                return false;
            }
        }

        return true; // Hepsi null veya boş
    }


    public const string DBTypeDateTime = "datetime";

    public const string DBType8bitInteger = "tinyint";

    public const string DBType16bitInteger = "smallint";

    public const string DBType32bitInteger = "int";

    public const string DBType64bitInteger = "bigint";

    public const string DBTypeVARBINARYMAX = "varbinary(max)";

    public const string DBTypeNVARCHARMAX = "NVARCHAR(MAX)";
    
    public const string DBTypeNVARCHAR1000 = "NVARCHAR(1000)";

    public const string DBTypeNVARCHAR255 = "NVARCHAR(255)";

    public const string DBTypeNVARCHAR250 = "NVARCHAR(250)";

    public const string DBTypeNVARCHAR100 = "NVARCHAR(100)";

    public const string DBTypeNVARCHAR5 = "NVARCHAR(5)";
    
    public const string DBTypeNVARCHAR10 = "NVARCHAR(10)";
    
    public const string DBTypeNVARCHAR30 = "NVARCHAR(30)";

    public const string DBTypeVARCHARMAX = "varchar(max)";

    public const string DBTypeVARCHAR500 = "VARCHAR(500)";

    public const string DBTypeVARCHAR250 = "VARCHAR(250)";

    public const string DBTypeVARCHAR126 = "VARCHAR(126)";

    public const string DBTypeVARCHAR100 = "VARCHAR(100)";

    public const string DBTypeVARCHAR72 = "VARCHAR(72)";

    public const string DBTypeNVARCHAR50 = "NVARCHAR(50)";

    public const string DBTypeVARCHAR50 = "VARCHAR(50)";

    public const string DBTypeVARCHAR32 = "VARCHAR(32)";

    public const string DBTypeVARCHAR24 = "VARCHAR(24)";

    public const string DBTypeVARCHAR16 = "VARCHAR(16)";

    public const string DBTypeVARCHAR5 = "VARCHAR(5)";

    public const string DBTypeVARCHAR2 = "VARCHAR(2)";

    public const string DBTypeCHAR1 = "CHAR(1)";

    public const string DBTypeUUID = "uniqueidentifier";

    public const string DBDefaultUUIDFunction = "NEWID()";

    public const string DBDefaultDateTimeFunction = "GETUTCDATE()";

    public const short MAX_COLLECTION_COUNT = 3;

    public const short MAX_REFLECTION_COUNT_PER_VERSE = 10;

    public const short MAX_REFLECTION_COUNT_PER_NOTE = 10;

    public const short MAX_NOTE_COUNT_PER_VERSE = 3;

    public const short MIN_LENGTH_FOR_ROOT = 1;

    public const short MAX_LENGTH_FOR_ROOT = 5;

    public class ScriptureDataType : Dictionary<short, ScriptureStructure> { }

    public class ScriptureStructure
    {
        public int SectionCount { get; set; }


        public SectionDataType Sections { get; set; } = [];
    }

    public class SectionDataType : Dictionary<short, SectionStructure> { }

    public class SectionStructure
    {
        public int ChapterCount { get; set; }
        public ChapterDataType Chapters { get; set; } = new ChapterDataType();
    }

    public class ChapterDataType : Dictionary<short, ChapterStructure> { }

    public class ChapterStructure
    {
        public int VerseCount { get; set; }
    }

    public static readonly Dictionary<string, int> AvailableScriptures = new()
            {
                { "t", 1 }, // Torah
            };

    public static readonly ScriptureDataType SCRIPTURE_DATA = new()
        {
            {
                1, new ScriptureStructure
                {
                    SectionCount = 39,
                    Sections = new SectionDataType
                    {
                        {
                            1, new SectionStructure
                            {

                                ChapterCount = 50,
                                Chapters = new ChapterDataType
                                    {
                                        { 1, new ChapterStructure { VerseCount = 31 } },
                                        { 2, new ChapterStructure { VerseCount = 25 } },
                                        { 3, new ChapterStructure { VerseCount = 24 } },
                                        { 4, new ChapterStructure { VerseCount = 26 } },
                                        { 5, new ChapterStructure { VerseCount = 32 } },
                                        { 6, new ChapterStructure { VerseCount = 22 } },
                                        { 7, new ChapterStructure { VerseCount = 24 } },
                                        { 8, new ChapterStructure { VerseCount = 22 } },
                                        { 9, new ChapterStructure { VerseCount = 29 } },
                                        { 10, new ChapterStructure { VerseCount = 32 } },
                                        { 11, new ChapterStructure { VerseCount = 32 } },
                                        { 12, new ChapterStructure { VerseCount = 20 } },
                                        { 13, new ChapterStructure { VerseCount = 18 } },
                                        { 14, new ChapterStructure { VerseCount = 24 } },
                                        { 15, new ChapterStructure { VerseCount = 21 } },
                                        { 16, new ChapterStructure { VerseCount = 16 } },
                                        { 17, new ChapterStructure { VerseCount = 27 } },
                                        { 18, new ChapterStructure { VerseCount = 33 } },
                                        { 19, new ChapterStructure { VerseCount = 38 } },
                                        { 20, new ChapterStructure { VerseCount = 18 } },
                                        { 21, new ChapterStructure { VerseCount = 34 } },
                                        { 22, new ChapterStructure { VerseCount = 24 } },
                                        { 23, new ChapterStructure { VerseCount = 20 } },
                                        { 24, new ChapterStructure { VerseCount = 67 } },
                                        { 25, new ChapterStructure { VerseCount = 34 } },
                                        { 26, new ChapterStructure { VerseCount = 35 } },
                                        { 27, new ChapterStructure { VerseCount = 46 } },
                                        { 28, new ChapterStructure { VerseCount = 22 } },
                                        { 29, new ChapterStructure { VerseCount = 35 } },
                                        { 30, new ChapterStructure { VerseCount = 43 } },
                                        { 31, new ChapterStructure { VerseCount = 55 } },
                                        { 32, new ChapterStructure { VerseCount = 32 } },
                                        { 33, new ChapterStructure { VerseCount = 20 } },
                                        { 34, new ChapterStructure { VerseCount = 31 } },
                                        { 35, new ChapterStructure { VerseCount = 29 } },
                                        { 36, new ChapterStructure { VerseCount = 43 } },
                                        { 37, new ChapterStructure { VerseCount = 36 } },
                                        { 38, new ChapterStructure { VerseCount = 30 } },
                                        { 39, new ChapterStructure { VerseCount = 23 } },
                                        { 40, new ChapterStructure { VerseCount = 23 } },
                                        { 41, new ChapterStructure { VerseCount = 57 } },
                                        { 42, new ChapterStructure { VerseCount = 38 } },
                                        { 43, new ChapterStructure { VerseCount = 34 } },
                                        { 44, new ChapterStructure { VerseCount = 34 } },
                                        { 45, new ChapterStructure { VerseCount = 28 } },
                                        { 46, new ChapterStructure { VerseCount = 34 } },
                                        { 47, new ChapterStructure { VerseCount = 31 } },
                                        { 48, new ChapterStructure { VerseCount = 22 } },
                                        { 49, new ChapterStructure { VerseCount = 33 } },
                                        { 50, new ChapterStructure { VerseCount = 26 } }
                                    }
                            }
                        },
                        {
                            2, new SectionStructure
                            {
                                ChapterCount = 40,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 22 } },
                                    { 2, new ChapterStructure { VerseCount = 25 } },
                                    { 3, new ChapterStructure { VerseCount = 22 } },
                                    { 4, new ChapterStructure { VerseCount = 31 } },
                                    { 5, new ChapterStructure { VerseCount = 23 } },
                                    { 6, new ChapterStructure { VerseCount = 30 } },
                                    { 7, new ChapterStructure { VerseCount = 29 } },
                                    { 8, new ChapterStructure { VerseCount = 28 } },
                                    { 9, new ChapterStructure { VerseCount = 35 } },
                                    { 10, new ChapterStructure { VerseCount = 29 } },
                                    { 11, new ChapterStructure { VerseCount = 10 } },
                                    { 12, new ChapterStructure { VerseCount = 51 } },
                                    { 13, new ChapterStructure { VerseCount = 22 } },
                                    { 14, new ChapterStructure { VerseCount = 31 } },
                                    { 15, new ChapterStructure { VerseCount = 27 } },
                                    { 16, new ChapterStructure { VerseCount = 36 } },
                                    { 17, new ChapterStructure { VerseCount = 16 } },
                                    { 18, new ChapterStructure { VerseCount = 27 } },
                                    { 19, new ChapterStructure { VerseCount = 25 } },
                                    { 20, new ChapterStructure { VerseCount = 26 } },
                                    { 21, new ChapterStructure { VerseCount = 37 } },
                                    { 22, new ChapterStructure { VerseCount = 30 } },
                                    { 23, new ChapterStructure { VerseCount = 33 } },
                                    { 24, new ChapterStructure { VerseCount = 18 } },
                                    { 25, new ChapterStructure { VerseCount = 40 } },
                                    { 26, new ChapterStructure { VerseCount = 37 } },
                                    { 27, new ChapterStructure { VerseCount = 21 } },
                                    { 28, new ChapterStructure { VerseCount = 43 } },
                                    { 29, new ChapterStructure { VerseCount = 46 } },
                                    { 30, new ChapterStructure { VerseCount = 38 } },
                                    { 31, new ChapterStructure { VerseCount = 18 } },
                                    { 32, new ChapterStructure { VerseCount = 35 } },
                                    { 33, new ChapterStructure { VerseCount = 23 } },
                                    { 34, new ChapterStructure { VerseCount = 35 } },
                                    { 35, new ChapterStructure { VerseCount = 35 } },
                                    { 36, new ChapterStructure { VerseCount = 38 } },
                                    { 37, new ChapterStructure { VerseCount = 29 } },
                                    { 38, new ChapterStructure { VerseCount = 31 } },
                                    { 39, new ChapterStructure { VerseCount = 43 } },
                                    { 40, new ChapterStructure { VerseCount = 38 } },
                                }
                            }
                        },
                        {
                            3, new SectionStructure
                            {
                                ChapterCount = 27,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 17 } },
                                    { 2, new ChapterStructure { VerseCount = 16 } },
                                    { 3, new ChapterStructure { VerseCount = 17 } },
                                    { 4, new ChapterStructure { VerseCount = 35 } },
                                    { 5, new ChapterStructure { VerseCount = 26 } },
                                    { 6, new ChapterStructure { VerseCount = 23 } },
                                    { 7, new ChapterStructure { VerseCount = 38 } },
                                    { 8, new ChapterStructure { VerseCount = 36 } },
                                    { 9, new ChapterStructure { VerseCount = 24 } },
                                    { 10, new ChapterStructure { VerseCount = 20 } },
                                    { 11, new ChapterStructure { VerseCount = 47 } },
                                    { 12, new ChapterStructure { VerseCount = 8 } },
                                    { 13, new ChapterStructure { VerseCount = 59 } },
                                    { 14, new ChapterStructure { VerseCount = 57 } },
                                    { 15, new ChapterStructure { VerseCount = 33 } },
                                    { 16, new ChapterStructure { VerseCount = 34 } },
                                    { 17, new ChapterStructure { VerseCount = 16 } },
                                    { 18, new ChapterStructure { VerseCount = 30 } },
                                    { 19, new ChapterStructure { VerseCount = 37 } },
                                    { 20, new ChapterStructure { VerseCount = 27 } },
                                    { 21, new ChapterStructure { VerseCount = 24 } },
                                    { 22, new ChapterStructure { VerseCount = 33 } },
                                    { 23, new ChapterStructure { VerseCount = 44 } },
                                    { 24, new ChapterStructure { VerseCount = 23 } },
                                    { 25, new ChapterStructure { VerseCount = 55 } },
                                    { 26, new ChapterStructure { VerseCount = 46 } },
                                    { 27, new ChapterStructure { VerseCount = 34 } },
                                },
                            }
                        },
                        {
                            4, new SectionStructure
                            {
                                ChapterCount = 36,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 54 } },
                                    { 2, new ChapterStructure { VerseCount = 34 } },
                                    { 3, new ChapterStructure { VerseCount = 51 } },
                                    { 4, new ChapterStructure { VerseCount = 49 } },
                                    { 5, new ChapterStructure { VerseCount = 31 } },
                                    { 6, new ChapterStructure { VerseCount = 27 } },
                                    { 7, new ChapterStructure { VerseCount = 89 } },
                                    { 8, new ChapterStructure { VerseCount = 26 } },
                                    { 9, new ChapterStructure { VerseCount = 23 } },
                                    { 10, new ChapterStructure { VerseCount = 36 } },
                                    { 11, new ChapterStructure { VerseCount = 35 } },
                                    { 12, new ChapterStructure { VerseCount = 16 } },
                                    { 13, new ChapterStructure { VerseCount = 33 } },
                                    { 14, new ChapterStructure { VerseCount = 45 } },
                                    { 15, new ChapterStructure { VerseCount = 41 } },
                                    { 16, new ChapterStructure { VerseCount = 35 } },
                                    { 17, new ChapterStructure { VerseCount = 28 } },
                                    { 18, new ChapterStructure { VerseCount = 32 } },
                                    { 19, new ChapterStructure { VerseCount = 22 } },
                                    { 20, new ChapterStructure { VerseCount = 29 } },
                                    { 21, new ChapterStructure { VerseCount = 35 } },
                                    { 22, new ChapterStructure { VerseCount = 41 } },
                                    { 23, new ChapterStructure { VerseCount = 30 } },
                                    { 24, new ChapterStructure { VerseCount = 25 } },
                                    { 25, new ChapterStructure { VerseCount = 18 } },
                                    { 26, new ChapterStructure { VerseCount = 65 } },
                                    { 27, new ChapterStructure { VerseCount = 23 } },
                                    { 28, new ChapterStructure { VerseCount = 31 } },
                                    { 29, new ChapterStructure { VerseCount = 39 } },
                                    { 30, new ChapterStructure { VerseCount = 17 } },
                                    { 31, new ChapterStructure { VerseCount = 54 } },
                                    { 32, new ChapterStructure { VerseCount = 42 } },
                                    { 33, new ChapterStructure { VerseCount = 56 } },
                                    { 34, new ChapterStructure { VerseCount = 29 } },
                                    { 35, new ChapterStructure { VerseCount = 34 } },
                                    { 36, new ChapterStructure { VerseCount = 13 } },
                                }
                            }
                        },
                        {
                            5, new SectionStructure
                            {
                                ChapterCount = 34,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 46 } },
                                    { 2, new ChapterStructure { VerseCount = 37 } },
                                    { 3, new ChapterStructure { VerseCount = 29 } },
                                    { 4, new ChapterStructure { VerseCount = 49 } },
                                    { 5, new ChapterStructure { VerseCount = 30 } },
                                    { 6, new ChapterStructure { VerseCount = 25 } },
                                    { 7, new ChapterStructure { VerseCount = 26 } },
                                    { 8, new ChapterStructure { VerseCount = 20 } },
                                    { 9, new ChapterStructure { VerseCount = 29 } },
                                    { 10, new ChapterStructure { VerseCount = 22 } },
                                    { 11, new ChapterStructure { VerseCount = 32 } },
                                    { 12, new ChapterStructure { VerseCount = 31 } },
                                    { 13, new ChapterStructure { VerseCount = 19 } },
                                    { 14, new ChapterStructure { VerseCount = 29 } },
                                    { 15, new ChapterStructure { VerseCount = 23 } },
                                    { 16, new ChapterStructure { VerseCount = 22 } },
                                    { 17, new ChapterStructure { VerseCount = 20 } },
                                    { 18, new ChapterStructure { VerseCount = 22 } },
                                    { 19, new ChapterStructure { VerseCount = 21 } },
                                    { 20, new ChapterStructure { VerseCount = 20 } },
                                    { 21, new ChapterStructure { VerseCount = 23 } },
                                    { 22, new ChapterStructure { VerseCount = 29 } },
                                    { 23, new ChapterStructure { VerseCount = 26 } },
                                    { 24, new ChapterStructure { VerseCount = 22 } },
                                    { 25, new ChapterStructure { VerseCount = 19 } },
                                    { 26, new ChapterStructure { VerseCount = 19 } },
                                    { 27, new ChapterStructure { VerseCount = 26 } },
                                    { 28, new ChapterStructure { VerseCount = 69 } },
                                    { 29, new ChapterStructure { VerseCount = 28 } },
                                    { 30, new ChapterStructure { VerseCount = 20 } },
                                    { 31, new ChapterStructure { VerseCount = 30 } },
                                    { 32, new ChapterStructure { VerseCount = 52 } },
                                    { 33, new ChapterStructure { VerseCount = 29 } },
                                    { 34, new ChapterStructure { VerseCount = 12 } },
                                }
                            }
                        },
                        {
                            6, new SectionStructure
                            {
                                ChapterCount = 24,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 18 } },
                                    { 2, new ChapterStructure { VerseCount = 24 } },
                                    { 3, new ChapterStructure { VerseCount = 17 } },
                                    { 4, new ChapterStructure { VerseCount = 24 } },
                                    { 5, new ChapterStructure { VerseCount = 15 } },
                                    { 6, new ChapterStructure { VerseCount = 27 } },
                                    { 7, new ChapterStructure { VerseCount = 26 } },
                                    { 8, new ChapterStructure { VerseCount = 35 } },
                                    { 9, new ChapterStructure { VerseCount = 27 } },
                                    { 10, new ChapterStructure { VerseCount = 43 } },
                                    { 11, new ChapterStructure { VerseCount = 23 } },
                                    { 12, new ChapterStructure { VerseCount = 24 } },
                                    { 13, new ChapterStructure { VerseCount = 33 } },
                                    { 14, new ChapterStructure { VerseCount = 15 } },
                                    { 15, new ChapterStructure { VerseCount = 63 } },
                                    { 16, new ChapterStructure { VerseCount = 10 } },
                                    { 17, new ChapterStructure { VerseCount = 18 } },
                                    { 18, new ChapterStructure { VerseCount = 28 } },
                                    { 19, new ChapterStructure { VerseCount = 51 } },
                                    { 20, new ChapterStructure { VerseCount = 9 } },
                                    { 21, new ChapterStructure { VerseCount = 45 } },
                                    { 22, new ChapterStructure { VerseCount = 34 } },
                                    { 23, new ChapterStructure { VerseCount = 16 } },
                                    { 24, new ChapterStructure { VerseCount = 33 } },
                                }
                            }
                        },
                        {
                            7, new SectionStructure
                            {
                                ChapterCount = 21,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 36 } },
                                    { 2, new ChapterStructure { VerseCount = 23 } },
                                    { 3, new ChapterStructure { VerseCount = 31 } },
                                    { 4, new ChapterStructure { VerseCount = 24 } },
                                    { 5, new ChapterStructure { VerseCount = 31 } },
                                    { 6, new ChapterStructure { VerseCount = 40 } },
                                    { 7, new ChapterStructure { VerseCount = 25 } },
                                    { 8, new ChapterStructure { VerseCount = 35 } },
                                    { 9, new ChapterStructure { VerseCount = 57 } },
                                    { 10, new ChapterStructure { VerseCount = 18 } },
                                    { 11, new ChapterStructure { VerseCount = 40 } },
                                    { 12, new ChapterStructure { VerseCount = 15 } },
                                    { 13, new ChapterStructure { VerseCount = 25 } },
                                    { 14, new ChapterStructure { VerseCount = 20 } },
                                    { 15, new ChapterStructure { VerseCount = 20 } },
                                    { 16, new ChapterStructure { VerseCount = 31 } },
                                    { 17, new ChapterStructure { VerseCount = 13 } },
                                    { 18, new ChapterStructure { VerseCount = 31 } },
                                    { 19, new ChapterStructure { VerseCount = 30 } },
                                    { 20, new ChapterStructure { VerseCount = 48 } },
                                    { 21, new ChapterStructure { VerseCount = 25 } },
                                }
                            }
                        },
                        {
                            8, new SectionStructure
                            {
                                ChapterCount = 31,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 28 } },
                                    { 2, new ChapterStructure { VerseCount = 36 } },
                                    { 3, new ChapterStructure { VerseCount = 21 } },
                                    { 4, new ChapterStructure { VerseCount = 22 } },
                                    { 5, new ChapterStructure { VerseCount = 12 } },
                                    { 6, new ChapterStructure { VerseCount = 21 } },
                                    { 7, new ChapterStructure { VerseCount = 17 } },
                                    { 8, new ChapterStructure { VerseCount = 22 } },
                                    { 9, new ChapterStructure { VerseCount = 27 } },
                                    { 10, new ChapterStructure { VerseCount = 27 } },
                                    { 11, new ChapterStructure { VerseCount = 15 } },
                                    { 12, new ChapterStructure { VerseCount = 25 } },
                                    { 13, new ChapterStructure { VerseCount = 23 } },
                                    { 14, new ChapterStructure { VerseCount = 52 } },
                                    { 15, new ChapterStructure { VerseCount = 35 } },
                                    { 16, new ChapterStructure { VerseCount = 23 } },
                                    { 17, new ChapterStructure { VerseCount = 58 } },
                                    { 18, new ChapterStructure { VerseCount = 30 } },
                                    { 19, new ChapterStructure { VerseCount = 24 } },
                                    { 20, new ChapterStructure { VerseCount = 42 } },
                                    { 21, new ChapterStructure { VerseCount = 16 } },
                                    { 22, new ChapterStructure { VerseCount = 23 } },
                                    { 23, new ChapterStructure { VerseCount = 28 } },
                                    { 24, new ChapterStructure { VerseCount = 23 } },
                                    { 25, new ChapterStructure { VerseCount = 44 } },
                                    { 26, new ChapterStructure { VerseCount = 25 } },
                                    { 27, new ChapterStructure { VerseCount = 12 } },
                                    { 28, new ChapterStructure { VerseCount = 25 } },
                                    { 29, new ChapterStructure { VerseCount = 11 } },
                                    { 30, new ChapterStructure { VerseCount = 31 } },
                                    { 31, new ChapterStructure { VerseCount = 13 } },
                                }
                            }
                        },
                        {
                            9, new SectionStructure
                            {
                                ChapterCount = 24,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 27 } },
                                    { 2, new ChapterStructure { VerseCount = 32 } },
                                    { 3, new ChapterStructure { VerseCount = 39 } },
                                    { 4, new ChapterStructure { VerseCount = 12 } },
                                    { 5, new ChapterStructure { VerseCount = 25 } },
                                    { 6, new ChapterStructure { VerseCount = 23 } },
                                    { 7, new ChapterStructure { VerseCount = 29 } },
                                    { 8, new ChapterStructure { VerseCount = 18 } },
                                    { 9, new ChapterStructure { VerseCount = 13 } },
                                    { 10, new ChapterStructure { VerseCount = 19 } },
                                    { 11, new ChapterStructure { VerseCount = 27 } },
                                    { 12, new ChapterStructure { VerseCount = 31 } },
                                    { 13, new ChapterStructure { VerseCount = 39 } },
                                    { 14, new ChapterStructure { VerseCount = 33 } },
                                    { 15, new ChapterStructure { VerseCount = 37 } },
                                    { 16, new ChapterStructure { VerseCount = 23 } },
                                    { 17, new ChapterStructure { VerseCount = 29 } },
                                    { 18, new ChapterStructure { VerseCount = 32 } },
                                    { 19, new ChapterStructure { VerseCount = 44 } },
                                    { 20, new ChapterStructure { VerseCount = 26 } },
                                    { 21, new ChapterStructure { VerseCount = 22 } },
                                    { 22, new ChapterStructure { VerseCount = 51 } },
                                    { 23, new ChapterStructure { VerseCount = 39 } },
                                    { 24, new ChapterStructure { VerseCount = 25 } },
                                }
                            }
                        },
                        {
                            10, new SectionStructure
                            {
                                ChapterCount = 22,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 53 } },
                                    { 2, new ChapterStructure { VerseCount = 46 } },
                                    { 3, new ChapterStructure { VerseCount = 28 } },
                                    { 4, new ChapterStructure { VerseCount = 20 } },
                                    { 5, new ChapterStructure { VerseCount = 32 } },
                                    { 6, new ChapterStructure { VerseCount = 38 } },
                                    { 7, new ChapterStructure { VerseCount = 51 } },
                                    { 8, new ChapterStructure { VerseCount = 66 } },
                                    { 9, new ChapterStructure { VerseCount = 28 } },
                                    { 10, new ChapterStructure { VerseCount = 29 } },
                                    { 11, new ChapterStructure { VerseCount = 43 } },
                                    { 12, new ChapterStructure { VerseCount = 33 } },
                                    { 13, new ChapterStructure { VerseCount = 34 } },
                                    { 14, new ChapterStructure { VerseCount = 31 } },
                                    { 15, new ChapterStructure { VerseCount = 34 } },
                                    { 16, new ChapterStructure { VerseCount = 34 } },
                                    { 17, new ChapterStructure { VerseCount = 24 } },
                                    { 18, new ChapterStructure { VerseCount = 46 } },
                                    { 19, new ChapterStructure { VerseCount = 21 } },
                                    { 20, new ChapterStructure { VerseCount = 43 } },
                                    { 21, new ChapterStructure { VerseCount = 29 } },
                                    { 22, new ChapterStructure { VerseCount = 54 } },
                                }
                            }
                        },
                        {
                            11, new SectionStructure
                            {
                                ChapterCount = 25,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 18 } },
                                    { 2, new ChapterStructure { VerseCount = 25 } },
                                    { 3, new ChapterStructure { VerseCount = 27 } },
                                    { 4, new ChapterStructure { VerseCount = 44 } },
                                    { 5, new ChapterStructure { VerseCount = 27 } },
                                    { 6, new ChapterStructure { VerseCount = 33 } },
                                    { 7, new ChapterStructure { VerseCount = 20 } },
                                    { 8, new ChapterStructure { VerseCount = 29 } },
                                    { 9, new ChapterStructure { VerseCount = 37 } },
                                    { 10, new ChapterStructure { VerseCount = 36 } },
                                    { 11, new ChapterStructure { VerseCount = 20 } },
                                    { 12, new ChapterStructure { VerseCount = 22 } },
                                    { 13, new ChapterStructure { VerseCount = 25 } },
                                    { 14, new ChapterStructure { VerseCount = 29 } },
                                    { 15, new ChapterStructure { VerseCount = 38 } },
                                    { 16, new ChapterStructure { VerseCount = 20 } },
                                    { 17, new ChapterStructure { VerseCount = 41 } },
                                    { 18, new ChapterStructure { VerseCount = 37 } },
                                    { 19, new ChapterStructure { VerseCount = 37 } },
                                    { 20, new ChapterStructure { VerseCount = 21 } },
                                    { 21, new ChapterStructure { VerseCount = 26 } },
                                    { 22, new ChapterStructure { VerseCount = 20 } },
                                    { 23, new ChapterStructure { VerseCount = 37 } },
                                    { 24, new ChapterStructure { VerseCount = 20 } },
                                    { 25, new ChapterStructure { VerseCount = 30 } },
                                }
                            }
                        },
                        {
                            12, new SectionStructure
                            {
                                ChapterCount = 66,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 31 } },
                                    { 2, new ChapterStructure { VerseCount = 22 } },
                                    { 3, new ChapterStructure { VerseCount = 26 } },
                                    { 4, new ChapterStructure { VerseCount = 6 } },
                                    { 5, new ChapterStructure { VerseCount = 30 } },
                                    { 6, new ChapterStructure { VerseCount = 13 } },
                                    { 7, new ChapterStructure { VerseCount = 25 } },
                                    { 8, new ChapterStructure { VerseCount = 23 } },
                                    { 9, new ChapterStructure { VerseCount = 20 } },
                                    { 10, new ChapterStructure { VerseCount = 34 } },
                                    { 11, new ChapterStructure { VerseCount = 16 } },
                                    { 12, new ChapterStructure { VerseCount = 6 } },
                                    { 13, new ChapterStructure { VerseCount = 22 } },
                                    { 14, new ChapterStructure { VerseCount = 32 } },
                                    { 15, new ChapterStructure { VerseCount = 9 } },
                                    { 16, new ChapterStructure { VerseCount = 14 } },
                                    { 17, new ChapterStructure { VerseCount = 14 } },
                                    { 18, new ChapterStructure { VerseCount = 7 } },
                                    { 19, new ChapterStructure { VerseCount = 25 } },
                                    { 20, new ChapterStructure { VerseCount = 6 } },
                                    { 21, new ChapterStructure { VerseCount = 17 } },
                                    { 22, new ChapterStructure { VerseCount = 25 } },
                                    { 23, new ChapterStructure { VerseCount = 18 } },
                                    { 24, new ChapterStructure { VerseCount = 23 } },
                                    { 25, new ChapterStructure { VerseCount = 12 } },
                                    { 26, new ChapterStructure { VerseCount = 21 } },
                                    { 27, new ChapterStructure { VerseCount = 13 } },
                                    { 28, new ChapterStructure { VerseCount = 29 } },
                                    { 29, new ChapterStructure { VerseCount = 24 } },
                                    { 30, new ChapterStructure { VerseCount = 33 } },
                                    { 31, new ChapterStructure { VerseCount = 9 } },
                                    { 32, new ChapterStructure { VerseCount = 20 } },
                                    { 33, new ChapterStructure { VerseCount = 24 } },
                                    { 34, new ChapterStructure { VerseCount = 17 } },
                                    { 35, new ChapterStructure { VerseCount = 10 } },
                                    { 36, new ChapterStructure { VerseCount = 22 } },
                                    { 37, new ChapterStructure { VerseCount = 38 } },
                                    { 38, new ChapterStructure { VerseCount = 22 } },
                                    { 39, new ChapterStructure { VerseCount = 8 } },
                                    { 40, new ChapterStructure { VerseCount = 31 } },
                                    { 41, new ChapterStructure { VerseCount = 29 } },
                                    { 42, new ChapterStructure { VerseCount = 25 } },
                                    { 43, new ChapterStructure { VerseCount = 28 } },
                                    { 44, new ChapterStructure { VerseCount = 28 } },
                                    { 45, new ChapterStructure { VerseCount = 25 } },
                                    { 46, new ChapterStructure { VerseCount = 13 } },
                                    { 47, new ChapterStructure { VerseCount = 15 } },
                                    { 48, new ChapterStructure { VerseCount = 22 } },
                                    { 49, new ChapterStructure { VerseCount = 26 } },
                                    { 50, new ChapterStructure { VerseCount = 11 } },
                                    { 51, new ChapterStructure { VerseCount = 23 } },
                                    { 52, new ChapterStructure { VerseCount = 15 } },
                                    { 53, new ChapterStructure { VerseCount = 12 } },
                                    { 54, new ChapterStructure { VerseCount = 17 } },
                                    { 55, new ChapterStructure { VerseCount = 13 } },
                                    { 56, new ChapterStructure { VerseCount = 12 } },
                                    { 57, new ChapterStructure { VerseCount = 21 } },
                                    { 58, new ChapterStructure { VerseCount = 14 } },
                                    { 59, new ChapterStructure { VerseCount = 21 } },
                                    { 60, new ChapterStructure { VerseCount = 22 } },
                                    { 61, new ChapterStructure { VerseCount = 11 } },
                                    { 62, new ChapterStructure { VerseCount = 12 } },
                                    { 63, new ChapterStructure { VerseCount = 19 } },
                                    { 64, new ChapterStructure { VerseCount = 11 } },
                                    { 65, new ChapterStructure { VerseCount = 25 } },
                                    { 66, new ChapterStructure { VerseCount = 24 } },
                                }
                            }
                        },
                        {
                            13, new SectionStructure
                            {
                                ChapterCount = 52,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 19 } },
                                    { 2, new ChapterStructure { VerseCount = 37 } },
                                    { 3, new ChapterStructure { VerseCount = 25 } },
                                    { 4, new ChapterStructure { VerseCount = 31 } },
                                    { 5, new ChapterStructure { VerseCount = 31 } },
                                    { 6, new ChapterStructure { VerseCount = 30 } },
                                    { 7, new ChapterStructure { VerseCount = 34 } },
                                    { 8, new ChapterStructure { VerseCount = 23 } },
                                    { 9, new ChapterStructure { VerseCount = 25 } },
                                    { 10, new ChapterStructure { VerseCount = 25 } },
                                    { 11, new ChapterStructure { VerseCount = 23 } },
                                    { 12, new ChapterStructure { VerseCount = 17 } },
                                    { 13, new ChapterStructure { VerseCount = 27 } },
                                    { 14, new ChapterStructure { VerseCount = 22 } },
                                    { 15, new ChapterStructure { VerseCount = 21 } },
                                    { 16, new ChapterStructure { VerseCount = 21 } },
                                    { 17, new ChapterStructure { VerseCount = 27 } },
                                    { 18, new ChapterStructure { VerseCount = 23 } },
                                    { 19, new ChapterStructure { VerseCount = 15 } },
                                    { 20, new ChapterStructure { VerseCount = 18 } },
                                    { 21, new ChapterStructure { VerseCount = 14 } },
                                    { 22, new ChapterStructure { VerseCount = 30 } },
                                    { 23, new ChapterStructure { VerseCount = 40 } },
                                    { 24, new ChapterStructure { VerseCount = 10 } },
                                    { 25, new ChapterStructure { VerseCount = 38 } },
                                    { 26, new ChapterStructure { VerseCount = 24 } },
                                    { 27, new ChapterStructure { VerseCount = 22 } },
                                    { 28, new ChapterStructure { VerseCount = 17 } },
                                    { 29, new ChapterStructure { VerseCount = 32 } },
                                    { 30, new ChapterStructure { VerseCount = 24 } },
                                    { 31, new ChapterStructure { VerseCount = 40 } },
                                    { 32, new ChapterStructure { VerseCount = 44 } },
                                    { 33, new ChapterStructure { VerseCount = 26 } },
                                    { 34, new ChapterStructure { VerseCount = 22 } },
                                    { 35, new ChapterStructure { VerseCount = 19 } },
                                    { 36, new ChapterStructure { VerseCount = 32 } },
                                    { 37, new ChapterStructure { VerseCount = 21 } },
                                    { 38, new ChapterStructure { VerseCount = 28 } },
                                    { 39, new ChapterStructure { VerseCount = 18 } },
                                    { 40, new ChapterStructure { VerseCount = 16 } },
                                    { 41, new ChapterStructure { VerseCount = 18 } },
                                    { 42, new ChapterStructure { VerseCount = 22 } },
                                    { 43, new ChapterStructure { VerseCount = 13 } },
                                    { 44, new ChapterStructure { VerseCount = 30 } },
                                    { 45, new ChapterStructure { VerseCount = 5 } },
                                    { 46, new ChapterStructure { VerseCount = 28 } },
                                    { 47, new ChapterStructure { VerseCount = 7 } },
                                    { 48, new ChapterStructure { VerseCount = 47 } },
                                    { 49, new ChapterStructure { VerseCount = 39 } },
                                    { 50, new ChapterStructure { VerseCount = 46 } },
                                    { 51, new ChapterStructure { VerseCount = 64 } },
                                    { 52, new ChapterStructure { VerseCount = 34 } },
                                }
                            }
                        },
                        {
                            14, new SectionStructure
                            {
                                ChapterCount = 48,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 28 } },
                                    { 2, new ChapterStructure { VerseCount = 10 } },
                                    { 3, new ChapterStructure { VerseCount = 27 } },
                                    { 4, new ChapterStructure { VerseCount = 17 } },
                                    { 5, new ChapterStructure { VerseCount = 17 } },
                                    { 6, new ChapterStructure { VerseCount = 14 } },
                                    { 7, new ChapterStructure { VerseCount = 27 } },
                                    { 8, new ChapterStructure { VerseCount = 18 } },
                                    { 9, new ChapterStructure { VerseCount = 11 } },
                                    { 10, new ChapterStructure { VerseCount = 22 } },
                                    { 11, new ChapterStructure { VerseCount = 25 } },
                                    { 12, new ChapterStructure { VerseCount = 28 } },
                                    { 13, new ChapterStructure { VerseCount = 23 } },
                                    { 14, new ChapterStructure { VerseCount = 23 } },
                                    { 15, new ChapterStructure { VerseCount = 8 } },
                                    { 16, new ChapterStructure { VerseCount = 63 } },
                                    { 17, new ChapterStructure { VerseCount = 24 } },
                                    { 18, new ChapterStructure { VerseCount = 32 } },
                                    { 19, new ChapterStructure { VerseCount = 14 } },
                                    { 20, new ChapterStructure { VerseCount = 44 } },
                                    { 21, new ChapterStructure { VerseCount = 37 } },
                                    { 22, new ChapterStructure { VerseCount = 31 } },
                                    { 23, new ChapterStructure { VerseCount = 49 } },
                                    { 24, new ChapterStructure { VerseCount = 27 } },
                                    { 25, new ChapterStructure { VerseCount = 17 } },
                                    { 26, new ChapterStructure { VerseCount = 21 } },
                                    { 27, new ChapterStructure { VerseCount = 36 } },
                                    { 28, new ChapterStructure { VerseCount = 26 } },
                                    { 29, new ChapterStructure { VerseCount = 21 } },
                                    { 30, new ChapterStructure { VerseCount = 26 } },
                                    { 31, new ChapterStructure { VerseCount = 18 } },
                                    { 32, new ChapterStructure { VerseCount = 32 } },
                                    { 33, new ChapterStructure { VerseCount = 33 } },
                                    { 34, new ChapterStructure { VerseCount = 31 } },
                                    { 35, new ChapterStructure { VerseCount = 15 } },
                                    { 36, new ChapterStructure { VerseCount = 38 } },
                                    { 37, new ChapterStructure { VerseCount = 28 } },
                                    { 38, new ChapterStructure { VerseCount = 23 } },
                                    { 39, new ChapterStructure { VerseCount = 29 } },
                                    { 40, new ChapterStructure { VerseCount = 49 } },
                                    { 41, new ChapterStructure { VerseCount = 26 } },
                                    { 42, new ChapterStructure { VerseCount = 20 } },
                                    { 43, new ChapterStructure { VerseCount = 27 } },
                                    { 44, new ChapterStructure { VerseCount = 31 } },
                                    { 45, new ChapterStructure { VerseCount = 25 } },
                                    { 46, new ChapterStructure { VerseCount = 24 } },
                                    { 47, new ChapterStructure { VerseCount = 23 } },
                                    { 48, new ChapterStructure { VerseCount = 35 } },
                                }
                            }
                        },
                        {
                            15, new SectionStructure
                            {
                                ChapterCount = 14,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 9 } },
                                    { 2, new ChapterStructure { VerseCount = 25 } },
                                    { 3, new ChapterStructure { VerseCount = 5 } },
                                    { 4, new ChapterStructure { VerseCount = 19 } },
                                    { 5, new ChapterStructure { VerseCount = 15 } },
                                    { 6, new ChapterStructure { VerseCount = 11 } },
                                    { 7, new ChapterStructure { VerseCount = 16 } },
                                    { 8, new ChapterStructure { VerseCount = 14 } },
                                    { 9, new ChapterStructure { VerseCount = 17 } },
                                    { 10, new ChapterStructure { VerseCount = 15 } },
                                    { 11, new ChapterStructure { VerseCount = 11 } },
                                    { 12, new ChapterStructure { VerseCount = 15 } },
                                    { 13, new ChapterStructure { VerseCount = 15 } },
                                    { 14, new ChapterStructure { VerseCount = 10 } },
                                }
                            }
                        },
                        {
                            16, new SectionStructure
                            {
                                ChapterCount = 4,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 20 } },
                                    { 2, new ChapterStructure { VerseCount = 27 } },
                                    { 3, new ChapterStructure { VerseCount = 5 } },
                                    { 4, new ChapterStructure { VerseCount = 21 } },
                                }
                            }
                        },
                        {
                            17, new SectionStructure
                            {
                                ChapterCount = 9,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 15 } },
                                    { 2, new ChapterStructure { VerseCount = 16 } },
                                    { 3, new ChapterStructure { VerseCount = 15 } },
                                    { 4, new ChapterStructure { VerseCount = 13 } },
                                    { 5, new ChapterStructure { VerseCount = 27 } },
                                    { 6, new ChapterStructure { VerseCount = 14 } },
                                    { 7, new ChapterStructure { VerseCount = 17 } },
                                    { 8, new ChapterStructure { VerseCount = 14 } },
                                    { 9, new ChapterStructure { VerseCount = 15 } },
                                }
                            }
                        },
                        {
                            18, new SectionStructure
                            {
                                ChapterCount = 1,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 21 } },
                                }
                            }
                        },
                        {
                            19, new SectionStructure
                            {
                                ChapterCount = 4,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 16 } },
                                    { 2, new ChapterStructure { VerseCount = 11 } },
                                    { 3, new ChapterStructure { VerseCount = 10 } },
                                    { 4, new ChapterStructure { VerseCount = 11 } },
                                }
                            }
                        },
                        {
                            20, new SectionStructure
                            {
                                ChapterCount = 7,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 16 } },
                                    { 2, new ChapterStructure { VerseCount = 13 } },
                                    { 3, new ChapterStructure { VerseCount = 12 } },
                                    { 4, new ChapterStructure { VerseCount = 14 } },
                                    { 5, new ChapterStructure { VerseCount = 14 } },
                                    { 6, new ChapterStructure { VerseCount = 16 } },
                                    { 7, new ChapterStructure { VerseCount = 20 } },
                                }
                            }
                        },
                        {
                            21, new SectionStructure
                            {
                                ChapterCount = 3,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 14 } },
                                    { 2, new ChapterStructure { VerseCount = 14 } },
                                    { 3, new ChapterStructure { VerseCount = 19 } },
                                }
                            }
                        },
                        {
                            22, new SectionStructure
                            {
                                ChapterCount = 3,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 17 } },
                                    { 2, new ChapterStructure { VerseCount = 20 } },
                                    { 3, new ChapterStructure { VerseCount = 19 } },
                                }
                            }
                        },
                        {
                            23, new SectionStructure
                            {
                                ChapterCount = 3,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 18 } },
                                    { 2, new ChapterStructure { VerseCount = 15 } },
                                    { 3, new ChapterStructure { VerseCount = 20 } },
                                }
                            }
                        },
                        {
                            24, new SectionStructure
                            {
                                ChapterCount = 2,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 15 } },
                                    { 2, new ChapterStructure { VerseCount = 23 } },
                                }
                            }
                        },
                        {
                            25, new SectionStructure
                            {
                                ChapterCount = 14,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 17 } },
                                    { 2, new ChapterStructure { VerseCount = 17 } },
                                    { 3, new ChapterStructure { VerseCount = 10 } },
                                    { 4, new ChapterStructure { VerseCount = 14 } },
                                    { 5, new ChapterStructure { VerseCount = 11 } },
                                    { 6, new ChapterStructure { VerseCount = 15 } },
                                    { 7, new ChapterStructure { VerseCount = 14 } },
                                    { 8, new ChapterStructure { VerseCount = 23 } },
                                    { 9, new ChapterStructure { VerseCount = 17 } },
                                    { 10, new ChapterStructure { VerseCount = 12 } },
                                    { 11, new ChapterStructure { VerseCount = 17 } },
                                    { 12, new ChapterStructure { VerseCount = 14 } },
                                    { 13, new ChapterStructure { VerseCount = 9 } },
                                    { 14, new ChapterStructure { VerseCount = 21 } },
                                }
                            }
                        },
                        {
                            26, new SectionStructure
                            {
                                ChapterCount = 3,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 14 } },
                                    { 2, new ChapterStructure { VerseCount = 17 } },
                                    { 3, new ChapterStructure { VerseCount = 24 } },
                                }
                            }
                        },
                        {
                            27, new SectionStructure
                            {
                                ChapterCount = 150,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 6 } },
                                    { 2, new ChapterStructure { VerseCount = 12 } },
                                    { 3, new ChapterStructure { VerseCount = 9 } },
                                    { 4, new ChapterStructure { VerseCount = 9 } },
                                    { 5, new ChapterStructure { VerseCount = 13 } },
                                    { 6, new ChapterStructure { VerseCount = 11 } },
                                    { 7, new ChapterStructure { VerseCount = 18 } },
                                    { 8, new ChapterStructure { VerseCount = 10 } },
                                    { 9, new ChapterStructure { VerseCount = 21 } },
                                    { 10, new ChapterStructure { VerseCount = 18 } },
                                    { 11, new ChapterStructure { VerseCount = 7 } },
                                    { 12, new ChapterStructure { VerseCount = 9 } },
                                    { 13, new ChapterStructure { VerseCount = 6 } },
                                    { 14, new ChapterStructure { VerseCount = 7 } },
                                    { 15, new ChapterStructure { VerseCount = 5 } },
                                    { 16, new ChapterStructure { VerseCount = 11 } },
                                    { 17, new ChapterStructure { VerseCount = 15 } },
                                    { 18, new ChapterStructure { VerseCount = 51 } },
                                    { 19, new ChapterStructure { VerseCount = 15 } },
                                    { 20, new ChapterStructure { VerseCount = 10 } },
                                    { 21, new ChapterStructure { VerseCount = 14 } },
                                    { 22, new ChapterStructure { VerseCount = 32 } },
                                    { 23, new ChapterStructure { VerseCount = 6 } },
                                    { 24, new ChapterStructure { VerseCount = 10 } },
                                    { 25, new ChapterStructure { VerseCount = 22 } },
                                    { 26, new ChapterStructure { VerseCount = 12 } },
                                    { 27, new ChapterStructure { VerseCount = 14 } },
                                    { 28, new ChapterStructure { VerseCount = 9 } },
                                    { 29, new ChapterStructure { VerseCount = 11 } },
                                    { 30, new ChapterStructure { VerseCount = 13 } },
                                    { 31, new ChapterStructure { VerseCount = 25 } },
                                    { 32, new ChapterStructure { VerseCount = 11 } },
                                    { 33, new ChapterStructure { VerseCount = 22 } },
                                    { 34, new ChapterStructure { VerseCount = 23 } },
                                    { 35, new ChapterStructure { VerseCount = 28 } },
                                    { 36, new ChapterStructure { VerseCount = 13 } },
                                    { 37, new ChapterStructure { VerseCount = 40 } },
                                    { 38, new ChapterStructure { VerseCount = 23 } },
                                    { 39, new ChapterStructure { VerseCount = 14 } },
                                    { 40, new ChapterStructure { VerseCount = 18 } },
                                    { 41, new ChapterStructure { VerseCount = 14 } },
                                    { 42, new ChapterStructure { VerseCount = 12 } },
                                    { 43, new ChapterStructure { VerseCount = 5 } },
                                    { 44, new ChapterStructure { VerseCount = 27 } },
                                    { 45, new ChapterStructure { VerseCount = 18 } },
                                    { 46, new ChapterStructure { VerseCount = 12 } },
                                    { 47, new ChapterStructure { VerseCount = 10 } },
                                    { 48, new ChapterStructure { VerseCount = 15 } },
                                    { 49, new ChapterStructure { VerseCount = 21 } },
                                    { 50, new ChapterStructure { VerseCount = 23 } },
                                    { 51, new ChapterStructure { VerseCount = 21 } },
                                    { 52, new ChapterStructure { VerseCount = 11 } },
                                    { 53, new ChapterStructure { VerseCount = 7 } },
                                    { 54, new ChapterStructure { VerseCount = 9 } },
                                    { 55, new ChapterStructure { VerseCount = 24 } },
                                    { 56, new ChapterStructure { VerseCount = 14 } },
                                    { 57, new ChapterStructure { VerseCount = 12 } },
                                    { 58, new ChapterStructure { VerseCount = 12 } },
                                    { 59, new ChapterStructure { VerseCount = 18 } },
                                    { 60, new ChapterStructure { VerseCount = 14 } },
                                    { 61, new ChapterStructure { VerseCount = 9 } },
                                    { 62, new ChapterStructure { VerseCount = 13 } },
                                    { 63, new ChapterStructure { VerseCount = 12 } },
                                    { 64, new ChapterStructure { VerseCount = 11 } },
                                    { 65, new ChapterStructure { VerseCount = 14 } },
                                    { 66, new ChapterStructure { VerseCount = 20 } },
                                    { 67, new ChapterStructure { VerseCount = 8 } },
                                    { 68, new ChapterStructure { VerseCount = 36 } },
                                    { 69, new ChapterStructure { VerseCount = 37 } },
                                    { 70, new ChapterStructure { VerseCount = 6 } },
                                    { 71, new ChapterStructure { VerseCount = 24 } },
                                    { 72, new ChapterStructure { VerseCount = 20 } },
                                    { 73, new ChapterStructure { VerseCount = 28 } },
                                    { 74, new ChapterStructure { VerseCount = 23 } },
                                    { 75, new ChapterStructure { VerseCount = 11 } },
                                    { 76, new ChapterStructure { VerseCount = 13 } },
                                    { 77, new ChapterStructure { VerseCount = 21 } },
                                    { 78, new ChapterStructure { VerseCount = 72 } },
                                    { 79, new ChapterStructure { VerseCount = 13 } },
                                    { 80, new ChapterStructure { VerseCount = 20 } },
                                    { 81, new ChapterStructure { VerseCount = 17 } },
                                    { 82, new ChapterStructure { VerseCount = 8 } },
                                    { 83, new ChapterStructure { VerseCount = 19 } },
                                    { 84, new ChapterStructure { VerseCount = 13 } },
                                    { 85, new ChapterStructure { VerseCount = 14 } },
                                    { 86, new ChapterStructure { VerseCount = 17 } },
                                    { 87, new ChapterStructure { VerseCount = 7 } },
                                    { 88, new ChapterStructure { VerseCount = 19 } },
                                    { 89, new ChapterStructure { VerseCount = 53 } },
                                    { 90, new ChapterStructure { VerseCount = 17 } },
                                    { 91, new ChapterStructure { VerseCount = 16 } },
                                    { 92, new ChapterStructure { VerseCount = 16 } },
                                    { 93, new ChapterStructure { VerseCount = 5 } },
                                    { 94, new ChapterStructure { VerseCount = 23 } },
                                    { 95, new ChapterStructure { VerseCount = 11 } },
                                    { 96, new ChapterStructure { VerseCount = 13 } },
                                    { 97, new ChapterStructure { VerseCount = 12 } },
                                    { 98, new ChapterStructure { VerseCount = 9 } },
                                    { 99, new ChapterStructure { VerseCount = 9 } },
                                    { 100, new ChapterStructure { VerseCount = 5 } },
                                    { 101, new ChapterStructure { VerseCount = 8 } },
                                    { 102, new ChapterStructure { VerseCount = 29 } },
                                    { 103, new ChapterStructure { VerseCount = 22 } },
                                    { 104, new ChapterStructure { VerseCount = 35 } },
                                    { 105, new ChapterStructure { VerseCount = 45 } },
                                    { 106, new ChapterStructure { VerseCount = 48 } },
                                    { 107, new ChapterStructure { VerseCount = 43 } },
                                    { 108, new ChapterStructure { VerseCount = 14 } },
                                    { 109, new ChapterStructure { VerseCount = 31 } },
                                    { 110, new ChapterStructure { VerseCount = 7 } },
                                    { 111, new ChapterStructure { VerseCount = 10 } },
                                    { 112, new ChapterStructure { VerseCount = 10 } },
                                    { 113, new ChapterStructure { VerseCount = 9 } },
                                    { 114, new ChapterStructure { VerseCount = 8 } },
                                    { 115, new ChapterStructure { VerseCount = 18 } },
                                    { 116, new ChapterStructure { VerseCount = 19 } },
                                    { 117, new ChapterStructure { VerseCount = 2 } },
                                    { 118, new ChapterStructure { VerseCount = 29 } },
                                    { 119, new ChapterStructure { VerseCount = 176 } },
                                    { 120, new ChapterStructure { VerseCount = 7 } },
                                    { 121, new ChapterStructure { VerseCount = 8 } },
                                    { 122, new ChapterStructure { VerseCount = 9 } },
                                    { 123, new ChapterStructure { VerseCount = 4 } },
                                    { 124, new ChapterStructure { VerseCount = 8 } },
                                    { 125, new ChapterStructure { VerseCount = 5 } },
                                    { 126, new ChapterStructure { VerseCount = 6 } },
                                    { 127, new ChapterStructure { VerseCount = 5 } },
                                    { 128, new ChapterStructure { VerseCount = 6 } },
                                    { 129, new ChapterStructure { VerseCount = 8 } },
                                    { 130, new ChapterStructure { VerseCount = 8 } },
                                    { 131, new ChapterStructure { VerseCount = 3 } },
                                    { 132, new ChapterStructure { VerseCount = 18 } },
                                    { 133, new ChapterStructure { VerseCount = 3 } },
                                    { 134, new ChapterStructure { VerseCount = 3 } },
                                    { 135, new ChapterStructure { VerseCount = 21 } },
                                    { 136, new ChapterStructure { VerseCount = 26 } },
                                    { 137, new ChapterStructure { VerseCount = 9 } },
                                    { 138, new ChapterStructure { VerseCount = 8 } },
                                    { 139, new ChapterStructure { VerseCount = 24 } },
                                    { 140, new ChapterStructure { VerseCount = 14 } },
                                    { 141, new ChapterStructure { VerseCount = 10 } },
                                    { 142, new ChapterStructure { VerseCount = 8 } },
                                    { 143, new ChapterStructure { VerseCount = 12 } },
                                    { 144, new ChapterStructure { VerseCount = 15 } },
                                    { 145, new ChapterStructure { VerseCount = 21 } },
                                    { 146, new ChapterStructure { VerseCount = 10 } },
                                    { 147, new ChapterStructure { VerseCount = 20 } },
                                    { 148, new ChapterStructure { VerseCount = 14 } },
                                    { 149, new ChapterStructure { VerseCount = 9 } },
                                    { 150, new ChapterStructure { VerseCount = 6 } },
                                }
                            }
                        },
                        {
                            28, new SectionStructure
                            {
                                ChapterCount = 31,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 33 } },
                                    { 2, new ChapterStructure { VerseCount = 22 } },
                                    { 3, new ChapterStructure { VerseCount = 35 } },
                                    { 4, new ChapterStructure { VerseCount = 27 } },
                                    { 5, new ChapterStructure { VerseCount = 23 } },
                                    { 6, new ChapterStructure { VerseCount = 35 } },
                                    { 7, new ChapterStructure { VerseCount = 27 } },
                                    { 8, new ChapterStructure { VerseCount = 36 } },
                                    { 9, new ChapterStructure { VerseCount = 18 } },
                                    { 10, new ChapterStructure { VerseCount = 32 } },
                                    { 11, new ChapterStructure { VerseCount = 31 } },
                                    { 12, new ChapterStructure { VerseCount = 28 } },
                                    { 13, new ChapterStructure { VerseCount = 25 } },
                                    { 14, new ChapterStructure { VerseCount = 35 } },
                                    { 15, new ChapterStructure { VerseCount = 33 } },
                                    { 16, new ChapterStructure { VerseCount = 33 } },
                                    { 17, new ChapterStructure { VerseCount = 28 } },
                                    { 18, new ChapterStructure { VerseCount = 24 } },
                                    { 19, new ChapterStructure { VerseCount = 29 } },
                                    { 20, new ChapterStructure { VerseCount = 30 } },
                                    { 21, new ChapterStructure { VerseCount = 31 } },
                                    { 22, new ChapterStructure { VerseCount = 29 } },
                                    { 23, new ChapterStructure { VerseCount = 35 } },
                                    { 24, new ChapterStructure { VerseCount = 34 } },
                                    { 25, new ChapterStructure { VerseCount = 28 } },
                                    { 26, new ChapterStructure { VerseCount = 28 } },
                                    { 27, new ChapterStructure { VerseCount = 27 } },
                                    { 28, new ChapterStructure { VerseCount = 28 } },
                                    { 29, new ChapterStructure { VerseCount = 27 } },
                                    { 30, new ChapterStructure { VerseCount = 33 } },
                                    { 31, new ChapterStructure { VerseCount = 31 } },
                                }
                            }
                        },
                        {
                            29, new SectionStructure
                            {
                                ChapterCount = 42,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 22 } },
                                    { 2, new ChapterStructure { VerseCount = 13 } },
                                    { 3, new ChapterStructure { VerseCount = 26 } },
                                    { 4, new ChapterStructure { VerseCount = 21 } },
                                    { 5, new ChapterStructure { VerseCount = 27 } },
                                    { 6, new ChapterStructure { VerseCount = 30 } },
                                    { 7, new ChapterStructure { VerseCount = 21 } },
                                    { 8, new ChapterStructure { VerseCount = 22 } },
                                    { 9, new ChapterStructure { VerseCount = 35 } },
                                    { 10, new ChapterStructure { VerseCount = 22 } },
                                    { 11, new ChapterStructure { VerseCount = 20 } },
                                    { 12, new ChapterStructure { VerseCount = 25 } },
                                    { 13, new ChapterStructure { VerseCount = 28 } },
                                    { 14, new ChapterStructure { VerseCount = 22 } },
                                    { 15, new ChapterStructure { VerseCount = 35 } },
                                    { 16, new ChapterStructure { VerseCount = 22 } },
                                    { 17, new ChapterStructure { VerseCount = 16 } },
                                    { 18, new ChapterStructure { VerseCount = 21 } },
                                    { 19, new ChapterStructure { VerseCount = 29 } },
                                    { 20, new ChapterStructure { VerseCount = 29 } },
                                    { 21, new ChapterStructure { VerseCount = 34 } },
                                    { 22, new ChapterStructure { VerseCount = 30 } },
                                    { 23, new ChapterStructure { VerseCount = 17 } },
                                    { 24, new ChapterStructure { VerseCount = 25 } },
                                    { 25, new ChapterStructure { VerseCount = 6 } },
                                    { 26, new ChapterStructure { VerseCount = 14 } },
                                    { 27, new ChapterStructure { VerseCount = 23 } },
                                    { 28, new ChapterStructure { VerseCount = 28 } },
                                    { 29, new ChapterStructure { VerseCount = 25 } },
                                    { 30, new ChapterStructure { VerseCount = 31 } },
                                    { 31, new ChapterStructure { VerseCount = 40 } },
                                    { 32, new ChapterStructure { VerseCount = 22 } },
                                    { 33, new ChapterStructure { VerseCount = 33 } },
                                    { 34, new ChapterStructure { VerseCount = 37 } },
                                    { 35, new ChapterStructure { VerseCount = 16 } },
                                    { 36, new ChapterStructure { VerseCount = 33 } },
                                    { 37, new ChapterStructure { VerseCount = 24 } },
                                    { 38, new ChapterStructure { VerseCount = 41 } },
                                    { 39, new ChapterStructure { VerseCount = 30 } },
                                    { 40, new ChapterStructure { VerseCount = 32 } },
                                    { 41, new ChapterStructure { VerseCount = 26 } },
                                    { 42, new ChapterStructure { VerseCount = 17 } },
                                }
                            }
                        },
                        {
                            30, new SectionStructure
                            {
                                ChapterCount = 8,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 17 } },
                                    { 2, new ChapterStructure { VerseCount = 17 } },
                                    { 3, new ChapterStructure { VerseCount = 11 } },
                                    { 4, new ChapterStructure { VerseCount = 16 } },
                                    { 5, new ChapterStructure { VerseCount = 16 } },
                                    { 6, new ChapterStructure { VerseCount = 12 } },
                                    { 7, new ChapterStructure { VerseCount = 14 } },
                                    { 8, new ChapterStructure { VerseCount = 14 } },
                                }
                            }
                        },
                        {
                            31, new SectionStructure
                            {
                                ChapterCount = 4,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 22 } },
                                    { 2, new ChapterStructure { VerseCount = 23 } },
                                    { 3, new ChapterStructure { VerseCount = 18 } },
                                    { 4, new ChapterStructure { VerseCount = 22 } },
                                }
                            }
                        },
                        {
                            32, new SectionStructure
                            {
                                ChapterCount = 5,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 22 } },
                                    { 2, new ChapterStructure { VerseCount = 22 } },
                                    { 3, new ChapterStructure { VerseCount = 66 } },
                                    { 4, new ChapterStructure { VerseCount = 22 } },
                                    { 5, new ChapterStructure { VerseCount = 22 } },
                                }
                            }
                        },
                        {
                            33, new SectionStructure
                            {
                                ChapterCount = 12,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 18 } },
                                    { 2, new ChapterStructure { VerseCount = 26 } },
                                    { 3, new ChapterStructure { VerseCount = 22 } },
                                    { 4, new ChapterStructure { VerseCount = 17 } },
                                    { 5, new ChapterStructure { VerseCount = 19 } },
                                    { 6, new ChapterStructure { VerseCount = 12 } },
                                    { 7, new ChapterStructure { VerseCount = 29 } },
                                    { 8, new ChapterStructure { VerseCount = 17 } },
                                    { 9, new ChapterStructure { VerseCount = 18 } },
                                    { 10, new ChapterStructure { VerseCount = 20 } },
                                    { 11, new ChapterStructure { VerseCount = 10 } },
                                    { 12, new ChapterStructure { VerseCount = 14 } },
                                }
                            }
                        },
                        {
                            34, new SectionStructure
                            {
                                ChapterCount = 10,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 22 } },
                                    { 2, new ChapterStructure { VerseCount = 23 } },
                                    { 3, new ChapterStructure { VerseCount = 15 } },
                                    { 4, new ChapterStructure { VerseCount = 17 } },
                                    { 5, new ChapterStructure { VerseCount = 14 } },
                                    { 6, new ChapterStructure { VerseCount = 14 } },
                                    { 7, new ChapterStructure { VerseCount = 10 } },
                                    { 8, new ChapterStructure { VerseCount = 17 } },
                                    { 9, new ChapterStructure { VerseCount = 32 } },
                                    { 10, new ChapterStructure { VerseCount = 3 } },
                                }
                            }
                        },
                        {
                            35, new SectionStructure
                            {
                                ChapterCount = 12,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 21 } },
                                    { 2, new ChapterStructure { VerseCount = 49 } },
                                    { 3, new ChapterStructure { VerseCount = 33 } },
                                    { 4, new ChapterStructure { VerseCount = 34 } },
                                    { 5, new ChapterStructure { VerseCount = 30 } },
                                    { 6, new ChapterStructure { VerseCount = 29 } },
                                    { 7, new ChapterStructure { VerseCount = 28 } },
                                    { 8, new ChapterStructure { VerseCount = 27 } },
                                    { 9, new ChapterStructure { VerseCount = 27 } },
                                    { 10, new ChapterStructure { VerseCount = 21 } },
                                    { 11, new ChapterStructure { VerseCount = 45 } },
                                    { 12, new ChapterStructure { VerseCount = 13 } },
                                }
                            }
                        },
                        {
                            36, new SectionStructure
                            {
                                ChapterCount = 10,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 11 } },
                                    { 2, new ChapterStructure { VerseCount = 70 } },
                                    { 3, new ChapterStructure { VerseCount = 13 } },
                                    { 4, new ChapterStructure { VerseCount = 24 } },
                                    { 5, new ChapterStructure { VerseCount = 17 } },
                                    { 6, new ChapterStructure { VerseCount = 22 } },
                                    { 7, new ChapterStructure { VerseCount = 28 } },
                                    { 8, new ChapterStructure { VerseCount = 36 } },
                                    { 9, new ChapterStructure { VerseCount = 15 } },
                                    { 10, new ChapterStructure { VerseCount = 44 } },
                                }
                            }
                        },
                        {
                            37, new SectionStructure
                            {
                                ChapterCount = 13,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 11 } },
                                    { 2, new ChapterStructure { VerseCount = 20 } },
                                    { 3, new ChapterStructure { VerseCount = 38 } },
                                    { 4, new ChapterStructure { VerseCount = 17 } },
                                    { 5, new ChapterStructure { VerseCount = 19 } },
                                    { 6, new ChapterStructure { VerseCount = 19 } },
                                    { 7, new ChapterStructure { VerseCount = 72 } },
                                    { 8, new ChapterStructure { VerseCount = 18 } },
                                    { 9, new ChapterStructure { VerseCount = 37 } },
                                    { 10, new ChapterStructure { VerseCount = 40 } },
                                    { 11, new ChapterStructure { VerseCount = 36 } },
                                    { 12, new ChapterStructure { VerseCount = 47 } },
                                    { 13, new ChapterStructure { VerseCount = 31 } },
                                }
                            }
                        },
                        {
                            38, new SectionStructure
                            {
                                ChapterCount = 29,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 54 } },
                                    { 2, new ChapterStructure { VerseCount = 55 } },
                                    { 3, new ChapterStructure { VerseCount = 24 } },
                                    { 4, new ChapterStructure { VerseCount = 43 } },
                                    { 5, new ChapterStructure { VerseCount = 41 } },
                                    { 6, new ChapterStructure { VerseCount = 66 } },
                                    { 7, new ChapterStructure { VerseCount = 40 } },
                                    { 8, new ChapterStructure { VerseCount = 40 } },
                                    { 9, new ChapterStructure { VerseCount = 44 } },
                                    { 10, new ChapterStructure { VerseCount = 14 } },
                                    { 11, new ChapterStructure { VerseCount = 47 } },
                                    { 12, new ChapterStructure { VerseCount = 41 } },
                                    { 13, new ChapterStructure { VerseCount = 14 } },
                                    { 14, new ChapterStructure { VerseCount = 17 } },
                                    { 15, new ChapterStructure { VerseCount = 29 } },
                                    { 16, new ChapterStructure { VerseCount = 43 } },
                                    { 17, new ChapterStructure { VerseCount = 27 } },
                                    { 18, new ChapterStructure { VerseCount = 17 } },
                                    { 19, new ChapterStructure { VerseCount = 19 } },
                                    { 20, new ChapterStructure { VerseCount = 8 } },
                                    { 21, new ChapterStructure { VerseCount = 30 } },
                                    { 22, new ChapterStructure { VerseCount = 19 } },
                                    { 23, new ChapterStructure { VerseCount = 32 } },
                                    { 24, new ChapterStructure { VerseCount = 31 } },
                                    { 25, new ChapterStructure { VerseCount = 31 } },
                                    { 26, new ChapterStructure { VerseCount = 32 } },
                                    { 27, new ChapterStructure { VerseCount = 34 } },
                                    { 28, new ChapterStructure { VerseCount = 21 } },
                                    { 29, new ChapterStructure { VerseCount = 30 } },
                                }
                            }
                        },
                        {
                            39, new SectionStructure
                            {
                                ChapterCount = 36,
                                Chapters = new ChapterDataType
                                {
                                    { 1, new ChapterStructure { VerseCount = 18 } },
                                    { 2, new ChapterStructure { VerseCount = 17 } },
                                    { 3, new ChapterStructure { VerseCount = 17 } },
                                    { 4, new ChapterStructure { VerseCount = 22 } },
                                    { 5, new ChapterStructure { VerseCount = 14 } },
                                    { 6, new ChapterStructure { VerseCount = 42 } },
                                    { 7, new ChapterStructure { VerseCount = 22 } },
                                    { 8, new ChapterStructure { VerseCount = 18 } },
                                    { 9, new ChapterStructure { VerseCount = 31 } },
                                    { 10, new ChapterStructure { VerseCount = 19 } },
                                    { 11, new ChapterStructure { VerseCount = 23 } },
                                    { 12, new ChapterStructure { VerseCount = 16 } },
                                    { 13, new ChapterStructure { VerseCount = 23 } },
                                    { 14, new ChapterStructure { VerseCount = 14 } },
                                    { 15, new ChapterStructure { VerseCount = 19 } },
                                    { 16, new ChapterStructure { VerseCount = 14 } },
                                    { 17, new ChapterStructure { VerseCount = 19 } },
                                    { 18, new ChapterStructure { VerseCount = 34 } },
                                    { 19, new ChapterStructure { VerseCount = 11 } },
                                    { 20, new ChapterStructure { VerseCount = 37 } },
                                    { 21, new ChapterStructure { VerseCount = 20 } },
                                    { 22, new ChapterStructure { VerseCount = 12 } },
                                    { 23, new ChapterStructure { VerseCount = 21 } },
                                    { 24, new ChapterStructure { VerseCount = 27 } },
                                    { 25, new ChapterStructure { VerseCount = 28 } },
                                    { 26, new ChapterStructure { VerseCount = 23 } },
                                    { 27, new ChapterStructure { VerseCount = 9 } },
                                    { 28, new ChapterStructure { VerseCount = 27 } },
                                    { 29, new ChapterStructure { VerseCount = 36 } },
                                    { 30, new ChapterStructure { VerseCount = 27 } },
                                    { 31, new ChapterStructure { VerseCount = 21 } },
                                    { 32, new ChapterStructure { VerseCount = 33 } },
                                    { 33, new ChapterStructure { VerseCount = 25 } },
                                    { 34, new ChapterStructure { VerseCount = 33 } },
                                    { 35, new ChapterStructure { VerseCount = 27 } },
                                    { 36, new ChapterStructure { VerseCount = 23 } },
                                }
                            }
                        },



                    },
                }
            }
        };

    public static IRuleBuilderOptions<T, string> AuthenticationUsernameRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(5).WithMessage("Username must be at least 5 characters long.")
            .MaximumLength(16).WithMessage("Username cannot exceed 16 characters.")
            .Matches("^(?=.*[a-z])[a-z0-9._]+$")
            .WithMessage("Username must contain at least one lowercase letter and can only include lowercase letters, numbers, '.', and '_'.");
    }

    public static IRuleBuilderOptions<T, string> AuthenticationEmailRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.")
            .EmailAddress().WithMessage("Invalid email address format.");
    }

    public static IRuleBuilderOptions<T, string> AuthenticationPasswordRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(256).WithMessage("Password cannot exceed 256 characters.");
    }

    public static IRuleBuilderOptions<T, string> AuthenticationNameRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(16).WithMessage("Name cannot exceed 16 characters.")
            .Matches("^[A-Za-z]+$").WithMessage("Name must contain only letters (A-Z).");
    }

    public static IRuleBuilderOptions<T, string?> AuthenticationSurnameRules<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(16).WithMessage("Surname cannot exceed 16 characters.")
            .Matches("^[A-Za-z]*$").WithMessage("Surname must contain only letters (A-Z) if provided.");
    }

    public static IRuleBuilderOptions<T, string?> AuthenticationGenderRules<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(1).WithMessage("Gender must be a single character.")
            .Must(g => string.IsNullOrEmpty(g) || g == "M" || g == "F" || g == "U")
            .WithMessage("Invalid gender. Allowed values are 'M', 'F', or 'U'.");
    }

    public static IRuleBuilderOptions<T, IFormFile?> AuthenticationImageRules<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder, long maxFileSize, long requiredWidth, long requiredHeight)
    {
        return ruleBuilder
            .Must(file => file == null || IsAllowedExtension(file)).WithMessage("Only JPEG or JPG files are allowed.")
            .Must(file => file == null || file.Length <= maxFileSize).WithMessage($"Image size must be less than {maxFileSize / (1024 * 1024)} MB.")
            .Must(file => file == null || IsValidImage(file, requiredWidth, requiredHeight)).WithMessage($"Image must be {requiredWidth}x{requiredHeight} pixels and square.");
    }

    private static bool IsAllowedExtension(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg" };
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    private static bool IsValidImage(IFormFile file, long requiredWidth, long requiredHeight)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var image = Image.Load(stream);
            return image.Width == image.Height && image.Width == requiredWidth && image.Height == requiredHeight;
        }
        catch
        {
            return false;
        }
    }

    public static IRuleBuilderOptions<T, string> NoteTextRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("NoteText must not be empty.")
            .MinimumLength(1).WithMessage("NoteText must not be empty.")
            .Must(BeValidHtml).WithMessage("NoteText contains invalid content.");
    }

    private static bool BeValidHtml(string noteText)
    {
        HtmlSanitizer sanitizer = new();

        // Allowed tags, attributes and CSS properties.
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.Add("p");
        sanitizer.AllowedTags.Add("b");
        sanitizer.AllowedTags.Add("i");
        sanitizer.AllowedTags.Add("u");
        sanitizer.AllowedTags.Add("span");
        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.Add("style");
        sanitizer.AllowedCssProperties.Add("color");

        string sanitized = sanitizer.Sanitize(noteText);
        bool isValid = sanitized == noteText;

        if (!isValid) return false;

        // Phase 2: Plain text constraint.
        HtmlDocument doc = new();
        doc.LoadHtml(noteText);
        string plainText = doc.DocumentNode.InnerText;

        isValid = plainText.Length <= 1000;
        if (!isValid) return false;

        // Phase 3: HasNested tag validation.
        return HasNestedTags(doc.DocumentNode);
    }

    private static bool HasNestedTags(HtmlNode node)
    {
        if (node == null)
            return false;

        foreach (var child in node.ChildNodes)
        {
            if (child.NodeType == HtmlNodeType.Element)
            {
                if (node.ParentNode != null && node.ParentNode.Name != "#document")
                    return true;

                if (HasNestedTags(child))
                    return true;
            }
        }
        return false;
    }

    public static IRuleBuilderOptions<T, short> ScriptureNumberRules<T>(
         this IRuleBuilder<T, short> ruleBuilder)
    {
        return ruleBuilder
            .Must(num => num >= 1)
                .WithMessage("Scripture number is too small; minimum is 1.")
            .Must(Utility.SCRIPTURE_DATA.ContainsKey)
                .WithMessage(x => $"Scripture number {x} is not valid.");
    }

    public static IRuleBuilderOptions<T, short> SectionNumberRules<T>(
        this IRuleBuilder<T, short> ruleBuilder,
        Func<T, short> getScriptureNumber)
    {
        return ruleBuilder
            .Must(num => num >= 1)
                .WithMessage("Section number is too small; minimum is 1.")
            .Must((dto, sectionNumber) =>
            {
                var scriptureNumber = getScriptureNumber(dto);
                return sectionNumber <= GetSectionCount(scriptureNumber);
            })
                .WithMessage(dto =>
                {
                    var scriptureNumber = getScriptureNumber(dto);
                    return $"Section number is too big; maximum is {GetSectionCount(scriptureNumber)} in scripture {scriptureNumber}.";
                });
    }

    public static IRuleBuilderOptions<T, short> ChapterNumberRules<T>(
        this IRuleBuilder<T, short> ruleBuilder,
        Func<T, short> getScriptureNumber,
        Func<T, short> getSectionNumber)
    {
        return ruleBuilder
            .Must(num => num >= 1)
                .WithMessage("Chapter number is too small; minimum is 1.")
            .Must((dto, chapterNumber) =>
            {
                var scriptureNumber = getScriptureNumber(dto);
                var sectionNumber = getSectionNumber(dto);
                return chapterNumber <= GetChapterCount(scriptureNumber, sectionNumber);
            })
                .WithMessage(dto =>
                {
                    var scriptureNumber = getScriptureNumber(dto);
                    var sectionNumber = getSectionNumber(dto);
                    return $"Chapter number is too big; maximum is {GetChapterCount(scriptureNumber, sectionNumber)}"
                         + $" in section {sectionNumber} of scripture {scriptureNumber}.";
                });
    }

    public static IRuleBuilderOptions<T, int> VerseNumberRules<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        Func<T, short> getScriptureNumber,
        Func<T, short> getSectionNumber,
        Func<T, short> getChapterNumber)
    {
        return ruleBuilder
            .Must(num => num >= 1)
                .WithMessage("Verse number is too small; minimum is 1.")
            .Must((dto, verseNumber) =>
            {
                var scriptureNumber = getScriptureNumber(dto);
                var sectionNumber = getSectionNumber(dto);
                var chapterNumber = getChapterNumber(dto);
                return verseNumber <= GetVerseCount(scriptureNumber, sectionNumber, chapterNumber);
            })
                .WithMessage(dto =>
                {
                    var scriptureNumber = getScriptureNumber(dto);
                    var sectionNumber = getSectionNumber(dto);
                    var chapterNumber = getChapterNumber(dto);
                    return $"Verse number is too big; maximum is {GetVerseCount(scriptureNumber, sectionNumber, chapterNumber)}"
                         + $" in chapter {chapterNumber}.";
                });
    }



    public static IRuleBuilderOptions<T, int> CollectionIdRule<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage("CollectionId must be greater than 0.")
            .LessThan(int.MaxValue).WithMessage("CollectionId must be less than the maximum integer value.");
    }

    public static IRuleBuilderOptions<T, string?> CollectionNameRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {

        return ruleBuilder.Must((x =>
        {
            string? trimmed = x?.Trim();
            return trimmed == null || !string.IsNullOrEmpty(trimmed);
        }))
        .WithMessage("Collection name cannot be empty or null.")
        .MinimumLength(1).WithMessage("Collection name must have at least 1 character.")
        .MaximumLength(24).WithMessage("Collection name cannot exceed 24 characters.");
    }

    public static IRuleBuilderOptions<T, string?> CollectionDescriptionRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must((x =>
        {
            string? trimmed = x?.Trim();
            return trimmed == null || !string.IsNullOrEmpty(trimmed);
        }))
            .MaximumLength(72).WithMessage("Collection description cannot exceed 72 characters.");
    }



    public static int GetSectionCount(short scriptureNumber)
    {
        if (Utility.SCRIPTURE_DATA.TryGetValue(scriptureNumber, out var scripture))
        {
            return scripture.SectionCount;
        }

        return -1;
    }

    public static int GetChapterCount(short scriptureNumber, short sectionNumber)
    {
        if (Utility.SCRIPTURE_DATA.TryGetValue(scriptureNumber, out var scripture) &&
            scripture.Sections.TryGetValue(sectionNumber, out var section))
        {
            return section.ChapterCount;
        }

        return -1;
    }

    public static int GetVerseCount(short scriptureNumber, short sectionNumber, short chapterNumber)
    {
        if (Utility.SCRIPTURE_DATA.TryGetValue(scriptureNumber, out var scripture) &&
            scripture.Sections.TryGetValue(sectionNumber, out var section) &&
            section.Chapters.TryGetValue(chapterNumber, out var chapter))
        {
            return chapter.VerseCount;
        }

        return -1;
    }
}


