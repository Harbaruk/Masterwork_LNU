using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Starter.API.Extensions
{
    public static class StringExtensions
    {
        public static string FirstLetterToLower(this string str)
        {
            Debug.Assert(str != null);
            if (str.Length == 0)
            {
                return String.Empty;
            }
            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}