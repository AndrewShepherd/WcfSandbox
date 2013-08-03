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
        class AvatarAndChannel
        {
            public string AvatarName { get; set; }
            public IChatRoomCallback Channel { get; set; }
        }

        readonly List<AvatarAndChannel> _clients = new List<AvatarAndChannel>();

        void IChatRoom.LogIn(string logInName)
        {
            var operationContext = OperationContext.Current;
            var requestContext = operationContext.RequestContext;
            IContextChannel contextChannel = operationContext.Channel;

            var callbackChannel = operationContext.GetCallbackChannel<IChatRoomCallback>();
            _clients.Add(new AvatarAndChannel
            {
                AvatarName = logInName,
                Channel = callbackChannel
            }
            );
        }

        void IChatRoom.Say(string text)
        {
            throw new NotImplementedException();
        }

        void IChatRoom.LogOut()
        {
            var operationContext = OperationContext.Current;
            var requestContext = operationContext.RequestContext;
            IContextChannel contextChannel = operationContext.Channel;
            var callbackChannel = operationContext.GetCallbackChannel<IChatRoomCallback>();
            var client = _clients.FirstOrDefault(n => n.Channel == callbackChannel);
            if (client != null)
            {
                _clients.Remove(client);
            }
        }
    }
}
