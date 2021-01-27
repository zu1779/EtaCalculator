namespace Zu1779.EtaCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EtaExtension
    {
        public static IEnumerable<T> TrackProgress<T>(this IEnumerable<T> ien, IEtaCalculator calculator = null) =>
            calculator == null ? ien : calculator.TrackProgress(ien);
    }
}
