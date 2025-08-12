using EntityFramework.Explained._Tools.Helpers;
using Microsoft.EntityFrameworkCore;
using QuickPulse.Explains;
using QuickPulse.Explains.Text;

namespace EntityFramework.Explained.Schema.Conventions;

[DocFile]
public class TablePerType
{
    public class AnimalServerDbContext<T> : DbContext where T : class
    {
        public DbSet<Animal> Animals => Set<Animal>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("DoesNotMatter");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Animal>().UseTptMappingStrategy();

            modelBuilder.Entity<Dog>()
            .ToTable("Dogs");

            modelBuilder.Entity<Cat>()
            .ToTable("Cats");

        }
    }

    public class AnimalSqliteDbContext<T> : DbContext where T : class
    {
        public DbSet<Animal> Animals => Set<Animal>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("DoesNotMatter");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Animal>().UseTptMappingStrategy();

            modelBuilder.Entity<Dog>()
            .ToTable("Dogs");

            modelBuilder.Entity<Cat>()
            .ToTable("Cats");
        }
    }

    public class Animal
    {
        public int Id { get; set; }
        public string isPet { get; set; }
    }

    public class Cat : Animal
    {
        public int numberOfMeows { get; set; }
    }

    public class Dog : Animal
    {
        public int numberOfBarks { get; set; }
    }

    [Fact]
    [DocHeader("Sql Server")]
    [DocContent("Creates tables per type classes: Animal, Dog and Cat using UseTptMappingStrategy() and ToTable() method. UseTptMappingStrategy() is necessary because else EF Core thinks you are using TPH. The tabels are one on one related with the property Id.")]
    public void SqlServer_Uses_TPT_With_Mapping_strategy()
    {
        using var context = new AnimalServerDbContext<Animal>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);

        Assert.Equal("CREATE TABLE [Animals] (", reader.SkipToLineContaining("Animals"));
        Assert.Equal("    [Id] int NOT NULL IDENTITY,", reader.NextLine());
        Assert.Equal("    [isPet] nvarchar(max) NOT NULL,", reader.NextLine());
        Assert.Equal("    CONSTRAINT [PK_Animals] PRIMARY KEY ([Id])", reader.NextLine());

        Assert.Equal("CREATE TABLE [Cats] (", reader.SkipToLineContaining("Cats"));
        Assert.Equal("    [Id] int NOT NULL,", reader.NextLine());
        Assert.Equal("    [numberOfMeows] int NOT NULL,", reader.NextLine());
        Assert.Equal("    CONSTRAINT [PK_Cats] PRIMARY KEY ([Id]),", reader.NextLine());
        Assert.Equal("    CONSTRAINT [FK_Cats_Animals_Id] FOREIGN KEY ([Id]) REFERENCES [Animals] ([Id]) ON DELETE CASCADE", reader.NextLine());

        Assert.Equal("CREATE TABLE [Dogs] (", reader.SkipToLineContaining("Dogs"));
        Assert.Equal("    [Id] int NOT NULL,", reader.NextLine());
        Assert.Equal("    [numberOfBarks] int NOT NULL,", reader.NextLine());
        Assert.Equal("    CONSTRAINT [PK_Dogs] PRIMARY KEY ([Id]),", reader.NextLine());
        Assert.Equal("    CONSTRAINT [FK_Dogs_Animals_Id] FOREIGN KEY ([Id]) REFERENCES [Animals] ([Id]) ON DELETE CASCADE", reader.NextLine());

    }

    [Fact]
    [DocHeader("Sqlite")]
    [DocContent("Creates tables per type classes: Animal, Dog and Cat using UseTptMappingStrategy() and ToTable() method. UseTptMappingStrategy() is necessary because else EF Core thinks you are using TPH. The tabels are one on one related with the property Id.")]
    public void Sqlite_Uses_Tpt_With_Mapping_strategy()
    {
        using var context = new AnimalSqliteDbContext<Animal>();
        var sql = context.Database.GenerateCreateScript();
        var reader = LinesReader.FromText(sql);

        Assert.Equal("CREATE TABLE \"Animals\" (", reader.SkipToLineContaining("Animals"));
        Assert.Equal("    \"Id\" INTEGER NOT NULL CONSTRAINT \"PK_Animals\" PRIMARY KEY AUTOINCREMENT,", reader.NextLine());
        Assert.Equal("    \"isPet\" TEXT NOT NULL", reader.NextLine());

        Assert.Equal("CREATE TABLE \"Cats\" (", reader.SkipToLineContaining("Cats"));
        Assert.Equal("    \"Id\" INTEGER NOT NULL CONSTRAINT \"PK_Cats\" PRIMARY KEY AUTOINCREMENT,", reader.NextLine());
        Assert.Equal("    \"numberOfMeows\" INTEGER NOT NULL,", reader.NextLine());
        Assert.Equal("    CONSTRAINT \"FK_Cats_Animals_Id\" FOREIGN KEY (\"Id\") REFERENCES \"Animals\" (\"Id\") ON DELETE CASCADE", reader.NextLine());

        Assert.Equal("CREATE TABLE \"Dogs\" (", reader.SkipToLineContaining("Dogs"));
        Assert.Equal("    \"Id\" INTEGER NOT NULL CONSTRAINT \"PK_Dogs\" PRIMARY KEY AUTOINCREMENT,", reader.NextLine());
        Assert.Equal("    \"numberOfBarks\" INTEGER NOT NULL,", reader.NextLine());
        Assert.Equal("    CONSTRAINT \"FK_Dogs_Animals_Id\" FOREIGN KEY (\"Id\") REFERENCES \"Animals\" (\"Id\") ON DELETE CASCADE", reader.NextLine());

    }
}