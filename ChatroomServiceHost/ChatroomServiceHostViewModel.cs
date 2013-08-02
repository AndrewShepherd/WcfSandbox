using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using System.Collections.Concurrent;
using System.Reflection;
using System.Windows.Threading;
using System.Windows.Input;
using System.ServiceModel;
using WpfUtils;
using ChatroomServiceImpl;

namespace ChatroomServiceHost
{
    class ChatroomServiceHostViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private static string ExtractPropertyName(LambdaExpression l)
        {
            MemberExpression memberExpression = (MemberExpression)l.Body;
            return memberExpression.Member.Name;
        }

        readonly ConcurrentBag<string> _propertiesToNotifyOn = new ConcurrentBag<string>();
        readonly Dispatcher _mainDispatcher = Dispatcher.CurrentDispatcher;
        private void Notify<T>(Expression<Func<ChatroomServiceHostViewModel, T>> propName)
        {
            _propertiesToNotifyOn.Add(ExtractPropertyName(propName));
            _mainDispatcher.BeginInvoke(new Action(() =>
            {
                var dg = this.PropertyChanged;
                string prop;
                while(_propertiesToNotifyOn.TryTake(out prop))
                {
                    dg(this, new PropertyChangedEventArgs(prop));
                }
            }));
        }

        ServiceHost _serviceHost;

        public void OpenHost()
        {
            _serviceHost = new ServiceHost(typeof(ChatroomService));
            _serviceHost.Open();
            Notify(_ => _.CanOpenHost);
            Notify(_ => _.CanCloseHost);

        }

        public void CloseHost()
        {
            _serviceHost.Close();
            _serviceHost = null;
            Notify(_ => _.CanOpenHost);
            Notify(_ => _.CanCloseHost);
        }

        public bool CanOpenHost
        {
            get
            {
                return (_serviceHost == null);
            }
        }

        public bool CanCloseHost
        {
            get
            {
                return (_serviceHost != null);
            }
        }

        public ICommand OpenHostCommand
        {
            get
            {
                return new DelegateCommand(() => this.OpenHost(), () => this.CanOpenHost);
            }
        }

        public ICommand CloseHostCommand
        {
            get
            {
                return new DelegateCommand(() => this.CloseHost(), () => this.CanCloseHost);
            }
        }
    }
}
