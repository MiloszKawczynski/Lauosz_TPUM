using System.Windows.Input;

namespace ViewModel
{
    public class ActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action execute;                //Action to be executed
        private readonly Func<bool> canExecute;         //Checking if we can execute the action

        public ActionCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) =>
            canExecute == null || canExecute();
        public void Execute(object parameter) => execute();

        internal void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }

}