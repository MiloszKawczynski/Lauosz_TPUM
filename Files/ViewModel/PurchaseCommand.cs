using Model;
using System.Windows.Input;

namespace ViewModel
{
    internal class PurchaseCommand : ICommand
    {
         public event EventHandler CanExecuteChanged;
         private readonly Action<IModelPlant> _execute;
         private readonly Func<IModelPlant, bool> _canExecute;

            
        public PurchaseCommand(Action<IModelPlant> execute, Func<IModelPlant, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((IModelPlant)parameter);
        }

        public void Execute(object parameter)
        {
           _execute((IModelPlant)parameter);
        }

        internal void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
