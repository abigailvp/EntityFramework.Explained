namespace EntityFramework.Explained._Tools.Helpers;

public static class StringExtensions
{
    public static string[] Lines(this string text) =>
        text.Split(["\r\n", "\n"], StringSplitOptions.None);
}