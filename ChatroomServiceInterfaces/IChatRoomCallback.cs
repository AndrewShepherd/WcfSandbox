using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatroomServiceInterfaces
{
    [ServiceContract]
    public interface IChatRoomCallback
    {
        [OperationContract(IsOneWay=true)]
        void SomebodyLoggedIn(string name);

        [OperationContract(IsOneWay=true)]
        void SomebodyLoggedOut(string name);


        [OperationContract(IsOneWay=true)]
        void SomebodySaid(string name, string text);
    }
}
