using System;
using System.Windows.Input;

namespace OpenTimelapseSort.Mvvm
{
    // FOREIGN CODE - doc-id:1000
    public class ActionCommand : ICommand
    {
        private readonly Predicate<object> _canBeExecuted;
        private readonly Action<object> _runExecute;

        // Constructor
        public ActionCommand(Action<object> runExecute)
            : this(runExecute, null)
        { }

        // Constructor - with params
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

        // event handler
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