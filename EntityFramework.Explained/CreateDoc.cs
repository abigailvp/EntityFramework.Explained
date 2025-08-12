using QuickPulse.Explains;

namespace EntityFramework.Explained;

public class CreateDoc
{
    // [Fact]
    [Fact(Skip = "Explicit")]
    public void Now()
    {
        Explain.These<CreateDoc>("TheDocs");
    }
}