


public class CoachCrudTests : CrudTestBase<AppDbContext, Coach>
{
    protected override AppDbContext CreateContext(DbContextOptions<AppDbContext> options)
        => new AppDbContext(options);

    protected override Coach CreateEntity()
        => new Coach(FullName.From("Unit Test"), EmailAddress.From("unit@test.com"));

    protected override DbSet<Coach> GetDbSet(AppDbContext context)
        => context.Coaches;

    protected override object[] GetPrimaryKey(Coach entity)
        => new object[] { entity.Id };

    protected override async Task ModifyEntityAsync(Coach entity)
    {
        entity.UpdateSkills(new List<string> { "unit", "test" });
        await Task.CompletedTask;
    }

    protected override async Task AssertUpdatedAsync(Coach entity)
    {
        Assert.Contains("unit", entity.Skills);
        Assert.Contains("test", entity.Skills);
        await Task.CompletedTask;
    }
}
