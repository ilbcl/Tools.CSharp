namespace Tools.Extensions
{
    public static class DecimalExtension
    {
        public static string ToStringWithTriads(this decimal value)
        {
            return Utility.ConvertToMoneyWithTriads(value);
        }
    }
}
