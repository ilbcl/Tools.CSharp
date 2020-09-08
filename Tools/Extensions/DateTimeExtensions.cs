using System;

namespace Tools.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime Trim(this DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - date.Ticks % roundTicks);
        }

        public static DateTime CalculateInsPeriodEndDateForDurationInMonths(this DateTime contractBeginDate, int durationInMonths)
        {
            if (durationInMonths <= 0)
            {
                return contractBeginDate;
            }

            var beginDay = contractBeginDate.Day;
            var beginMonth = contractBeginDate.Month;
            var beginYear = contractBeginDate.Year;

            var endDay = beginDay - 1;
            var endMonth = beginMonth + durationInMonths;
            var endYear = beginYear;

            if (endMonth > 12) {
                var deltaYear = Convert.ToInt32(Math.Floor((decimal)(endMonth - 1) / 12));
                endYear = endYear + deltaYear;
                endMonth = endMonth - deltaYear * 12;

                if (endMonth == 0)
                {
                    endMonth = 12;
                    endYear = endYear - 1;
                }
            }

            if (endDay == 0) {
                endMonth = endMonth - 1;

                if (endMonth == 0)
                {
                    endMonth = 12;
                    endYear = endYear - 1;
                }

                endDay = DateTime.DaysInMonth(endYear, endMonth);
            }
            else {
                var monthEndDay = DateTime.DaysInMonth(endYear, endMonth);
                if (monthEndDay < endDay) {
                    endDay = monthEndDay;
                }
            }

            return new DateTime(endYear, endMonth, endDay, 23, 59, 59, contractBeginDate.Kind);
        }
    }
}
