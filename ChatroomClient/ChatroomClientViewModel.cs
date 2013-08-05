using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtils;
using System.Windows.Input;
using System.ServiceModel;
using ChatroomServiceInterfaces;
using System.Collections.ObjectModel;

namespace ChatroomClient
{
    public sealed class ChatroomClientViewModel : ViewModel<ChatroomClientViewModel>
    {


        private object _mainContent;
        public object MainContent
        {
            get
            {
                return _mainContent;
            }
            set
            {
                if (_mainContent != value)
                {
                    var disposable = _mainContent as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                    _mainContent = value;
                    Notify(_ => _.MainContent);
                }
            }
        }


        public bool CanLogIn
        {
            get
            {
                return (
                            !(string.IsNullOrWhiteSpace(_avatarName))
                                &&
                            (_channelFactory == null)
                        );

            }
        }

        public bool CanLogOut
        {
            get
            {
                return _channelFactory != null;
            }
        }

        private string _avatarName;
        public string AvatarName
        {
            get
            {
                return _avatarName;
            }
            set
            {
                if (_avatarName != value)
                {
                    _avatarName = value;
                    Notify(_ => _.AvatarName);
                    Notify(_ => _.CanLogIn);
                }
            }
        }

        private DuplexChannelFactory<IChatRoom> _channelFactory = null;
        private ChatroomClientImpl _chatroomClientImpl;
        private IChatRoom _chatRoom = null;

        public void LogIn()
        {
            Action action = () =>
                {
                    try
                    {
                        _chatroomClientImpl = new ChatroomClientImpl();
                        _channelFactory = new DuplexChannelFactory<IChatRoom>(_chatroomClientImpl, "chatRoomEndpoint");
                        _channelFactory.Open();
                        _chatRoom = _channelFactory.CreateChannel();
                        _chatRoom.LogIn(this._avatarName);
                        Notify(_ => _.CanLogIn);
                        Notify(_ => _.CanLogOut);
                        this.MainContent = new ChatroomContentViewModel(_chatroomClientImpl, this.Dispatcher);
                    }
                    catch (Exception ex)
                    {
                        this.MainContent = ex;
                    }
                };
            Task.Run(action);
        }




        public void LogOut()
        {
            Task.Run(() =>
                {
                    try
                    {
                        _chatroomClientImpl = null;
                        _chatRoom.LogOut();
                        ICommunicationObject communicationObject = _chatRoom as ICommunicationObject;
                        if (communicationObject != null)
                        {
                            communicationObject.Close();
                        }

                        _channelFactory.Close();

                        _chatRoom = null;
                        _channelFactory = null;
                        Notify(_ => _.CanLogIn);
                        Notify(_ => _.CanLogOut);
                    }
                    catch (Exception ex)
                    {
                        this.MainContent = ex;
                    }
                });
        }


        public ICommand LogInCommand
        {
            get
            {
                return new DelegateCommand(LogIn, () => this.CanLogIn);
            }
        }

        public ICommand LogOutCommand
        {
            get
            {
                return new DelegateCommand(LogOut, () => this.CanLogOut);
            }
        }
    }
}
