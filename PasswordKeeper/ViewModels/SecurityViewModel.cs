using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PasswordKeeper.Models;
using PasswordKeeper.Views;
using System.ComponentModel;
using System.Windows;

namespace PasswordKeeper.ViewModels
{
    public class SecurityViewModel : ViewModelBase
    {
        private LoginPasswordAccess _loginPasswordAccess;
        private bool _allowClosing; // Чтобы не возникало диалоговое окно закрытия программы, при закрытии этой формы и открытии следующей.

        public SecurityViewModel()
        {
            _loginPasswordAccess = new LoginPasswordAccess();
            LoginCommand = new RelayCommand<Window>(OnLoginCommand, o => Password.Length > 0);
            _allowClosing = true;
        }


        public RelayCommand<CancelEventArgs> ClosingCommand
        {
            get
            {
                return new RelayCommand<CancelEventArgs>((e) =>
                {
                    if (_allowClosing)
                    {
                        var result = MessageBox.Show("Вы действительно хотите закрыть программу?", "Выход", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            Application.Current.Shutdown();
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                });
            }
        }


        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                LoginResultVisibility = Visibility.Hidden;
                LoginCommand.RaiseCanExecuteChanged();
            }
        }


        private Visibility _loginResultVisibility = Visibility.Hidden;
        public Visibility LoginResultVisibility
        {
            get => _loginResultVisibility;
            set
            {
                _loginResultVisibility = value;
                RaisePropertyChanged();
            }
        }

 
        public RelayCommand<Window> LoginCommand { get; }
        private void OnLoginCommand(Window window)
        {
            if (Password == _loginPasswordAccess.LoginPassword)
            {
                _allowClosing = false; // Запрещаем появление диалогового окна закрытия формы.
                PasswordView passwordView = new PasswordView();
                passwordView.Show();
                window.Close();
            }
            else
            {
                LoginResultVisibility = Visibility.Visible;
            }
        }
    }
}
