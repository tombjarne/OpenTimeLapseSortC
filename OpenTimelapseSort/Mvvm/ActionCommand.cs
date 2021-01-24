using System;
using System.Windows.Input;

namespace OpenTimelapseSort.Mvvm
{
    public class ActionCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public ActionCommand(Action<object> execute)
            : this(execute, null)
        { }

        public ActionCommand(Action<object> execute,
            Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        // TODO: rework

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value; 
            remove => CommandManager.RequerySuggested -= value; 
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}