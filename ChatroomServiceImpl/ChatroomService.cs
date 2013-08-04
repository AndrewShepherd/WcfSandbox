using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ChatroomServiceInterfaces;

namespace ChatroomServiceImpl
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConfigurationName="ChatroomServiceImpl", ConcurrencyMode=ConcurrencyMode.Multiple)]
    public class ChatroomService : IChatRoom
    {
        class AvatarAndChannel
        {
            public string AvatarName { get; set; }
            public IChatRoomCallback Channel { get; set; }
        }

       
        readonly List<AvatarAndChannel> _clients = new List<AvatarAndChannel>();


        Task Broadcast(Action<IChatRoomCallback> action)
        {
            Task[] tasks;
            lock (_clients)
            {
                tasks = _clients.Select(avc => avc.Channel)
                                    .Select(c => new Action(() => action(c)))
                                    .Select(a => Task.Run(a))
                                    .ToArray();
            }
           return Task.WhenAll(tasks);
        }

        void IChatRoom.LogIn(string logInName)
        {
            var operationContext = OperationContext.Current;
            var requestContext = operationContext.RequestContext;
            IContextChannel contextChannel = operationContext.Channel;

            var callbackChannel = operationContext.GetCallbackChannel<IChatRoomCallback>();
            lock (_clients)
            {
                _clients.Add(new AvatarAndChannel
                {
                    AvatarName = logInName,
                    Channel = callbackChannel
                }
                );
            }
            Broadcast(c => c.SomebodyLoggedIn(logInName));
           
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
            string loggedOutAvatar = null;
            lock (_clients)
            {
                var client = _clients.FirstOrDefault(n => n.Channel == callbackChannel);
                if (client != null)
                {
                    _clients.Remove(client);
                    loggedOutAvatar = client.AvatarName;


                    var clientCallback = client.Channel as ICommunicationObject;
                    if (clientCallback != null)
                    {
                        //clientCallback.Close();
                    }
                }
            }

            Broadcast(c => c.SomebodyLoggedOut(loggedOutAvatar));

        }
    }
}
