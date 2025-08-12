using QuickPulse.Explains;

namespace EntityFramework.Explained;

public class CreateDoc
{
    [Fact]
    public void Now()
    {
        Explain.These<CreateDoc>("TheDocs");
    }
}