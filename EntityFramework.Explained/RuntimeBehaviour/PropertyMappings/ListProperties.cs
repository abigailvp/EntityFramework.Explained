using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickPulse.Explains;

namespace EntityFramework.Explained.RuntimeBehaviour.PropertyMappings;

[DocFile]
public class ListProperties
{
    public class Thing
    {
        public int Id { get; set; }
        public List<string> StringListProperty { get; set; } = [];
    }

    [Fact]
    [DocContent(
    "EF Core does not detect in-place mutations to, for instance, a `List<string>` " +
    "when only a value converter is used. The property reference remains unchanged, " +
    "so change tracking is not triggered and `SaveChanges()` persists nothing.  ")]
    [DocCodeExample(nameof(MapThingNoValueComparer))]
    public async Task Update_not_performed_without_ValueComparer()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        Thing thing;

        using (var context = new AppDbContext(options))
        {
            thing = new Thing { StringListProperty = ["Test", "Testier"] };
            context.Things.Add(thing);
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var toUpdate = await context.Things.FindAsync(thing.Id);
            toUpdate?.StringListProperty.Add("new one");
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var updated = await context.Things.FindAsync(thing.Id);
            Assert.Equal(["Test", "Testier"], updated?.StringListProperty);
        }
    }

    [DocExample]
    private static void MapThingNoValueComparer(EntityTypeBuilder<Thing> entityTypeBuilder)
    {
        var converter = new ValueConverter<List<string>, string>(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
            );
        entityTypeBuilder.Property(c => c.StringListProperty)
            .HasConversion(converter);
    }

    public class AppDbContext : DbContext
    {
        public DbSet<Thing> Things { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            MapThingNoValueComparer(modelBuilder.Entity<Thing>());
        }
    }

    [Fact]
    [DocContent(
        "Adding a `ValueComparer<List<string>>` to the mapping allows EF Core " +
        "to detect in-place list mutations. The comparer inspects the list's contents, " +
        "so `SaveChanges()` correctly persists changes without replacing the list instance.  ")]
    [DocCodeExample(nameof(MapThingWithValueComparer))]
    public async Task Update_is_performed_with_ValueComparer()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppVCDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AppVCDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        Thing thing;

        using (var context = new AppVCDbContext(options))
        {
            thing = new Thing { StringListProperty = ["Test", "Testier"] };
            context.Things.Add(thing);
            await context.SaveChangesAsync();
        }

        using (var context = new AppVCDbContext(options))
        {
            var toUpdate = await context.Things.FindAsync(thing.Id);
            toUpdate?.StringListProperty.Add("new one");
            await context.SaveChangesAsync();
        }

        using (var context = new AppVCDbContext(options))
        {
            var updated = await context.Things.FindAsync(thing.Id);
            Assert.Equal(["Test", "Testier", "new one"], updated?.StringListProperty);
        }
    }

    [DocExample]
    private static void MapThingWithValueComparer(EntityTypeBuilder<Thing> entityTypeBuilder)
    {
        var converter = new ValueConverter<List<string>, string>(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
            );
        var comparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
        );
        entityTypeBuilder.Property(c => c.StringListProperty)
            .HasConversion(converter)
            .Metadata.SetValueComparer(comparer);
    }

    public class AppVCDbContext : DbContext
    {
        public DbSet<Thing> Things { get; set; }
        public AppVCDbContext(DbContextOptions<AppVCDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            MapThingWithValueComparer(modelBuilder.Entity<Thing>());
        }
    }
}


