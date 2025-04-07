using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerwerLogika;
using System;

namespace Testy
{
    [TestClass]
    public class DiscountNotificationTest
    {
        [TestMethod]
        public void ShouldNotifyAboutDiscount()
        {
            var notifier = new DiscountNotifier();
            var mockObserver = new MockObserver();

            using (notifier.Subscribe(mockObserver))
            {
                notifier.NotifyDiscount(0.2f);
                Assert.AreEqual(0.2f, mockObserver.LastDiscount);
            }
        }

        private class MockObserver : IObserver<float>
        {
            public float LastDiscount { get; private set; }

            public void OnNext(float value) => LastDiscount = value;
            public void OnError(Exception error) { }
            public void OnCompleted() { }
        }
    }
}