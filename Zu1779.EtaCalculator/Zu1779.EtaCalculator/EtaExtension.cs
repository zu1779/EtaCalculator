namespace Zu1779.EtaCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EtaExtension
    {
        public static IEnumerable<T> TrackProgress<T>(this IEnumerable<T> ien, IEtaCalculator? calculator = null) 
            => calculator == null ? ien : calculator.TrackProgress(ien);

        public static IEnumerable<T> TrackForEachProgress<T>(this IEnumerable<T> ien, TimeSpan tsProgress, Action<EtaCalculator, T?> progressAction)
            => ien.TrackForEachProgress(tsProgress, progressAction, ien => ien.Count(), (_, _) => 1);

        public static IEnumerable<T> TrackForEachProgress<T>(this IEnumerable<T> ien, TimeSpan tsProgress, Action<EtaCalculator, T?> progressAction, 
            Func<IEnumerable<T>, double?> total, Func<T, int, double> advance) {

            EtaCalculator eta = new EtaCalculator();
            eta.TimeThreshold = tsProgress;
            eta.TimeProgress += (s, a) => progressAction((EtaCalculator)s!, (T?)eta.Tag);
            eta.SetTotal(total(ien));
            eta.Done = 0;
            eta.Start();
            foreach (var (item, index) in ien.Select((item, index) => (item, index))) {
                eta.Tag = item;
                yield return item;
                eta.Advance(advance(item, index));
            }
            eta.Complete();
        }

        // public static IEnumerable<T> TrackForEachProgress<T>(this IEnumerable<T> ien, TimeSpan tsProgress, Action<EtaCalculator> progressAction) {
        //     EtaCalculator eta = new EtaCalculator();
        //     eta.TimeThreshold = tsProgress;
        //     eta.TimeProgress += (s, a) => progressAction((EtaCalculator)s);
        //     eta.SetTotal(ien.Count());
        //     eta.Done = 0;
        //     eta.Start();
        //     foreach (var item in ien) {
        //         yield return item;
        //         eta.Advance(1);
        //     }
        //     eta.Complete();
        // }
    }
}
