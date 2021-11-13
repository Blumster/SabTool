using System;
using System.Collections.Generic;

namespace SabTool.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if (input.Length >= 2 && input[0] == quote && input[input.Length - 1] == quote)
                return input.Substring(1, input.Length - 2);

            return input;
        }

        public static IEnumerable<string> Split(this string str, Func<char, bool> controller)
        {
            var nextPiece = 0;

            for (var c = 0; c < str.Length; ++c)
            {
                if (controller(str[c]))
                {
                    yield return str[nextPiece..c];

                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }
    }
}
