using System;

namespace Tools
{
    [Serializable]
    public class DateTimeRange : IEquatable<DateTimeRange>
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _isValid;

        /// <summary>
        /// Создает новый экземпляр периода
        /// </summary>
        public DateTimeRange()
        {
            _isValid = false;
        }

        /// <summary>
        /// Создает новый экземпляр периода с указанными датами
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        public DateTimeRange(DateTime startDate, DateTime endDate)
        {
            SetDates(startDate, endDate);
        }

        /// <summary>
        /// Начальная дата текущего периода
        /// </summary>
        /// <returns>Возвращает значение начальной даты текущего периода</returns>
        public DateTime StartDate
        {
            get { return _startDate; }
        }

        /// <summary>
        /// Конечная дата текущего периода
        /// </summary>
        /// <returns>Возвращает значение конечной даты текущего периода</returns>
        public DateTime EndDate
        {
            get { return _endDate; }
        }

        /// <summary>
        /// Указывает установлены ли значения начальной и конечной даты
        /// </summary>
        /// <returns>Возвращает <b>true</b> если значения дат установлены; иначе <b>false</b></returns>
        public bool IsValid
        {
            get { return _isValid; }
        }

        /// <summary>
        /// Максимально возможный период
        /// </summary>
        /// <returns>Возвращает максимально возможный период</returns>
        public static DateTimeRange MaxPeriod
        {
            get { return new DateTimeRange(DateTime.MinValue.AddMilliseconds(1), DateTime.MaxValue); }
        }

        /// <summary>
        /// Устанавливает новые даты текущего периода
        /// </summary>
        /// <param name="dtStartDate">Начальная дата</param>
        /// <param name="dtEndDate">Конечная дата</param>
        public void SetDates(DateTime dtStartDate, DateTime dtEndDate)
        {
            if (dtStartDate > dtEndDate)
            {
                throw new InvalidOperationException("Дата начала периода больше даты окончания");
            }

            _startDate = dtStartDate;
            _endDate = dtEndDate;
            _isValid = Utility.IsDateTimeAssigned(dtStartDate) && Utility.IsDateTimeAssigned(dtEndDate);
        }

        /// <summary>
        /// Проверяет пересекается ли указанный период с текущим
        /// </summary>
        /// <param name="period">Период</param>
        /// <returns>Возвращает <b>true</b> если периоды перекаются; иначе <b>false</b></returns>
        public bool Intersects(DateTimeRange period)
        {
            if ((period == null))
            {
                throw new ArgumentNullException();
            }

            if (!(period.IsValid) || !(IsValid))
            {
                return false;
            }

            if (_startDate == period.StartDate || _endDate == period.EndDate)
            {
                return true;
            }

            if (_startDate < period.StartDate && period.StartDate <= _endDate)
            {
                return true;
            }

            if (_startDate <= period.EndDate && period.EndDate < _endDate)
            {
                return true;
            }

            return period.HasAllDates(this);
        }

        /// <summary>
        /// Проверяет входит ли указанный период в текущий
        /// </summary>
        /// <param name="period">Период</param>
        /// <returns>Возвращает <b>true</b> если указанный период входит в текущий; иначе <b>false</b></returns>
        public bool HasAllDates(DateTimeRange period)
        {
            if ((period == null))
            {
                throw new ArgumentNullException();
            }

            if (!period.IsValid || !IsValid)
            {
                return false;
            }

            return _startDate <= period.EndDate && period.EndDate <= _endDate && _startDate <= period.StartDate && period.StartDate <= _endDate;
        }

        /// <summary>
        /// Проверяет входит ли указанная дата в текущий период
        /// </summary>
        /// <param name="someDate">Дата</param>
        /// <returns>Возвращает <b>true</b> если указанная дата входит в текущий период; иначе <b>false</b></returns>
        public bool HasDate(DateTime someDate)
        {
            if (!IsValid)
            {
                return false;
            }

            return _startDate <= someDate && someDate <= _endDate;
        }

        /// <summary>
        /// Проверяет пересекаются ли указанные периоды.
        /// </summary>
        /// <param name="period1">Первый период</param>
        /// <param name="period2">Второй период</param>
        /// <returns>Возвращает <b>true</b> если периоды перекаются; иначе <b>false</b></returns>
        public static bool Intersects(DateTimeRange period1, DateTimeRange period2)
        {
            if (period1 == null || period2 == null)
            {
                throw new ArgumentNullException();
            }

            return period1.Intersects(period2);
        }

        /// <summary>
        /// Проверяет входит ли второй период в первый
        /// </summary>
        /// <param name="period1">Первый период</param>
        /// <param name="period2">Второй период</param>
        /// <returns>Возвращает <b>true</b> если второй период входит в первый; иначе <b>false</b></returns>
        public static bool HasAllDates(DateTimeRange period1, DateTimeRange period2)
        {
            if (period1 == null || period2 == null)
            {
                throw new ArgumentNullException();
            }

            return period1.HasAllDates(period2);
        }

        public static bool operator ==(DateTimeRange firstRange, DateTimeRange secondRange)
        {
            return RangesAreEqual(firstRange, secondRange);
        }

        public static bool operator !=(DateTimeRange firstRange, DateTimeRange secondRange)
        {
            return !RangesAreEqual(firstRange, secondRange);
        }

        public bool EqualsToRange(DateTimeRange other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!IsValid || !other.IsValid)
            {
                return false;
            }

            return StartDate == other.StartDate && EndDate == other.EndDate;
        }

        bool IEquatable<DateTimeRange>.Equals(DateTimeRange other)
        {
            return EqualsToRange(other);
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                dynamic range = obj as DateTimeRange;
                return EqualsToRange(range);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (!IsValid)
            {
                return base.GetHashCode();
            }
            return StartDate.GetHashCode() | EndDate.GetHashCode();
        }

        public static bool RangesAreEqual(DateTimeRange firstRange, DateTimeRange secondRange)
        {
            if (ReferenceEquals(firstRange, secondRange))
            {
                return true;
            }

            return !ReferenceEquals(firstRange, null) ? firstRange.EqualsToRange(secondRange) : secondRange.EqualsToRange(null);
        }

        public override string ToString()
        {
            if (IsValid)
            {
                return $"{StartDate.ToShortDateString()} {StartDate.ToShortTimeString()} - {EndDate.ToShortDateString()} {EndDate.ToShortTimeString()}";
            }
            return base.ToString();
        }
    }
}