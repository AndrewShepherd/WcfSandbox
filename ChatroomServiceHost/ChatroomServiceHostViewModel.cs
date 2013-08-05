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
    class ChatroomServiceHostViewModel : ViewModel<ChatroomServiceHostViewModel>
    {

        ServiceHost _serviceHost;
        ChatroomService _serviceInstance;



        private object _mainDisplayContent = null;
        public object MainDisplayContent
        {
            get
            {
                return _mainDisplayContent;
            }
            set
            {
                if (_mainDisplayContent != value)
                {
                    _mainDisplayContent = value;
                    Notify(_ => _.MainDisplayContent);
                }
            }
        }


        public void OpenHost()
        {
            Task.Run(() =>
                {
                    try
                    {
                        _serviceInstance = new ChatroomService();
                        _serviceHost = new ServiceHost(_serviceInstance);
                        _serviceHost.Open();
                        Notify(_ => _.CanOpenHost);
                        Notify(_ => _.CanCloseHost);
                    }
                    catch (Exception ex)
                    {
                        this.MainDisplayContent = ex;
                        _serviceHost = null;
                    }
                }
            );

        }

        public void CloseHost()
        {
            _serviceHost.Close();
            _serviceHost = null;
            _serviceInstance = null;
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
