using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WCFChat.Interfaces
{
    [ServiceContract]
    public interface IChatClient
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(string user, string message);

        [OperationContract(IsOneWay = true)]
        void GetUsersList(UsersList usersList);
    }

    [DataContract]
    public class UsersList
    {
        [DataMember]
        public List<string> List { get; set; }
    }
}