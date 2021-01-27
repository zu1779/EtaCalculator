namespace Zu1779.EtaCalculator
{
    using System;
    using System.Diagnostics;

    //TODO: decide to change StopWatch with a injectable interface
    //TODO: unit testing project
    //TODO: using something like unitsnet for showing measures?
    //TODO: integration with quartz?
    //TODO: integrate with iqueryable for automatic foreach data acquisition
    //TODO: integrate classic for cycle for data acquisition?
    //TODO: integrate a conditional dump of the state based on count or time gap

    public class EtaCalculator
    {
        public EtaCalculator() => stopwatch = new Stopwatch();
        private readonly Stopwatch stopwatch;

        public DateTimeOffset? UtcStart { get; private set; }
        public DateTimeOffset? UtcStop => (!UtcStart.HasValue || !TotalTime.HasValue) ? null : UtcStart.Value.Add(TotalTime.Value);

        public decimal Total { get; set; }
        public decimal Done { get; set; }
        public decimal ToDo
        {
            get => Total - Done;
            set => Done = Total - ToDo;
        }

        public decimal DoneProportion
        {
            get => Done / Total;
            set => Done = value * Total;
        }
        public decimal ToDoProportion => 1 - DoneProportion;

        public TimeSpan? TotalTime => 
            Done == 0 ? null : TimeSpan.FromMilliseconds(stopwatch.Elapsed.TotalMilliseconds / (double)Done * (double)Total);
        public TimeSpan DoneTime => stopwatch.Elapsed;
        public TimeSpan? ToDoTime => TotalTime?.Subtract(DoneTime);

        public double ItemPerSecond => (double)Done / DoneTime.TotalSeconds;

        /// <summary>
        /// Gets a value indicating whether the System.Diagnostics.Stopwatch timer is running.
        /// </summary>
        public bool IsRunning => stopwatch.IsRunning;

        /// <summary>
        /// Gets the total elapsed time measured by the current instance.
        /// </summary>
        public TimeSpan Elapsed => stopwatch.Elapsed;
        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in milliseconds.
        /// </summary>
        public long ElapsedMilliseconds => stopwatch.ElapsedMilliseconds;
        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in timer ticks.
        /// </summary>
        public long ElapsedTicks { get { return stopwatch.ElapsedTicks; } }

        #region Management
        /// <summary>
        /// Starts, or resumes, measuring elapsed time for an interval.
        /// </summary>
        public void Start()
        {
            UtcStart = DateTimeOffset.UtcNow;
            stopwatch.Start();
        }
        /// <summary>
        ///  Stops time interval measurement, resets the elapsed time to zero, and starts measuring elapsed time.
        /// </summary>
        public void Restart() => stopwatch.Restart();
        /// <summary>
        /// Stops measuring elapsed time for an interval.
        /// </summary>
        public void Stop() => stopwatch.Stop();
        public void Complete() => Done = Total;
        /// <summary>
        /// Stops time interval measurement and resets the elapsed time to zero.
        /// </summary>
        public void Reset() => stopwatch.Reset();
        #endregion

        public override string ToString()
        {
            return $"Tot: {Total:n0} [{TotalTime.ToUMStr()}] - Done: {Done:n0} ({DoneProportion:p}) [{DoneTime.ToUMStr()}] - ToDo: {ToDo:n0} ({ToDoProportion:p}) [{ToDoTime.ToUMStr()}] => {ItemPerSecond:n} item/s., From {UtcStart:d} {UtcStart:t} To {UtcStop:d} {UtcStop:t}";
        }
        public string ToString(decimal done)
        {
            Done = done;
            return ToString();
        }

        #region Static methods
        /// <summary>
        /// Gets the frequency of the timer as the number of ticks per second. This field is read-only.
        /// </summary>
        public static long Frequency => Stopwatch.Frequency;

        /// <summary>
        /// Indicates whether the timer is based on a high-resolution performance counter. This field is read-only.
        /// </summary>
        public static bool IsHighResolution => Stopwatch.IsHighResolution;

        /// <summary>
        /// Gets the current number of ticks in the timer mechanism.
        /// </summary>
        /// <returns>A long integer representing the tick counter value of the underlying timer mechanism.</returns>
        public static long GetTimestamp() => Stopwatch.GetTimestamp();

        /// <summary>
        /// Initializes a new System.Diagnostics.Stopwatch instance, sets the elapsed time property to zero, and starts measuring elapsed time.
        /// </summary>
        /// <returns>A System.Diagnostics.Stopwatch that has just begun measuring elapsed time.</returns>
        public static EtaCalculator StartNew(decimal total = 100, decimal done = 0)
        {
            var ew = new EtaCalculator
            {
                Total = total,
                Done = done,
            };
            ew.Start();
            return ew;
        }
        #endregion
    }
}
