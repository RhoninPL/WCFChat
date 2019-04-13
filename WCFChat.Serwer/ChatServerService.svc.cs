using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using WCFChat.Interfaces;

namespace WCFChat.Serwer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class ChatServerService : IChatServer
    {
        #region Private Fields

        private readonly Dictionary<IChatClient, string> _users = new Dictionary<IChatClient, string>();

        #endregion

        #region Public Methods

        public bool Join(string username)
        {
            if (!_users.FirstOrDefault(user => user.Value == username).Equals(new KeyValuePair<IChatClient, string>()))
                return false;

            var connecton = OperationContext.Current.GetCallbackChannel<IChatClient>();
            _users[connecton] = username;

            new Task(RefreshUsers).Start();

            return true;
        }

        public void Leave()
        {
            var connection = OperationContext.Current.GetCallbackChannel<IChatClient>();
            if (!_users.TryGetValue(connection, out var userName))
                return;

            var userEntity = _users.FirstOrDefault(user => user.Value == userName).Key;
            _users.Remove(userEntity);

            new Task(RefreshUsers).Start();
        }

        public void SendMessage(string message)
        {
            var connection = OperationContext.Current.GetCallbackChannel<IChatClient>();
            if (!_users.TryGetValue(connection, out var user))
                return;

            new Task(() => SendMessageToOtherUsers(message, connection, user)).Start();
        }
        
        #endregion

        #region Private Methods 

        private void RefreshUsers()
        {
            var usersList = new UsersList { List = _users.Values.ToList() };
            var removeChatClients = new List<IChatClient>();
            foreach (var usersKey in _users.Keys)
            {
                try
                {
                    usersKey.GetUsersList(usersList);
                }
                catch
                {
                    removeChatClients.Add(usersKey);
                }
            }

            foreach (var chatClient in removeChatClients)
            {
                _users.Remove(chatClient);
            }
        }

        private void SendMessageToOtherUsers(string message, IChatClient connection, string user)
        {
            var removeChatClients = new List<IChatClient>();

            foreach (var usersKey in _users.Keys)
            {
                if (usersKey == connection)
                    continue;
                try
                {
                    usersKey.ReceiveMessage(user, message);
                }
                catch
                {
                    removeChatClients.Add(usersKey);
                }
            }

            foreach (var chatClient in removeChatClients)
            {
                _users.Remove(chatClient);
            }
        }

        #endregion
    }
}
