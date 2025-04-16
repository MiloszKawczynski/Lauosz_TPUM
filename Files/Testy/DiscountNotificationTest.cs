using SerwerLogika;

namespace Testy
{
    [TestClass]
    public class DiscountNotificationTest
    {
        [TestMethod]
        public void ShouldNotifyAboutDiscount()
        {
            var notifier = new TestDicsountNotifier();
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

        private class TestDicsountNotifier : IDiscountNotifier
        {
            private readonly List<IObserver<float>> _observers = new();
            public void NotifyDiscount(float discount)
            {
                foreach (var observer in _observers)
                {
                    observer.OnNext(discount);
                }
            }

            public IDisposable Subscribe(IObserver<float> observer)
            {
                _observers.Add(observer);
                return new Unsubscriber(_observers, observer);
            }

            private class Unsubscriber : IDisposable
            {
                private readonly List<IObserver<float>> _observers;
                private readonly IObserver<float> _observer;
                private bool _disposed;

                public Unsubscriber(List<IObserver<float>> observers, IObserver<float> observer)
                {
                    _observers = observers;
                    _observer = observer;
                }

                public void Dispose()
                {
                    if (_disposed) return;

                    if (_observer != null && _observers.Contains(_observer))
                    {
                        _observers.Remove(_observer);
                    }
                    _disposed = true;
                }
            }
        }
    }
}