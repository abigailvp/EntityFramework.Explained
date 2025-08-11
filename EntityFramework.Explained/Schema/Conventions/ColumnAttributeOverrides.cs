using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using EntityFramework.Explained._Tools.Helpers;
using EntityFramework.Explained._Tools.TestContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickPulse.Explains;
using QuickPulse.Explains.Text;
using QuickPulse.Show;

namespace EntityFramework.Explained.Schema.Conventions;

[DocFile] //maakt van classnaam automatisch een titel in doc
public class ColumnAttributesOverrides
{
    public class Student
    {
        [Column("Id", Order = 0)]
        public int StudentId { get; set; } = default!; //met of zonder KLASSEnaam (dus hier: Student)

        [Column("Age", Order = 2)]
        public int StudentAge { get; set; }

        [Column("Name", Order = 1)]
        public string StudentName { get; set; }

        [Column("Birthdate", Order = 3)]
        public DateTime StudentBirthdate { get; set; }

    }


    [Fact]
    [DocHeader("Sql Server")]
    [DocContent("Generates `nvarchar(max)`.")]
    public void SqlServer()
    {
        using var context = new TestSqlServerContext<Student>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);
        // reader.AsAssertsToLogFile();

        Assert.Equal("CREATE TABLE [Items] (", reader.NextLine());
        Assert.Equal("    [Id] int NOT NULL IDENTITY,", reader.NextLine());
        Assert.Equal("    [Name] nvarchar(max) NOT NULL,", reader.NextLine());
        Assert.Equal("    [Age] int NOT NULL,", reader.NextLine());
        Assert.Equal("    [Birthdate] datetime2 NOT NULL,", reader.NextLine());
        Assert.Equal("    CONSTRAINT [PK_Items] PRIMARY KEY ([Id])", reader.NextLine());


    }

    [Fact]
    [DocHeader("Sql Server")]
    [DocContent("Generates `nvarchar(max)`.")]
    public void SqlServerFollowsOrder()
    {
        using var context = new TestSqlServerContext<Student>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);
        // reader.AsAssertsToLogFile();

        Assert.Equal("CREATE TABLE [Items] (", reader.NextLine());
        Assert.Equal("    [Id] int NOT NULL IDENTITY,", reader.NextLine());
        Assert.Equal("    [Age] int NOT NULL,", reader.SkipToLineContaining("int")); //naam staat op lijn hiervoor want order=1
        Assert.Equal("    [Birthdate] datetime2 NOT NULL,", reader.NextLine());
        Assert.Equal("    CONSTRAINT [PK_Items] PRIMARY KEY ([Id])", reader.NextLine());
        Assert.Equal(");", reader.NextLine());
        Assert.Equal("GO", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfContent());

    }

    [Fact]
    [DocHeader("Sqlite")]
    [DocContent("Generates `TEXT`.")]
    public void Sqlite()
    {
        using var context = new TestSqliteContext<Student>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);
        // reader.AsAssertsToLogFile();
        Assert.Contains("\"Name\" TEXT NOT NULL", reader.SkipToLineContaining("Name"));
        Assert.Contains("\"Age\" INTEGER NOT NULL", reader.NextLine());
        Assert.Contains("\"Birthdate\" TEXT NOT NULL", reader.NextLine()); //geen datetime!!
    }
}