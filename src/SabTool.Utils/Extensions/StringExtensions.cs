namespace SabTool.Utils.Extensions;

public static class StringExtensions
{
    public static string TrimMatchingQuotes(this string input, char quote)
    {
        return input.Length >= 2 && input[0] == quote && input[^1] == quote ? input[1..^1] : input;
    }

    public static IEnumerable<string> Split(this string str, Func<char, bool> controller)
    {
        int nextPiece = 0;

        for (int c = 0; c < str.Length; ++c)
        {
            if (controller(str[c]))
            {
                yield return str[nextPiece..c];

                nextPiece = c + 1;
            }
        }

        yield return str[nextPiece..];
    }
}
