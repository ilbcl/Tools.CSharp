using System;
using System.Text;

namespace Tools.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToFlattenedExceptionInfo(this Exception exception)
        {
            StringBuilder flattenedInfo = new StringBuilder();
            while (exception != null)
            {
                flattenedInfo.AppendLine(exception.Message);
                flattenedInfo.AppendLine(exception.StackTrace);

                exception = exception.InnerException;

                if (exception == null)
                {
                    break;
                }

                flattenedInfo.AppendLine("==================================================").AppendLine();
            }
            return flattenedInfo.ToString();
        }
    }
}