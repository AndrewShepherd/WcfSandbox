using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfUtils
{
    public class ViewModel<T> :  INotifyPropertyChanged where T : ViewModel<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private static string ExtractPropertyName(LambdaExpression l)
        {
            MemberExpression memberExpression = (MemberExpression)l.Body;
            return memberExpression.Member.Name;
        }

        readonly ConcurrentBag<string> _propertiesToNotifyOn = new ConcurrentBag<string>();
        readonly Dispatcher _mainDispatcher = Dispatcher.CurrentDispatcher;

        protected void Notify<U>(Expression<Func<T, U>> propName)
        {
            _propertiesToNotifyOn.Add(ExtractPropertyName(propName));
            _mainDispatcher.BeginInvoke(new Action(() =>
            {
                var dg = this.PropertyChanged;
                string prop;
                while (_propertiesToNotifyOn.TryTake(out prop))
                {
                    dg(this, new PropertyChangedEventArgs(prop));
                }
            }));
        }



    }
}
