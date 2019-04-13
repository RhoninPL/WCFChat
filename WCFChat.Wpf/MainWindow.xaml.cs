using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using WCFChat.Interfaces;

namespace WCFChat.Wpf
{
    public partial class MainWindow : Window
    {
        #region Private variables

        private ChannelFactory<IChatServer> _channel;
        private IChatServer _service;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Join(object sender, RoutedEventArgs e)
        {
            Join();
        }
        
        private void SendMessage(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void ReceiveMessage(string user, string message)
        {
            Messages.Text += $"{Environment.NewLine}{user}: {message}";
        }

        private void UserMessageInput_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            SendMessage();
        }

        private void UserNameInput_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            Join();
        }

        #endregion

        #region Private Methods

        private void ToggleElements()
        {
            UserMessageInput.IsEnabled = !UserMessageInput.IsEnabled;
            SendMessageButton.IsEnabled = !SendMessageButton.IsEnabled;
            UserNameInput.IsEnabled = !UserNameInput.IsEnabled;

            Title = SendMessageButton.IsEnabled ?
                $"Status: Connected as {UserNameInput.Text}" :
                $"Status: Disconnected";
            JoinButton.Content = SendMessageButton.IsEnabled ? "Rozłącz" : "Dołącz";
        }

        private void SendMessage()
        {
            try
            {
                Messages.Text += $"{Environment.NewLine}Ja: {UserMessageInput.Text}";
                _service.SendMessage(UserMessageInput.Text);
                UserMessageInput.Text = string.Empty;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Nie przesłano wiadomości, zostajesz wylogowany.", MessageBoxButton.OK, MessageBoxImage.Error);
                ToggleElements();
            }
        }

        private void CreateConnection()
        {
            try
            {
                if (_channel != null && _channel.State == CommunicationState.Opened)
                    return;

                var adres = new Uri("http://localhost:2222/");
                _channel = new DuplexChannelFactory<IChatServer>(new InstanceContext(new ChatClientService
                {
                    AppendMessage = ReceiveMessage,
                    RefreshUsers = RefreshUsers
                }), new WSDualHttpBinding(), new EndpointAddress(adres));
                _service = _channel.CreateChannel();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Błąd połączenia", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshUsers(List<string> list)
        {
            UsersList.Items.Clear();
            foreach (var user in list)
                UsersList.Items.Add(user);
        }

        private void Join()
        {
            try
            {
                if (JoinButton.Content.Equals("Rozłącz"))
                {
                    _service.Leave();
                    _channel.Close();
                    ToggleElements();

                    return;
                }

                CreateConnection();

                if (!_service.Join(UserNameInput.Text))
                {
                    MessageBox.Show("Twoja nazwa użytkownika już istnieje.", "Problem z logowaniem");

                    return;
                }

                ToggleElements();

            }
            catch
            {
                MessageBox.Show("Wystąpił błąd krytyczny programu. \nProgram zostanie zamknięty.",
                    "Błąd krytyczny programu", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        #endregion
    }
}