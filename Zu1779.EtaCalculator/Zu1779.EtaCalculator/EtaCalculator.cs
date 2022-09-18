namespace Zu1779.EtaCalculator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Timers;

    using Zu1779.GenUtil;

    //TODO: integrate classic for cycle for data acquisition?
    //TODO: integrate a conditional dump of the state based on count or time gap

    //TODO: implement an option to never approximate to (done 100% or todo 0%) if not really finished
    //TODO: implement the possibility to choose the data span from which calculate statistics (all, last minute, last timespan)

    //TODO: check optional nuget to use as plugin
    //TODO: refactoring with EtaCalculator state (stopped, ...)
    //TODO: decide what to do in a unknown total state, and refactoring accordingly

    public class EtaCalculator : IEtaCalculator
    {
        public EtaCalculator(IStopwatch stopwatch = null)
        {
            this.stopwatch = stopwatch ?? new StopwatchWrapper();
            thresholdTimer.Elapsed += (s, e) => OnTimeProgress(Done, Total.Value);
        }

        private readonly IStopwatch stopwatch;
        private readonly Timer thresholdTimer = new Timer { AutoReset = true, Enabled = false, };
        //private double? lastCountProgress;
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

        public event EventHandler<EtaCountEventArgs> CountProgress;
        public event EventHandler<EtaTimeEventArgs> TimeProgress;
        public event EventHandler<EtaEventArgs> Progress;
        private void OnCountProgress(double done, double? total)
        {
            var e = new EtaCountEventArgs { Done = done, Total = total };
            CountProgress?.Invoke(this, e);
            OnProgress(e);
        }
        private void OnTimeProgress(double done, double? total)
        {
            var e = new EtaTimeEventArgs { Done = done, Total = total };
            TimeProgress?.Invoke(this, e);
            OnProgress(e);
        }
        private void OnProgress(EtaEventArgs e) => Progress?.Invoke(this, e);

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
                    //TODO: Check if _done pass thresold
                }
                else if (CountThreshold >= 1)
                {
                    //TODO: implement a raise mode to permit advance of values different of 1
                    if (done % CountThreshold == 0) OnCountProgress(Done, Total);
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
        public IEtaCalculator Advance(double progressAdd)
        {
            Done += progressAdd;
            if (Total.HasValue) Done = Done.MaxCap(Total.Value);
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
            return $"Tot: {Total:n0} [{TotalTime}] - " +
                $"Done: {Done:n0} ({DoneProportion:p0}) [{DoneTime}] - " +
                $"ToDo: {ToDo:n0} ({ToDoProportion:p0}) [{ToDoTime}] => " +
                $"{ItemPerSecond:n} item/s, From {StartTime:d} {StartTime:t} To {StopTime:d} {StopTime:t}";
        }

        public IEtaCalculator SetTotal(double? total)
        {
            Total = total;
            return this;
        }
    }
}
