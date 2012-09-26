using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public static long ToTimestamp(this DateTime value)
        {
            return (long)(DateTime.Now.Subtract(epoch).TotalMilliseconds);
        }

        public static DateTime ToDateTime(this long value)
        {
            return new DateTime(value * 10000 + epoch.Ticks);
        }
    }
}
