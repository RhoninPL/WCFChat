using System.Collections.Generic;
using System.ServiceModel;

namespace WCFChat.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IChatClient))]
    public interface IChatServer
    {

        [OperationContract(IsOneWay = false)]
        bool Join(string username);

        [OperationContract(IsOneWay = true)]
        void SendMessage(string message);

        [OperationContract(IsOneWay = true)]
        void Leave();
    }
}