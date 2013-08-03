using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ChatroomServiceInterfaces;


namespace ChatroomClient
{
    class ChatroomClientImpl : IChatRoomCallback
    {
        void IChatRoomCallback.SomebodySaid(string name, string text)
        {
            throw new NotImplementedException();
        }
    }
}
