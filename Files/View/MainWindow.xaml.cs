using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using SharedModel;
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
            }

       
        }

        private class PlantObserver : IObserver<List<IPlant>>
        {
            private readonly Dispatcher _dispatcher;
            private readonly AbstractViewModelAPI vm;

            public PlantObserver(Dispatcher dispatcher, AbstractViewModelAPI vm)
            {
                _dispatcher = dispatcher;
                this.vm = vm;
            }

            public void OnNext(List<IPlant> plants)
            {
                _dispatcher.Invoke(() =>
                    vm.UpdatePlants(plants));
            }

            public void OnError(Exception ex)
            {
                _dispatcher.Invoke(() =>
                    MessageBox.Show($"Błąd: {ex.Message}"));
            }

            public void OnCompleted()
            {
            }


        }

        public MainWindow()
        {
            InitializeComponent();
            var vm = AbstractViewModelAPI.CreateAPI();
            DataContext = vm;
            _ = vm.InitializeConnectionAsync(new DiscountObserver(Dispatcher), new PlantObserver(Dispatcher, vm));

            
        }
    }
}