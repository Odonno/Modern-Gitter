using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Gitter.Converters
{
    public class DateTimeOffsetToTimeAgoConverter : IValueConverter
    {
        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("ConvertersResources");


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dateTimeOffset = (DateTime)value;
            var timeSpanDiff = new DateTimeOffset(DateTime.Now).Subtract(dateTimeOffset);

            if (timeSpanDiff.TotalSeconds < 60)
                return string.Format(_resourceLoader.GetString("SecondsAgo"),
                    timeSpanDiff.Seconds,
                    timeSpanDiff.Seconds > 1 ? "s" : "");

            if (timeSpanDiff.TotalMinutes < 60)
                return string.Format(_resourceLoader.GetString("MinutesAgo"),
                    timeSpanDiff.Minutes,
                    timeSpanDiff.Minutes > 1 ? "s" : "");

            if (timeSpanDiff.TotalHours < 24)
                return string.Format(_resourceLoader.GetString("HoursAgo"),
                    timeSpanDiff.Hours,
                    timeSpanDiff.Hours > 1 ? "s" : "");

            return string.Format(_resourceLoader.GetString("DaysAgo"),
                timeSpanDiff.Days,
                timeSpanDiff.Days > 1 ? "s" : "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
