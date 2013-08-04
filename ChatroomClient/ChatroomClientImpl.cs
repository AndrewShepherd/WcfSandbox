using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ChatroomServiceInterfaces;


namespace ChatroomClient
{
    class LoggedInEventArgs : EventArgs
    {
        public string AvatarName { get; set; }
    }

    class LoggedOutEventArgs : EventArgs
    {
        public string AvatarName { get; set; }
    }




    class ChatroomClientImpl : IChatRoomCallback
    {

        public event EventHandler<LoggedInEventArgs> SomebodyLoggedIn;
        public event EventHandler<LoggedOutEventArgs> SomebodyLoggedOut;

        void IChatRoomCallback.SomebodySaid(string name, string text)
        {

        }

        void IChatRoomCallback.SomebodyLoggedIn(string name)
        {
            var dg = this.SomebodyLoggedIn;
            if (dg != null)
            {
                dg(this, new LoggedInEventArgs { AvatarName = name });
            }
        }

        void IChatRoomCallback.SomebodyLoggedOut(string name)
        {
            var dg = this.SomebodyLoggedOut;
            if (dg != null)
            {
                dg(this, new LoggedOutEventArgs { AvatarName = name });
            }
        }
    }
}
