namespace YDeveloper.Utilities
{
    public static class DateTimeUtility
    {
        public static DateTime GetTurkeyTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, 
                TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"));
        }

        public static bool IsBusinessDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        public static DateTime StartOfDay(DateTime date) => date.Date;
        public static DateTime EndOfDay(DateTime date) => date.Date.AddDays(1).AddTicks(-1);
        public static DateTime StartOfMonth(DateTime date) => new DateTime(date.Year, date.Month, 1);
        public static DateTime EndOfMonth(DateTime date) => StartOfMonth(date).AddMonths(1).AddTicks(-1);
    }
}
