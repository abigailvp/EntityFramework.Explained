using QuickPulse.Explains;

namespace EntityFramework.Explained;

[DocFile]
[DocFileHeader("EntityFramework.Explained")]
[DocContent("> Because We Need to Talk About Kevin.  \n")]
public class CreateDoc
{
    [Fact]
    public void Now()
    {
        Explain.This<CreateDoc>("README.md");
    }
}