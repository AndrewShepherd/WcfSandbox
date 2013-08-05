using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtils;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace ChatroomClient
{
    public class ChatroomContentViewModel : ViewModel<ChatroomContentViewModel>, IDisposable
    {
        private readonly ChatroomClientImpl _chatRoomClientImpl;

        public ChatroomContentViewModel(ChatroomClientImpl chatRoomClientImpl, Dispatcher uiThreadDispatcher)
            : base(uiThreadDispatcher)
        {
            _chatRoomClientImpl = chatRoomClientImpl;
            _chatRoomClientImpl.SomebodyLoggedIn += SomebodyLoggedIn;
            _chatRoomClientImpl.SomebodyLoggedOut += SomebodyLoggedOut;
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

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _chatRoomClientImpl.SomebodyLoggedIn -= SomebodyLoggedIn;
            _chatRoomClientImpl.SomebodyLoggedOut -= SomebodyLoggedOut;
        }

        #endregion
    }
}
