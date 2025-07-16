using GestionVoluntariadoEventosGUI.Commands;
using GestionVoluntariadoEventosGUI.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GestionVoluntariadoEventosGUI.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
		private string _userName= string.Empty;

		public string UserName
		{
			get { return _userName; }
			set { 
				_userName= value;
				OnPropertyChanged();
			}
		}

		private SecureString _password = new SecureString();

		public SecureString Password
		{
			get { return _password; }
			set { _password = value;
                OnPropertyChanged();
            }
		}

		private bool _showPassword;

		public bool ShowPassword
        {
			get { return _showPassword; }
			set { 
				_showPassword = value;
                OnPropertyChanged();
            }
		}

		public ICommand LoginCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand CreateOneCommand { get; }
        private readonly ApiService _apiService;
        private readonly Servicios.NavigationService _navigationService; // Servicio para navegar entre vistas

        public LoginViewModel(ApiService apiService, Servicios.NavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            LoginCommand = new RelayCommand(async (p) => await Login(), (p) => CanLogin());
            ExitCommand = new RelayCommand((p) => ExitApplication());
            CreateOneCommand = new RelayCommand((p) => NavigateToUserCreation());
        }
        private async Task Login()
        {
            string plainPassword = new System.Net.NetworkCredential(string.Empty, Password).Password;
            var (user, errorMessage) = await _apiService.LoginAsync(UserName, plainPassword);
            if (user != null)
            {
                MessageBox.Show($"¡Bienvenido, {user.FullName}!", "Login Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                //_navigationService.NavigateTo(new MainMenuViewModel(_apiService, _navigationService)); // Navegar a la pantalla principal después del login exitoso [cite: 133]
            }
            else {
                MessageBox.Show(errorMessage ?? "Error desconocido al iniciar sesión.", "Error de Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Password = new SecureString(); // Limpiar la contraseña
        }
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UserName) && Password.Length > 0; //Verifique los requisitos de complegidad de la contraseña
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }
        private void NavigateToUserCreation()
        {
            _navigationService.NavigateTo(new UserCreationViewModel(_apiService, _navigationService));
        }
    }
}
