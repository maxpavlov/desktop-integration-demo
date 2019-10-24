using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using companion.Utils;

namespace companion.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand CloseCommand { get; set; }

        public DelegateCommand MinimizeCommand { get; }

        public DelegateCommand DragMoveCommand { get; }

        public String Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public BaseViewModel()
        {
            CloseCommand = new DelegateCommand(CloseWindow, null);
            MinimizeCommand = new DelegateCommand(MinimizeWindow, null);
            DragMoveCommand = new DelegateCommand(DragMove, null);
        }

        public void CloseWindow(object parameter)
        {
            if (parameter is Window mainWindow)
                mainWindow.Close();
        }

        private void MinimizeWindow(object parameter)
        {
            if (parameter is Window mainWindow)
                mainWindow.WindowState = WindowState.Minimized;
        }

        private void DragMove(object parameter)
        {
            if (parameter is Window mainWindow)
                mainWindow.DragMove();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
