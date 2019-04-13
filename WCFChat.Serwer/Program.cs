using System;
using System.ServiceModel;
using WCFChat.Interfaces;

namespace WCFChat.Serwer
{
    public class Program
    {
        #region Public Methods

        public static void Main(string[] args)
        {
            var adress = new Uri("http://localhost:2222/");
            var host = new ServiceHost(typeof(ChatServerService), adress);
            host.AddServiceEndpoint(typeof(IChatServer), new WSDualHttpBinding(), adress);
            host.Open();

            Console.WriteLine($"Serwer uruchomiony @ {DateTime.Now}");
            Console.ReadKey();
        }

        #endregion
    }
}