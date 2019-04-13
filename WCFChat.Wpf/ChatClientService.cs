using System;
using System.Collections.Generic;
using System.Linq;
using WCFChat.Interfaces;

namespace WCFChat.Wpf
{
    public class ChatClientService: IChatClient
    {
        #region Public Properties

        public Action<string, string> AppendMessage;
        public Action<List<string>> RefreshUsers;

        #endregion
 
        #region Public Methods

        public void ReceiveMessage(string user, string message)
        {
            AppendMessage(user, message);
        }

        public void GetUsersList(UsersList usersList)
        {
            RefreshUsers(usersList.List.ToList());
        }

        #endregion
    }
}