using System;
using System.Windows.Input;

namespace ImageProcessor.UI.Commands
{
    public class Command : ICommand
    {
        private Action m_Action;

        public Command(Action action)
        {
            m_Action = action;
        }


        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            m_Action();
        }
    }
}
