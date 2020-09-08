using System;

namespace Tools
{
    public class Range<T> where T : IComparable
    {
        private T _min;
        private T _max;

        public Range(T min, T max)
        {
            SetRange(min, max);
        }

        public T Min
        {
            get { return _min; }
        }

        public T Max
        {
            get { return _max; }
        }

        public void SetRange(T tMin, T tMax)
        {
            if (Min.CompareTo(Max) <= 0)
            {
                _min = tMin;
                _max = tMax;
            }
            else
            {
                _min = tMax;
                _max = tMin;
            }
        }

        /// <summary>
        /// Проверяет пересекается ли указанный диапазон с текущим
        /// </summary>
        /// <param name="range">Диапазон</param>
        /// <returns>Возвращает <b>true</b> если диапазоны перекаются; иначе <b>false</b></returns>
        public bool Intersects(Range<T> range)
        {
            if ((range == null))
            {
                throw new ArgumentNullException();
            }

            dynamic minComparisonResult = _min.CompareTo(range.Min);
            dynamic maxComparisonResult = _max.CompareTo(range.Max);

            if (minComparisonResult == 0 || maxComparisonResult == 0)
            {
                return true;
            }

            if (minComparisonResult < 0 && range.Min.CompareTo(_max) <= 0)
            {
                return true;
            }

            if (_min.CompareTo(range.Max) <= 0 && maxComparisonResult > 0)
            {
                return true;
            }

            return range.HasAllValues(this);
        }

        /// <summary>
        /// Проверяет входит ли указанный диапазон в текущий
        /// </summary>
        /// <param name="range">Диапазон</param>
        /// <returns>Возвращает <b>true</b> если указанный диапазон входит в текущий; иначе <b>false</b></returns>
        public bool HasAllValues(Range<T> range)
        {
            if ((range == null))
            {
                throw new ArgumentNullException();
            }

            return _min.CompareTo(range.Max) <= 0 && range.Max.CompareTo(_max) <= 0 && _min.CompareTo(range.Min) <= 0 && range.Min.CompareTo(_max) <= 0;
        }

        /// <summary>
        /// Проверяет входит ли указанное значение в текущий диапазон
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns>Возвращает <b>true</b> если указанное значение входит в текущий диапазон; иначе <b>false</b></returns>
        public bool HasValue(T value)
        {
            return _min.CompareTo(value) <= 0 && _max.CompareTo(value) >= 0;
        }

        /// <summary>
        /// Проверяет пересекаются ли указанные диапазоны.
        /// </summary>
        /// <param name="range1">Первый диапазон</param>
        /// <param name="range2">Второй диапазон</param>
        /// <returns>Возвращает <b>true</b> если диапазоны перекаются; иначе <b>false</b></returns>
        public static bool Intersects(Range<T> range1, Range<T> range2)
        {
            if ((range1 == null) || (range2 == null))
            {
                throw new ArgumentNullException();
            }

            return range1.Intersects(range2);
        }

        /// <summary>
        /// Проверяет входит ли второй диапазон в первый
        /// </summary>
        /// <param name="range1">Первый диапазон</param>
        /// <param name="range2">Второй диапазон</param>
        /// <returns>Возвращает <b>true</b> если второй диапазон входит в первый; иначе <b>false</b></returns>
        public static bool HasAllValues(Range<T> range1, Range<T> range2)
        {
            if ((range1 == null) || (range2 == null))
            {
                throw new ArgumentNullException();
            }

            return range1.HasAllValues(range2);
        }
    }
}