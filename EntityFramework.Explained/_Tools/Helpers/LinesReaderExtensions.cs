using QuickPulse.Explains.Text;

namespace EntityFramework.Explained._Tools.Helpers;

public static class LinesReaderExtensions
{
    public static string SkipToLineContaining(this LinesReader reader, string fragment)
    {
        while (true)
        {
            var line = reader.NextLine();
            if (line.Contains(fragment, StringComparison.OrdinalIgnoreCase))
                return line;
        }
    }
}