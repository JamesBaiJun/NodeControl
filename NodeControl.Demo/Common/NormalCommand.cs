using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NodeControl.Demo.Common
{
    public class NormalCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteMethod?.Invoke((T)parameter);
        }

        public Action<T> ExecuteMethod { get; set; }
    }

    public class NormalCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteMethod?.Invoke();
        }

        public Action ExecuteMethod { get; set; }
    }
}
