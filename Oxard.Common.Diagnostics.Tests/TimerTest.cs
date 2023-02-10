using NUnit.Framework;
using Timer = Oxard.Common.Diagnostics.Timer;

namespace Mobile.Common.Diagnostics.Tests
{
    [TestFixture]
    public class TimerTest
    {
        [Test]
        public void TimerStartAndStopNormally()
        {
            Timer timer = new Timer(TimeSpan.FromMilliseconds(10), () => { });
            timer.Start();
            timer.Stop();
        }
    }
}
