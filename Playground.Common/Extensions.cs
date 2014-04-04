using System;

namespace Playground.Common
{
    public static class Extensions
    {
        public static string StripSlash(this string input)
        {
            if (!String.IsNullOrEmpty(input) && (input.EndsWith("/") || input.EndsWith("\\")))
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }
    }
}