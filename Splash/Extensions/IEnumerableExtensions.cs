using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> LastN<T>(this IEnumerable<T> collection, int n)
        {
            return collection.Skip(Math.Max(0, collection.Count() - n)).Take(n); ;
        }
    }
}