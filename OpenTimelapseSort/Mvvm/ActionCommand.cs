using System;
using System.Windows.Input;

namespace OpenTimelapseSort.Mvvm
{
    public class ActionCommand : ICommand
    {
        private readonly Predicate<object> _canBeExecuted;
        private readonly Action<object> _runExecute;

        public ActionCommand(Action<object> runExecute)
            : this(runExecute, null)
        {
        }

        public ActionCommand(Action<object> runExecute,
            Predicate<object> canBeExecuted)
        {
            _runExecute = runExecute;
            _canBeExecuted = canBeExecuted;
        }

        public bool CanExecute(object parameter)
        {
            return _canBeExecuted == null || _canBeExecuted(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _runExecute(parameter);
        }
    }
}