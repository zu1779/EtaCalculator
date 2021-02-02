namespace Zu1779.EtaCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IEtaCalculator
    {
        double? Total { get; set; }
        IEtaCalculator SetTotal(double? total);
        double Done { get; set; }
        IEtaCalculator SetDone(double done)
        {
            Done = done;
            return this;
        }
        double? DoneProportion { get; set; }
        IEtaCalculator SetDoneProportion(double? value)
        {
            DoneProportion = value;
            return this;
        }
        double? ToDo { get; set; }
        IEtaCalculator SetToDo(double? value)
        {
            ToDo = value;
            return this;
        }
        double? ToDoProportion { get; set; }
        IEtaCalculator SetTodoProportion(double? value)
        {
            ToDoProportion = value;
            return this;
        }

        TimeSpan? TotalTime { get; }
        TimeSpan DoneTime { get; }
        TimeSpan? ToDoTime { get; }

        double? CountThreshold { get; set; }
        IEtaCalculator SetCountThreshold(double? threshold = null)
        {
            CountThreshold = threshold;
            return this;
        }
        TimeSpan? TimeThreshold { get; set; }
        IEtaCalculator SetTimeThreashold(TimeSpan? threshold = null)
        {
            TimeThreshold = threshold;
            return this;
        }

        double? ItemPerSecond { get; }

        /// <summary>
        /// Starts, or resumes, measuring elapsed time for an interval.
        /// </summary>
        IEtaCalculator Start();
        /// <summary>
        /// Advance task progress. Alias of setting <see cref="Done"/> property.
        /// </summary>
        /// <param name="progressAdd">Valut to add to task progress.</param>
        IEtaCalculator Advance(double progressAdd);
        IEtaCalculator Complete();

        IEnumerable<T> TrackProgress<T>(IEnumerable<T> ien)
        {
            SetTotal(ien.LongCount()).SetDone(0).Start();
            foreach (var item in ien)
            {
                yield return item;
                SetDone(Done + 1);
            }
        }

        /// <summary>
        /// Event raised each time a count progress is reached
        /// </summary>
        event EventHandler<EtaCountEventArgs> CountProgress;
        /// <summary>
        /// Event raised each time a time progress is reached
        /// </summary>
        event EventHandler<EtaTimeEventArgs> TimeProgress;
        /// <summary>
        /// Event raised each time a count progress or a time progress is reached
        /// </summary>
        event EventHandler<EtaEventArgs> Progress;
    }
}