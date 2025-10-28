using System;

namespace Toon.Utils
{
    internal class DateTimeUtility
    {
        internal static string DefaultDateTimeOutputFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        internal static DateTime ConvertDateTime(DateTime dateTime, DateTimeKind dateTimeKind)
        {
            return dateTimeKind switch
            {
                DateTimeKind.Local => SwitchToLocalTime(dateTime),
                DateTimeKind.Unspecified => new DateTime(dateTime.Ticks, dateTimeKind),
                DateTimeKind.Utc => SwitchToUtcTime(dateTime),
                _ => throw new InvalidOperationException("Invalid dateTimeKind specified."),
            };
        }

        internal static string ToIsoString(DateTime dateTime, DateTimeKind dateTimeKind)
        {
            dateTime = ConvertDateTime(dateTime, dateTimeKind);

            return dateTime.ToString(DefaultDateTimeOutputFormat);
        }

        internal static string ToIsoString(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString(DefaultDateTimeOutputFormat);
        }

        private static DateTime SwitchToLocalTime(DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Unspecified => new DateTime(dateTime.Ticks, DateTimeKind.Local),
                DateTimeKind.Utc => dateTime.ToLocalTime(),
                DateTimeKind.Local => dateTime,
                _ => dateTime,
            };
        }

        private static DateTime SwitchToUtcTime(DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Unspecified => new DateTime(dateTime.Ticks, DateTimeKind.Utc),
                DateTimeKind.Utc => dateTime,
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                _ => dateTime,
            };
        }
    }
}
