namespace Zu1779.EtaCalculator
{
    using System;

    public static class EtaCalculatorExtension
    {
        public static string ToUMStr(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalMilliseconds < 4000) return $"{timeSpan.TotalMilliseconds:n0} ms.";
            else if (timeSpan.TotalSeconds < 120) return $"{timeSpan.TotalSeconds:n0} s.";
            else if (timeSpan.TotalMinutes < 180) return $"{timeSpan.TotalMinutes:n2} min.";
            else if (timeSpan.TotalHours < 48) return $"{timeSpan.TotalHours:n2} h.";
            else return $"{timeSpan.TotalDays:n2} d.";
        }

        public static string ToUMStr(this TimeSpan? timeSpanNullable)
        {
            if (!timeSpanNullable.HasValue) return "-";
            return timeSpanNullable.Value.ToUMStr();
        }
    }
}
