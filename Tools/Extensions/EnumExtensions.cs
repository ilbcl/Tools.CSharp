using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Tools.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static string GetDescription(this Enum enumMember, int index = 0)
        {
            if (enumMember == null)
                return null;

            if (enumMember.GetType().GetField(enumMember.ToString()).IsDefined(typeof(DescriptionAttributes), false))
            {
                return
                    (enumMember.GetType().GetField(enumMember.ToString()).GetCustomAttributes(typeof(DescriptionAttributes), false)[0]
                     as DescriptionAttributes).Descriptions.Skip(index).FirstOrDefault() ?? enumMember.ToString();
            }

            return enumMember.ToString();
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class DescriptionAttributes : Attribute
    {
        private readonly string[] _descriptions;

        public DescriptionAttributes(string description)
        {
            _descriptions = new[] { description };
        }

        public DescriptionAttributes(params string[] descriptions)
        {
            _descriptions = descriptions;
        }

        public IEnumerable<string> Descriptions => _descriptions;
    }
}