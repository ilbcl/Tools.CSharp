using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Tools.Extensions
{
    public static class StringExtensions
    {
        private static Encoding _targetEncoding = Encoding.UTF8;

        private static readonly Regex _numericTemplate = new Regex("^\\d+$", RegexOptions.Compiled);

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
    }
}
