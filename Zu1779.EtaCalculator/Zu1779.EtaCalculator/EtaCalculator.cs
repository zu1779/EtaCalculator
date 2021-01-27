namespace Zu1779.EtaCalculator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Timers;
    using Humanizer;

    //TODO: unit testing project
    //TODO: using something like unitsnet for showing measures?
    //TODO: integration with quartz?
    //TODO: integrate with iqueryable for automatic foreach data acquisition
    //TODO: integrate classic for cycle for data acquisition?
    //TODO: integrate a conditional dump of the state based on count or time gap

    //TODO: implement an option to never approximate to (done 100% or todo 0%) if not really finished

    public class EtaCalculator : IEtaCalculator
    {
        public EtaCalculator(IStopwatch stopwatch = null)
        {
            this.stopwatch = stopwatch ?? new StopwatchWrapper();
            thresholdTimer.Elapsed += (s, e) => Progress?.Invoke(this, new EtaTimeEventArgs { Done = Done, Total = Total });
        }

        private readonly IStopwatch stopwatch;
        private readonly Timer thresholdTimer = new Timer { AutoReset = true, Enabled = false, };
        private void StartThresholdTimer()
        {
            if (!thresholdTimer.Enabled && TimeThreshold.HasValue)
            {
                thresholdTimer.Interval = TimeThreshold.Value.TotalMilliseconds;
                thresholdTimer.Start();
            }
        }
        private void StopThresholdTimer()
        {
            if (thresholdTimer.Enabled) thresholdTimer.Stop();
        }

        public event EventHandler<EtaEventArgs> Progress;

        public DateTimeOffset? StartTime { get; private set; }
        public DateTimeOffset? StopTime
        {
            get
            {
                if (!StartTime.HasValue || !TotalTime.HasValue) return null;
                else return StartTime.Value.Add(TotalTime.Value);
            }
        }

        public double? Total { get; set; }
        private double done;
        public double Done
        {
            get => done;
            set
            {
                done = value;
                if (done == Total)
                {
                    stopwatch.Stop();
                    StopThresholdTimer();
                }
                // Consider threshold as percentage
                if (CountThreshold >= 0 && CountThreshold < 1)
                {
                    // Check if _done pass thresold
                }
                else if (CountThreshold >= 1)
                {
                    if (done % CountThreshold == 0) Progress?.Invoke(this, new EtaCountEventArgs { Done = done, Total = Total });
                }
            }
        }
        public double? DoneProportion
        {
            get => Done / Total;
            set => Done = (value * Total) ?? 0;
        }
        public double? ToDo
        {
            get => Total - Done;
            set => Done = (Total - ToDo) ?? 0;
        }
        public double? ToDoProportion
        {
            get => 1 - DoneProportion;
            set => DoneProportion = 1 - value;
        }

        public double? CountThreshold { get; set; }
        public TimeSpan? TimeThreshold { get; set; }

        public TimeSpan? TotalTime
        {
            get
            {
                if (!Total.HasValue || Done == 0) return null;
                else return TimeSpan.FromMilliseconds(stopwatch.Elapsed.TotalMilliseconds / Done * Total.Value);
            }
        }
        public TimeSpan DoneTime => stopwatch.Elapsed;
        public TimeSpan? ToDoTime => TotalTime?.Subtract(DoneTime);

        public double? ItemPerSecond => Done > 0 && DoneTime > TimeSpan.Zero ? Done / DoneTime.TotalSeconds : null;

        #region Management
        public IEtaCalculator Start()
        {
            StartTime = DateTimeOffset.Now;
            stopwatch.Start();
            StartThresholdTimer();
            return this;
        }
        public IEtaCalculator Advance(double done)
        {
            Done = done;
            return this;
        }
        public IEtaCalculator Complete()
        {
            Done = Total ?? double.MaxValue;
            return this;
        }
        #endregion

        public override string ToString()
        {
            return $"Tot: {Total:n0} [{TotalTime?.Humanize()}] - " +
                $"Done: {Done:n0} ({DoneProportion:p0}) [{DoneTime.Humanize()}] - " +
                $"ToDo: {ToDo:n0} ({ToDoProportion:p0}) [{ToDoTime?.Humanize()}] => " +
                $"{ItemPerSecond:n} item/s, From {StartTime:d} {StartTime:t} To {StopTime:d} {StopTime:t}";
        }
    }
}
