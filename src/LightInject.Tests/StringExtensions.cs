using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LightInject.Tests
{
    public static class StringExtensions
    {
        public static bool Matches(this String s, string pattern, bool ignoreCase = true)
        {
            if (ignoreCase)
            {
                return ((Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase))) ? true : false;
            }
            else
            {
                return (Regex.IsMatch(s, pattern)) ? true : false;
            }
        }
    }
}
