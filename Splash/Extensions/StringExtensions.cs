using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Extensions
{
    public static class StringExtensions
    {
        public static bool IsSet(this String str)
        {
            return !String.IsNullOrEmpty(str);
        }

        public static IEnumerable<T> AsEnumerable<T>(this String str)
        {
            return str.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries).ToList().Cast<T>();
        }
    }
}