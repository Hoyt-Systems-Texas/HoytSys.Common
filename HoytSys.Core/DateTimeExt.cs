using System;

namespace HoytSys.Core
{
    public static class DateTimeExt
    {
        private static readonly DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public static long ToUnix(this DateTime dateTime)
        {
            return(long) dateTime.ToUniversalTime().Subtract(
                UnixTime).TotalMilliseconds;
        }

        /// <summary>
        ///     Used to create a date from a unix timestamp.
        /// </summary>
        /// <param name="date">The date number to create the date time for.</param>
        /// <returns>The date time.</returns>
        public static DateTime FromUnix(long date)
        {
            return UnixTime.AddMilliseconds(date);
        }
    }
}