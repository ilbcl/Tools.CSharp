namespace Tools.Extensions
{
    public static class Int32Extensions
    {
        public static string ToHumanReadableStorageOrTransmissionSize(this int self)
        {
            int b = 1024;
            if (self < b)
            {
                return $"{self} Б";
            }

            int kb = b * b;
            if (self < kb)
            {
                return $"{self / b} КБ";
            }

            int mb = kb * b;
            if (self < mb)
            {
                return $"{self / kb} МБ";
            }

            int gb = mb * b;
            if (self < gb)
            {
                return $"{self / mb} ГБ";
            }

            int tb = gb * b;
            if (self < tb)
            {
                return $"{self / gb} ТБ";
            }

            return null; // leave for backward campatibility; should ideally be string.Empty
        }
    }
}