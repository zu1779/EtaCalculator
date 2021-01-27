namespace Zu1779.EtaCalculator
{
    using System;

    public class EtaEventArgs : EventArgs
    {
        public double Done { get; set; }
        public double? Total { get; set; }
    }

    public class EtaCountEventArgs : EtaEventArgs
    {
    }

    public class EtaTimeEventArgs : EtaEventArgs
    {
    }
}
