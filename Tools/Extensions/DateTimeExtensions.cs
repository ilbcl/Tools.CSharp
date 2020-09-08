using System;

namespace Tools.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime Trim(this DateTime date, long roundTicks) => new DateTime(date.Ticks - date.Ticks % roundTicks);

        public static string ToDateFormat(this DateTime dt) => dt.ToString("dd.MM.yyyy");

        public static string ToDateAndTimeFormat(this DateTime dt) => dt.ToString("dd.MM.yyyy HH:mm");

        public static string ToDateAndTimeFormatReversed(this DateTime dt) => dt.ToString("HH:mm dd.MM.yyyy");
    }
}