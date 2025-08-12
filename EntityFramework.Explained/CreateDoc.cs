using QuickPulse.Explains;

namespace EntityFramework.Explained;

public class CreateDoc
{
    [Fact(Skip = "Explicit")]
    public void Now()
    {
        Explain.These<CreateDoc>("TheDocs");
    }
}