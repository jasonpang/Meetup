using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> TakeLastN<T>(this IEnumerable<T> source, int n)
        {
            if (source == null)
                throw new ArgumentNullException("Source cannot be null");

            int goldenIndex = source.Count() - n;
            return source.SkipWhile((val, index) => index < goldenIndex);
        }
    }
}