using System;
using System.Collections.Generic;

namespace SerwerLogika
{
    public class DiscountNotifier : IObservable<float>
    {
        private readonly List<IObserver<float>> _observers = new();
        private readonly Timer _timer;

        public DiscountNotifier()
        {
            _timer = new Timer(CheckDiscount, null,
                TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private void CheckDiscount(object? state)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                foreach (var observer in _observers)
                {
                    observer.OnNext(0.2f);
                }
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

            public Unsubscriber(List<IObserver<float>> observers, IObserver<float> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}