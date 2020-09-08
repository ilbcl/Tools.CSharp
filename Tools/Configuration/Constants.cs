using System.Collections.Generic;
using System.Linq;

namespace Tools.Configuration
{
    public static class RegEx
    {
        public const string Alpha = @"^[a-zA-Z]+$";
        public const string Numeric = @"^[0-9]+$";
        public const string Decimal = @"^[0-9]+([\.\,][0-9]+)?$";
        public const string AlphaNumeric = @"^[a-zA-Z0-9]+$";
        public const string Email = @"^(\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)$";
        public const string AlphaWithCyrillic = @"^[a-zа-яёА-ЯЁA-Z\s]+$";

        public static string StringArrayToInRegex(IEnumerable<string> words)
        {
            var ret = string.Empty;
            ret += "^";
            ret = words.Aggregate(ret, (current, word) => current + ("(" + word + ")|"));
            ret = ret.Substring(0, ret.Length - 1);
            ret += "$";
            return ret;
        }

        public static string StringArrayToNotInRegex(IEnumerable<string> words)
        {
            var ret = string.Empty;
            ret += "^((?!";
            ret = words.Aggregate(ret, (current, word) => current + (word + "|"));
            ret = ret.Substring(0, ret.Length - 1);
            ret += ").)*$";
            return ret;
        }
    }
}