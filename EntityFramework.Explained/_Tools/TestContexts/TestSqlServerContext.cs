using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Explained._Tools.TestContexts;

public class TestSqlServerContext<T> : DbContext where T : class
{
    public DbSet<T> Items => Set<T>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("DoesNotMatter"); // Required by EF, never actually used
}

public class TestSqlServerContext<T1, T2> : DbContext where T1 : class where T2 : class
{
    public DbSet<T1> Items1 => Set<T1>();
    public DbSet<T2> Items2 => Set<T2>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("DoesNotMatter"); // Required by EF, never actually used
}