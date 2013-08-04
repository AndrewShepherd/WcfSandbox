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
    public class ChatroomClientViewModel : ViewModel<ChatroomClientViewModel>
    {

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
                    _chatroomClientImpl = new ChatroomClientImpl();
                    _chatroomClientImpl.SomebodyLoggedIn += SomebodyLoggedIn;
                    _chatroomClientImpl.SomebodyLoggedOut += SomebodyLoggedOut;
                    _channelFactory = new DuplexChannelFactory<IChatRoom>(_chatroomClientImpl, "chatRoomEndpoint");
                    _channelFactory.Open();
                    _chatRoom = _channelFactory.CreateChannel();
                    _chatRoom.LogIn(this._avatarName);
                    Notify(_ => _.CanLogIn);
                    Notify(_ => _.CanLogOut);
                };
            Task.Run(action);
        }


        private readonly ObservableCollection<string> _events = new ObservableCollection<string>();

        public ObservableCollection<string> Events
        {
            get
            {
                return _events;
            }
        }

        void SomebodyLoggedIn(object sender, LoggedInEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                _events.Add(string.Format("{0} logged in", e.AvatarName));
            }));
        }

        void SomebodyLoggedOut(object sender, LoggedOutEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                _events.Add(string.Format("{0} logged out", e.AvatarName));
            }));
        }

        public void LogOut()
        {
            Task.Run(() =>
                {
                    _chatroomClientImpl.SomebodyLoggedIn -= SomebodyLoggedIn;
                    _chatroomClientImpl.SomebodyLoggedOut -= SomebodyLoggedOut;
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
