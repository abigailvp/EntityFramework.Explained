using EntityFramework.Explained._Tools.TestContexts;
using Microsoft.EntityFrameworkCore;
using QuickPulse.Explains;
using QuickPulse.Explains.Formatters;
using QuickPulse.Explains.Text;

namespace EntityFramework.Explained.Schema.Relationships.UniDirectionalOneToMany;

[DocFile]
[DocContent(
@"Because the entity used in the `DbSet` has a collection of another entity type, the latter are mapped as well.  
EF infers and includes related entities in the schema even when only one side is explicitly registered in the `DbContext`.  
")]
public class UniDirectionalOneToManyWithOneDbSet
{
    [CodeExample]
    public class Post
    {
        public int Id { get; set; }
        public Blog Blog { get; set; } = default!;
    }

    [CodeExample]
    public class Blog
    {
        public int Id { get; set; }
    }

    [Fact]
    [DocContent("When using the following simple model of *one* `Blog` containing *many* `Posts`: ")]
    [DocExample(typeof(Blog))]
    [DocExample(typeof(Post))]
    [DocContent("And then adding a `DbSet<Blog>` to the `DbContext` EF generates the following ddl for Sql Server:  \n")]
    [DocContent("**Blog:**")]
    [DocExample(typeof(UniDirectionalOneToManyWithOneDbSet), nameof(ExpectedDdlScriptForBlog), "sql")]
    [DocContent("**Post:**")]
    [DocExample(typeof(UniDirectionalOneToManyWithOneDbSet), nameof(ExpectedDdlScriptForPost), "sql")]
    [DocContent("**Index:**")]
    [DocExample(typeof(UniDirectionalOneToManyWithOneDbSet), nameof(ExpectedDdlScriptForIndex), "sql")]
    [DocContent("*Note:* No other mappings were added.")]
    public void SqlServer()
    {
        using var context = new TestSqlServerContext<Post>();

        var sql = context.Database.GenerateCreateScript();

        var reader = LinesReader.FromText(sql);

        Assert.Equal(ExpectedDdlScriptForBlog(), reader.NextLines(4));

        reader.Skip(3);
        Assert.Equal(ExpectedDdlScriptForPost(), reader.NextLines(6));

        reader.Skip(3);
        Assert.Equal(ExpectedDdlScriptForIndex(), reader.NextLine());

        reader.Skip(4);
        Assert.True(reader.EndOfContent());
    }

    [CodeSnippet]
    [CodeFormat(typeof(StringArrayToString))]
    private string[] ExpectedDdlScriptForBlog()
    {
        return
            [
                "CREATE TABLE [Blog] (",
                "    [Id] int NOT NULL IDENTITY,",
                "    CONSTRAINT [PK_Blog] PRIMARY KEY ([Id])",
                ");"
            ];
    }

    [CodeSnippet]
    [CodeReplace("Item", "Post")]
    [CodeFormat(typeof(StringArrayToString))]
    private string[] ExpectedDdlScriptForPost()
    {
        return
            [
                "CREATE TABLE [Items] (",
                "    [Id] int NOT NULL IDENTITY,",
                "    [BlogId] int NOT NULL,",
                "    CONSTRAINT [PK_Items] PRIMARY KEY ([Id]),",
                "    CONSTRAINT [FK_Items_Blog_BlogId] FOREIGN KEY ([BlogId]) REFERENCES [Blog] ([Id]) ON DELETE CASCADE",
                ");"
            ];
    }

    [CodeSnippet]
    [CodeReplace("return", "")]
    [CodeReplace("Item", "Post")]
    [CodeReplace("\"", "")]
    private string ExpectedDdlScriptForIndex()
    {
        return "CREATE INDEX [IX_Items_BlogId] ON [Items] ([BlogId]);";
    }
}