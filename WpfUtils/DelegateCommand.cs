using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq.Expressions;
using System.ComponentModel;

namespace WpfUtils
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _command;
        private readonly Expression<Func<bool>> _canExecute;
        private readonly string _canExecutePropertyName;

        public DelegateCommand(Action command, Expression<Func<bool>> canExecute)
        {
            this._command = command;
            this._canExecute = canExecute;

            // TODO: Try and subscribe to canExecute
            var body = (MemberExpression)canExecute.Body;
            _canExecutePropertyName = body.Member.Name;
            var containingObject = ((ConstantExpression)body.Expression).Value;
            var notifyPropertyChanged = (INotifyPropertyChanged)containingObject;
            notifyPropertyChanged.PropertyChanged += notifyPropertyChanged_PropertyChanged;

            
        }

        void notifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _canExecutePropertyName)
            {
                if (_canExecuteChanged != null)
                {
                    _canExecuteChanged(this, EventArgs.Empty);
                }
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            Func<bool> f = _canExecute.Compile();
            return f();
        }

        private EventHandler _canExecuteChanged;
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
            }

            remove
            {
                _canExecuteChanged -= value;
            }
        }

        void ICommand.Execute(object parameter)
        {
            this._command();
        }
    }
} 
