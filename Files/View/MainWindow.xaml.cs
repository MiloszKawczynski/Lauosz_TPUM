using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ViewModel;

namespace View
{
    public partial class MainWindow : Window
    {
        private class DiscountObserver : IObserver<float>
        {
            private readonly Dispatcher _dispatcher;

            public DiscountObserver(Dispatcher dispatcher)
            {
                _dispatcher = dispatcher;
            }

            public void OnNext(float discount)
            {
                _dispatcher.Invoke(() =>
                    MessageBox.Show($"Promocja {discount * 100}%!"));
            }

            public void OnError(Exception ex)
            {
                _dispatcher.Invoke(() =>
                    MessageBox.Show($"Błąd: {ex.Message}"));
            }

            public void OnCompleted()
            {
                // Optional: Add completion logic if needed
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            var vm = AbstractViewModelAPI.CreateAPI();
            DataContext = vm;
            _ = vm.InitializeConnectionAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    vm.SubscribeToDiscounts(new DiscountObserver(Dispatcher));
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                        MessageBox.Show($"Błąd połączenia: {ex.Message}"));
                }
            });
        }
    }
}