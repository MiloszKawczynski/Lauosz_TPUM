namespace SerwerLogika
{
    internal class DiscountNotifier : IObservable<float>, IDiscountNotifier
    {
        private readonly List<IObserver<float>> _observers = new();
        private readonly Timer _timer;

        public DiscountNotifier()
        {
            _timer = new Timer(CheckDiscount, null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
        }

        private void CheckDiscount(object? state)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                NotifyDiscount(0.2f);
            }
        }

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