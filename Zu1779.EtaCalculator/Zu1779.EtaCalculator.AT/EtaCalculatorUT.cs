namespace Zu1779.EtaCalculator.AT
{
    using System;

    using Moq;
    using Xunit;

    using Zu1779.EtaCalculator;

    public class EtaCalculatorUT
    {
        [Theory, InlineData(2), InlineData(4), InlineData(10), InlineData(17), InlineData(432)]
        public void CheckTodoValues(int total)
        {
            var moqStopwatch = new Mock<IStopwatch>(MockBehavior.Strict);
            moqStopwatch.Setup(c => c.Start());
            moqStopwatch.Setup(c => c.Stop());

            IEtaCalculator calculator = new EtaCalculator(moqStopwatch.Object);
            calculator.SetTotal(total);
            calculator.Start();
            for (int cycle = 0; cycle < total; cycle++)
            {
                calculator.Advance(cycle + 1);
                Assert.Equal(cycle + 1, calculator.Done);
                Assert.Equal((cycle + 1) / (double)total, calculator.DoneProportion.Value, 3);
            }
            calculator.Complete();

            Assert.Equal(total, calculator.Done);
            Assert.Equal(1, calculator.DoneProportion);
            Assert.Equal(0, calculator.ToDo);
            Assert.Equal(0, calculator.ToDoProportion);
            moqStopwatch.Verify(c => c.Start(), Times.Once());
            moqStopwatch.Verify(c => c.Stop(), Times.AtLeastOnce());
        }
    }
}
