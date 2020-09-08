using System;

namespace Tools
{
    /// <summary>
    /// Represents a time range which is defined with a START and END time (bounds).
    /// Both bounds are stored internally as Timespan objects.
    /// - it a range for one day (00:00-24:00) which means it is date independend!
    /// - precision is reduced to hours and minutes
    /// - bounds can be set directly with start and end properties or with SetEnd() und SetStart() methods.
    /// - 0:00 at the end is displayed as 24:00
    /// </summary>
    public class TimeRange
    {
        /// <summary>
        /// Gets the amount of minutes of a whole day (24 hours)
        /// </summary>
        public const int MinutesOfTheDay = 1440;
        private bool __initialized = false;

        /// <summary>
        /// Instantiates a new time range with a given start- and end-timespan
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public TimeRange(TimeSpan start, TimeSpan end)
            : this()
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Instantiates a new time range for a given Start and End with hours and minutes
        /// </summary>
        /// <param name="startH"></param>
        /// <param name="startM"></param>
        /// <param name="endH"></param>
        /// <param name="endM"></param>
        public TimeRange(int startH, int startM, int endH, int endM)
            : this()
        {
            Start = new TimeSpan(startH, startM, 0);
            End = new TimeSpan(endH, endM, 0);
        }

        /// <summary>
        /// Instantiates a new timerange instance with the smallest possible lower and highest possible upper bound (00:00-24:00)
        /// </summary>
        public TimeRange()
        {
            Start = new TimeSpan(0, 0, 0);
            End = new TimeSpan(24, 0, 0);
            __initialized = true;
        }

        /// <summary>
        /// represents the timerange in a format like: 12:00-14:00
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FormatTime(Start) + '-' + FormatTime(End);
        }

        /// <summary>
        /// Formats the time properly
        /// </summary>
        /// <param name="timefield"></param>
        /// <returns></returns>
        public static string FormatTime(TimeSpan timefield)
        {
            if (timefield.Hours == 0 && timefield.Minutes == 0)
            {
                return "24:00";
            }
            else
            {
                return timefield.Hours.ToString().PadLeft(2, '0') + ":" + timefield.Minutes.ToString().PadLeft(2, '0');
            }
        }

        /// <summary>
        /// Sets the hours and minutes of the start
        /// </summary>
        /// <param name="h"></param>
        /// <param name="m"></param>
        public void SetStart(int h, int m)
        {
            Start = new TimeSpan(h, m, 0);
        }

        /// <summary>
        /// Sets the hours and minutes of the end
        /// </summary>
        /// <param name="h"></param>
        /// <param name="m"></param>
        public void SetEnd(int h, int m)
        {
            End = new TimeSpan(h, m, 0);
        }

        /// <summary>
        /// Checks equality
        /// </summary>
        /// <param name="obj">another TimeRange instance</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            TimeRange tr = (TimeRange)obj;
            return ToString() == tr.ToString();
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        /// <summary>
        /// Checks if this timerange clashes with another one.
        /// <constraint>bounds are exclusive</constraint>
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if there is a clash</returns>
        public bool Clashes(TimeRange other)
        {
            return Clashes(other, false);
        }

        /// <summary>
        /// Checks if this timerange clashes with another one.
        /// </summary>
        /// <param name="other">The timerange to compare</param>
        /// <param name="inclusive">Use Inclusive or SemiExclusive Boundaries</param>
        /// <returns></returns>
        public bool Clashes(TimeRange other, bool inclusive)
        {
            if (inclusive)
            {
                return (other.Start <= Start && other.End >= End) ||
                    (other.Start < Start && other.End >= Start) ||
                    (other.End > End && other.Start <= End) ||
                    (other.Start >= Start && other.End <= End);
            }
            else
            {
                return (other.Start < Start && other.End > End) ||
                    (other.Start < Start && other.End > Start) ||
                    (other.End > End && other.Start < End) ||
                    (other.Start >= Start && other.End <= End);
            }
        }

        /// <summary>
        /// if Equals is overriden this operator needs to overloaded
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(TimeRange left, TimeRange right)
        {
            if ((object)left == null && (object)right == null)
            {
                return true;
            }
            else if ((object)left != null && (object)right != null)
            {
                return left.ToString() == right.ToString();
            }
            return false;
        }

        /// <summary>
        /// if Equals is overriden this operator needs to overloaded
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(TimeRange left, TimeRange right)
        {
            return !(left == right);
        }

        /// <summary>
        /// true is NOW() within a range
        /// </summary>
        /// <returns></returns>
        public bool IsNow()
        {
            DateTime d = DateTime.Now;
            return IsIn(new TimeSpan(d.Hour, d.Minute, d.Second));
        }

        /// <summary>
        /// Checks if a given time value is within the bounds of the instance
        /// <constraint>Bounds are inclusive!</constraint>
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public bool IsIn(TimeSpan timespan)
        {
            return (Start.Ticks <= timespan.Ticks && End.Ticks >= timespan.Ticks);
        }

        /// <summary>
        /// Checks if a given timerange is within the bounds of the instance
        /// <constraint>Bounds are inclusive</constraint>
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public bool IsIn(TimeRange timerange)
        {
            return IsIn(timerange.Start) && IsIn(timerange.End);
        }

        /// <summary>
        /// Tries to parse a given string into a TimeRange object
        /// Strings of the following format are parsed:
        /// - 1:00-12:20
        /// - 1:00:10-12:20:00
        /// - 01:00-12:30
        /// - 1:0-12:30
        /// - 100-1230
        /// - 0100-1230
        /// - 1-1230
        /// </summary>
        /// <param name="timeRangeString"></param>
        /// <returns>null if could not be parsed</returns>
        public static TimeRange Parse(string input)
        {
            string[] parts = input.Split('-');
            if (parts.Length == 2)
            {
                TimeSpan? start = ParseTimeSpan(parts[0]);
                TimeSpan? end = ParseTimeSpan(parts[1]);
                if (start == null || end == null || start.Value > end.Value) return null;
                return new TimeRange((start ?? new TimeSpan()), (end ?? new TimeSpan()));
            }
            return null;
        }

        /// <summary>
        /// Takes a given string which represents a time (hours and minutes) and tries to parse it into a Timespan.
        /// It recognizes strings in the following formats: 10:30, 10, 10:0, 10:3, 1030
        /// </summary>
        /// <param name="input">your string which should be parsed</param>
        /// <returns>if cannot parse then null is returned</returns>
        public static TimeSpan? ParseTimeSpan(string input)
        {
            string[] parts = input.Split(':');
            int h = 0, m = 0;
            if (parts.Length >= 2)
            {
                if (int.TryParse(parts[0], out h) && int.TryParse(parts[1], out m))
                {
                    if (h > 24 || m > 60) return null;
                    return new TimeSpan(h, m, 0);
                }
                return null;
            }
            else if (input.Length > 0)
            {
                switch (input.Length)
                {
                    case 1:
                        if (int.TryParse(input, out h))
                        {
                            if (h > 24) return null;
                            return new TimeSpan(h, 0, 0);
                        }
                        break;
                    case 2:
                        goto case 1;
                    case 3:
                        goto case 4;
                    case 4:
                        string minutes = input.Substring(input.Length - 2, 2);
                        string hours = input.Substring(0, ((input.Length == 3) ? 1 : 2));
                        if (int.TryParse(hours, out h) && int.TryParse(minutes, out m))
                        {
                            if (h > 24 || m > 60) return null;
                            return new TimeSpan(h, m, 0);
                        }
                        break;
                }
            }
            return null;
        }

        #region Properties

        private TimeSpan _start;
        private TimeSpan _end;

        /// <summary>
        /// Gets/Sets the Start bound
        /// <constraint>Must be before end</constraint>
        /// <constraint>Minimum is 00:00; Maximum is 23:59</constraint>
        /// </summary>
        public TimeSpan Start
        {
            get { return _start; }
            set
            {
                if (__initialized)
                {
                    if (value.TotalMinutes > TimeRange.MinutesOfTheDay - 1) throw new ArgumentException("Min. 00:00, Max. 23:59");
                    if (value >= End) throw new ArgumentException("Start must be before End.");
                }
                _start = value;
            }
        }

        /// <summary>
        /// Gets/Sets the End bound
        /// <constraint>must be after start</constraint>
        /// <constraint>Minimum is 00:01; Maximum is 24:00</constraint>
        /// </summary>
        public TimeSpan End
        {
            get { return _end; }
            set
            {
                if (__initialized)
                {
                    if (value.TotalMinutes < 1 || value.TotalMinutes > TimeRange.MinutesOfTheDay) throw new ArgumentException("Min. 00:01, Max. 24:00");
                    if (value <= Start) throw new ArgumentException("End must be after Start.");
                }
                _end = value;
            }
        }

        #endregion
    }
}
