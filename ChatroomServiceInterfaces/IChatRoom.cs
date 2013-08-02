using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomServiceInterfaces
{
    [ServiceContract(ConfigurationName="IChatRoom", CallbackContract=typeof(IChatRoomCallback))]
    public interface IChatRoom
    {
        [OperationContract]
        void LogIn(string logInName);

        void Say(string text);

        [OperationContract]
        void LogOut();
    }
}
