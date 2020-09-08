using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Tools.Configuration;

namespace Tools.Extensions
{
    public static class StringExtensions
    {
        private static Encoding _targetEncoding = Encoding.UTF8;

        private static readonly Regex _numericTemplate = new Regex("^\\d+$", RegexOptions.Compiled);

        private static readonly Dictionary<char, char> KeyBoardChars;

        public static string TruncateWithEllipsis(this string self, int maxLength)
        {
            if (null == self)
            {
                return string.Empty;
            }
            return (self.Length > maxLength) ? self.Substring(0, maxLength) + "..." : self;
        }

        public static string Base64Encode(this string self)
        {
            return Convert.ToBase64String(_targetEncoding.GetBytes(self ?? string.Empty));
        }

        public static string Base64Decode(this string self)
        {
            return _targetEncoding.GetString(Convert.FromBase64String(self ?? string.Empty));
        }

        /// <summary>
        /// Функция сожмет строку (GZIP) и вернет её в кодировке Base64.
        /// </summary>
        /// <returns>Сжатая строка (GZIP) в кодировке Base64</returns>
        public static string Compress(this string uncompressedString)
        {
            byte[] compressedBytes;

            using (var uncompressedStream = new MemoryStream(_targetEncoding.GetBytes(uncompressedString)))
            {
                using (var compressedStream = new MemoryStream())
                {
                    using (var compressorStream = new GZipStream(compressedStream, CompressionLevel.Fastest, true))
                    {
                        uncompressedStream.CopyTo(compressorStream);
                    }

                    compressedBytes = compressedStream.ToArray();
                }
            }

            return Convert.ToBase64String(compressedBytes);
        }

        /// <summary>
        /// Функция восстановит сжатую строку (GZIP) в кодировке Base64 и вернет несжатую строку.
        /// </summary>
        /// <param name="compressedString">Сжатая строка (GZIP) в кодировке Base64</param>
        /// <returns>Восстановленная (несжатая) строка</returns>
        public static string Decompress(this string compressedString)
        {
            byte[] decompressedBytes;

            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

            using (var decompressorStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    decompressorStream.CopyTo(decompressedStream);

                    decompressedBytes = decompressedStream.ToArray();
                }
            }

            return _targetEncoding.GetString(decompressedBytes);
        }

        public static string RemoveWhiteSpaces(this string self, string placeholder = @"")
        {
            return (null != self) ? new string(self.Where(c => !char.IsWhiteSpace(c)).ToArray()) : placeholder;
        }
        public static string LeaveOnlyNumbers(this string self, string placeholder = @"")
        {
            return (null != self) ? new string(self.Where(c => char.IsNumber(c)).ToArray()) : placeholder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string self)
        {
            return string.IsNullOrWhiteSpace(self);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this string self, params object[] args)
        {
            return string.Format(self, args);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this string self, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, self, args);
        }
        public static bool IsNumeric(this string self)
        {
            return !IsNullOrWhiteSpace(self) && _numericTemplate.IsMatch(self);
        }
        private const string SPACE = @" ";
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Space(this string self)
        {
            return SPACE;
        }

        public static bool Contains(this string str, string value, StringComparison comparison) =>
            (str.IndexOf(value, comparison) >= 0);

        public static string KeyBoardSwitch(this string str)
        {
            var chars = str.ToLower().ToCharArray();
            var newStr = (char[])chars.Clone();
            var length = chars.Length;
            for (var i = 0; i < length; i++)
            {
                if (KeyBoardChars.Keys.Contains(chars[i]))
                    newStr[i] = KeyBoardChars[chars[i]];

                if (KeyBoardChars.Values.Contains(chars[i]))
                    newStr[i] = KeyBoardChars.Keys.First(k => KeyBoardChars[k] == chars[i]);
            }
            return new string(newStr);
        }

        [DebuggerStepThrough]
        public static string Join(this IEnumerable<string> values, string separator, bool removeEmptyValues = false)
        {
            if (removeEmptyValues)
                values = values.Where(v => !string.IsNullOrWhiteSpace(v));
            return string.Join(separator, values);
        }

        [DebuggerStepThrough]
        public static bool IsMatch(this string value, string pattern, RegexOptions options) => Regex.IsMatch(value, pattern, options);

        public static bool IsEmail(this string value) => !string.IsNullOrEmpty(value) && Regex.IsMatch(value, RegEx.Email);

        [DebuggerStepThrough]
        public static bool IsMatch(this string value, string pattern) => value.IsMatch(pattern, RegexOptions.None);

        [DebuggerStepThrough]
        public static string Replace(this string input, string pattern, string replacement, RegexOptions options) =>
            Regex.Replace(input, pattern, replacement, options);

        private static string StripTagsCharArray(string rsource)
        {
            var source = $"{rsource}";
            var array = new char[source.Length];
            var arrayIndex = 0;
            var inside = false;

            foreach (var @let in source)
            {
                switch (@let)
                {
                    case '<':
                        inside = true;
                        continue;
                    case '>':
                        inside = false;
                        continue;
                }
                if (inside) continue;
                array[arrayIndex] = @let;
                arrayIndex++;
            }
            return new string(array, 0, arrayIndex);
        }

        public static string StripHtml(this string source) => StripTagsCharArray(source)
                .Replace("&nbsp;", " ")
                .Replace("&quot;", "\"")
                .Replace("\r\n", " ").Replace("  ", string.Empty).Trim();

        [DebuggerStepThrough]
        public static string ToUppercaseFirstLetter(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            var a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string GetHash(this string str, Func<byte[], byte[]> hasher)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var hashed = hasher(bytes);
            var sBuilder = new StringBuilder();
            foreach (var b in hashed)
                sBuilder.Append(b.ToString("x2"));
            var res = sBuilder.ToString();
            return res;
        }

        public static string GetMd5Hash(this string str)
        {
            var hasher = new Func<byte[], byte[]>(bytes =>
            {
                var md5 = MD5.Create();
                var hashed = md5.ComputeHash(bytes);
                return hashed;
            });
            var res = GetHash(str, hasher);
            return res;
        }

        public static string GetSha256Hash(this string str)
        {
            var hasher = new Func<byte[], byte[]>(bytes =>
            {
                var sha256 = SHA256.Create();
                var hashed = sha256.ComputeHash(bytes);
                return hashed;
            });
            var res = GetHash(str, hasher);
            return res;
        }

        public static string GetHmacmd5Hash(this string str, string key)
        {
            var hasher = new Func<byte[], byte[]>(bytes =>
            {
                var md5 = new HMACMD5(Encoding.UTF8.GetBytes(key)) { HashName = "md5" };
                var hashed = md5.ComputeHash(bytes);
                return hashed;
            });
            var res = GetHash(str, hasher);
            return res;
        }

        public static string GetBaseString(this string property) =>
            string.Concat(Encoding.UTF8.GetBytes(property).Length, property);

        public static string AddParameterToUrl(this string url, string name, string value)
        {
            var hash = string.Empty;
            if (url.IndexOf('#') > -1)
            {
                hash = url.Substring(url.IndexOf('#'));
                url = url.Remove(url.IndexOf('#'));
            }

            var newParameter = $"{name}={HttpUtility.UrlEncode(value)}";
            newParameter = url.IndexOf("?") < 0 ? "?" + newParameter : "&" + newParameter;

            return string.Concat(url, newParameter, hash);
        }

        public static string RemoveParameterFromUrl(this string url, string name)
        {
            if (url.IndexOf("?") < 0)
                return url;

            var hash = string.Empty;
            if (url.IndexOf('#') > -1)
            {
                hash = url.Substring(url.IndexOf('#'));
                url = url.Remove(url.IndexOf('#'));
            }

            var leftAndQuery = url.Split(new[] { "?" }, StringSplitOptions.None);

            if (leftAndQuery[1].IndexOf("&") < 0)
                return url.Substring(0, url.LastIndexOf("?")) + hash;

            var splitedQuery = leftAndQuery[1].Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            var newQuery = splitedQuery.Where(spl => spl.Split(new[] { "=" }, StringSplitOptions.None)[0] != name);

            if (newQuery.Count() < 0)
                return leftAndQuery[0] + hash;

            return $"{leftAndQuery[0]}?{newQuery.Join("&")}{hash}";
        }

        public static string SplitInParts(this string s, int partLength, bool fromEnd)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));
            if (fromEnd)
            {
                var charArray = s.ToCharArray();
                Array.Reverse(charArray);
                s = new string(charArray);
            }
            var parts = new List<string>();
            for (var i = 0; i < s.Length; i += partLength)
            {
                var subStr = s.Substring(i, Math.Min(partLength, s.Length - i));
                if (fromEnd)
                {
                    var charArray = subStr.ToCharArray();
                    Array.Reverse(charArray);
                    subStr = new string(charArray);
                }
                parts.Add(subStr);
            }
            if (fromEnd)
                parts.Reverse();
            return parts.Join(" ");
        }
    }
}