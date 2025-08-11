using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using EntityFramework.Explained._Tools.Helpers;
using EntityFramework.Explained._Tools.TestContexts;
using Microsoft.EntityFrameworkCore;
using QuickPulse.Explains;
using QuickPulse.Explains.Text;
using QuickPulse.Show;

namespace EntityFramework.Explained.Schema.Conventions;

[DocFile] //maakt van classnaam automatisch een titel in doc
public class RequiredAttributes
{
    public class Thing
    {
        public int Id { get; set; } = default!;

        [Required]
        public string StringProperty { get; set; } = default!;
    }

    [Fact]
    [DocHeader("Sql Server")] //wordt header
    [DocContent("Generates `nvarchar(max)`.")] //wordt paragraaf
    public void SqlServer()
    {
        using var context = new TestSqlServerContext<Thing>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql); //hulpfunctie die test schrijft in quickpulse file
        // reader.AsAssertsToLogFile(); //maakt nieuwe file
        // Assert.Contains("[Id] int NOT NULL IDENTITY, ", reader.SkipToLineContaining("Id"));
        // Assert.Contains("[StringProperty] nvarchar(max) NOT NULL", reader.SkipToLineContaining("StringProperty"));
        Assert.Equal("CREATE TABLE [Items] (", reader.NextLine());
        Assert.Equal("    [Id] int NOT NULL IDENTITY,", reader.NextLine());
        Assert.Equal("    [StringProperty] nvarchar(max) NOT NULL,", reader.NextLine()); //not null want required
        Assert.Equal("    CONSTRAINT [PK_Items] PRIMARY KEY ([Id])", reader.NextLine()); //automatisch id maken
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
        using var context = new TestSqliteContext<Thing>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);
        reader.AsAssertsToLogFile();
        Assert.Contains("\"StringProperty\" TEXT NOT NULL", reader.SkipToLineContaining("StringProperty"));
    }
}