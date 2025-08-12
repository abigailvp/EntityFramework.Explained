using EntityFramework.Explained._Tools.TestContexts;
using Microsoft.EntityFrameworkCore;
using QuickPulse.Explains;
using QuickPulse.Explains.Text;

namespace EntityFramework.Explained.Schema.Conventions;

[DocFile]
public class IdentityStrategy
{
    public class UnidentifiedThing
    {
        public int Id { get; set; }
    }

    [Keyless]
    public class ProfessionalUnidentifiedThing
    {
        public int Id { get; set; }
    }

    [PrimaryKey(nameof(Id))]
    public class Thing
    {
        public Guid Id { get; set; }
        public SmallThing? thingy { get; set; }
    }

    [Owned]
    public class SmallThing
    {
        public string? description { get; set; }
    }

    [Fact]
    [DocHeader("Sql Server")]
    [DocContent("throws InvalidOperationException if primary key is not defined")]
    public void SqlServer_No_Pk()
    {
        using var context = new TestSqlServerContext<UnidentifiedThing>();
        var ex = Assert.Throws<InvalidOperationException>(() => context.Database.GenerateCreateScript());
        Assert.Contains("The entity type 'Id<LazyThing>' requires a primary key to be defined.", ex.Message);
    }

    [Fact]
    [DocHeader("Sql Server")]
    [DocContent("generates id as a column with Primary Key constraint in table when primary key is defined of right type")]
    public void SqlServer_With_Pk()
    {
        using var context = new TestSqlServerContext<Thing>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);

        Assert.Equal("    [Id] uniqueidentifier NOT NULL,", reader.SkipToLineContaining("Id"));
        Assert.Equal("    CONSTRAINT [PK_Items] PRIMARY KEY ([Id])", reader.NextLine());
    }

    [Fact]
    [DocHeader("Sql Server")]
    [DocContent("doesn't need a pk if data annotation says so ([Keyless]. You cannot use CRUD on this table.")]
    public void SqlServer_With_Keyless_Configured()
    {
        using var context = new TestSqlServerContext<ProfessionalUnidentifiedThing>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);

        Assert.Equal("CREATE TABLE [Items] (", reader.NextLine());
        Assert.Equal("    [Id] int NOT NULL", reader.NextLine());
    }

    [Fact]
    [DocHeader("Sqlite")]
    [DocContent("generates Id as a column with Primary Key constraint in table when primary key is defined of right type")]
    public void Sqlite_With_Pk()
    {
        using var context = new TestSqliteContext<Thing>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);

        Assert.Equal("    \"Id\" TEXT NOT NULL CONSTRAINT \"PK_Items\" PRIMARY KEY", reader.SkipToLineContaining("Id"));
    }

    [Fact]
    [DocHeader("Sqlite")]
    [DocContent("only needs an Id for entities, not for valuetypes inside an entity.")]
    public void Sqlite_Only_Id_For_Entities()
    {
        using var context = new TestSqliteContext<Thing>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);

        Assert.Equal("    \"Id\" TEXT NOT NULL CONSTRAINT \"PK_Items\" PRIMARY KEY,", reader.SkipToLineContaining("Id"));
        Assert.Equal("    \"thingy_description\" TEXT NOT NULL", reader.NextLine());
    }

    [Fact]
    [DocHeader("Sqlite")]
    [DocContent("can only have specific types, SqlServer is even stricter in which types can be used. ")]
    public void Sqlite__With_Pk()
    {
        using var context = new TestSqliteContext<Thing>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);

        Assert.Equal("    \"Id\" TEXT NOT NULL CONSTRAINT \"PK_Items\" PRIMARY KEY,", reader.SkipToLineContaining("Id"));
        Assert.Equal("    \"thingy_description\" TEXT NOT NULL", reader.NextLine());
    }
}