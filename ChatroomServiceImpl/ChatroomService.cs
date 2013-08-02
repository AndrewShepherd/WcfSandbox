using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ChatroomServiceInterfaces;

namespace ChatroomServiceImpl
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConfigurationName="ChatroomServiceImpl")]
    public class ChatroomService : IChatRoom
    {
        void IChatRoom.LogIn(string logInName)
        {
            throw new NotImplementedException();
        }

        void IChatRoom.Say(string text)
        {
            throw new NotImplementedException();
        }

        void IChatRoom.LogOut()
        {
            throw new NotImplementedException();
        }
    }
}
