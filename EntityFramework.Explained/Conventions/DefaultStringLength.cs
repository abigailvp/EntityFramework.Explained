using EntityFramework.Explained._Tools.Helpers;
using EntityFramework.Explained._Tools.TestContexts;
using Microsoft.EntityFrameworkCore;
using QuickPulse.Explains;
using QuickPulse.Explains.Text;

namespace EntityFramework.Explained.Conventions;

[DocFile]
public class DefaultStringLength
{
    public class Thing
    {
        public int Id { get; set; }
        public string StringProperty { get; set; } = default!;
    }

    [Fact]
    [DocHeader("Sql Server")]
    [DocContent("Generates `nvarchar(max)`.")]
    public void SqlServer()
    {
        using var context = new TestSqlServerContext<Thing>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);
        Assert.Contains("[StringProperty] nvarchar(max) NOT NULL", reader.SkipToLineContaining("StringProperty"));
    }

    [Fact]
    [DocHeader("Sqlite")]
    [DocContent("Generates `TEXT`.")]
    public void Sqlite()
    {
        using var context = new TestSqliteContext<Thing>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);
        Assert.Contains("\"StringProperty\" TEXT NOT NULL", reader.SkipToLineContaining("StringProperty"));
    }
}