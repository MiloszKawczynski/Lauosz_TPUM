using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ViewModel
{
    public class ViewModelAPI : INotifyPropertyChanged
    {
        private AbstractModelAPI _modelAPI;
        private int ballsAmount = 1;
        private int ballR = 30;
        private bool isRunning = false;
        public ObservableCollection<IModelBall> ModelBalls => _modelAPI.GetModelBalls();


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public String Balls
        {
            get { return Convert.ToString(ballsAmount); }
            set
            {
                try
                {
                    ballsAmount = Convert.ToInt32(value);
                    OnPropertyChanged();
                }
                catch (System.FormatException)
                {
                    ballsAmount = 0;
                }
            }
        }


        public ViewModelAPI()
        {
            _modelAPI = AbstractModelAPI.CreateAPI();
            EnableAction = new ActionCommand(TurnOn);
            DisableAction = new ActionCommand(TurnOff);

        }

        private void TurnOn()
        {

            if (isRunning)
            {
                _modelAPI.TurnOff();
            }
            _modelAPI.TurnOn(500, 666, ballsAmount, ballR);
            isRunning = true;
            OnPropertyChanged("ModelBalls");
        }

        private void TurnOff()
        {
            
            _modelAPI.TurnOff();
            isRunning = false;
            OnPropertyChanged("ModelBalls");
        }

        public ICommand EnableAction { get; set; }
        public ICommand DisableAction { get; set; }
    }
}