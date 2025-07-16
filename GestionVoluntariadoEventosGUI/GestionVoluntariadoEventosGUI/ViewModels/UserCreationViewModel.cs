using GestionVoluntariadoEventosGUI.Commands;
using GestionVoluntariadoEventosGUI.Models;
using GestionVoluntariadoEventosGUI.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionVoluntariadoEventosGUI.ViewModels
{
    public class UserCreationViewModel : BaseViewModel
    {
		private string _fullName;

		public string FullName
		{
			get { return _fullName; }
			set { 
				_fullName = value;
                OnPropertyChanged();
				((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
		}

		private string _phoneNumber;

		public string PhoneNumber
		{
			get { return _phoneNumber; }
			set { 
				_phoneNumber = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

		private string _email;

		public string Email
		{
			get { return _email; }
			set { 
				_email = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

        private string _userName = string.Empty;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

        private SecureString _password = new SecureString();	

		public SecureString Password	
		{
			get { return _password; }
			set { 
				_password = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
		}

		private SecureString _repeatPassword = new SecureString();

		public SecureString RepeatPassword
		{
			get { return _repeatPassword; }
			set { _repeatPassword = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

		private bool _termsAccepted;

		public bool TermsAccepted
        {
			get { return _termsAccepted; }
			set { 
				_termsAccepted = value;
                OnPropertyChanged();
               ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
		}

        public ICommand CreateCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ViewTermsCommand { get; }


		//Campos
		private readonly ApiService _apiService;
		private readonly NavigationService _navigationService;
        //ctor

        public UserCreationViewModel(ApiService apiService, NavigationService navigationService)
        {
			_apiService = apiService;
			_navigationService = navigationService;
			CreateCommand = new RelayCommand(async (p) => await CreateUser(), (p) => CanCreateUser());
            BackCommand = new RelayCommand((p)=>_navigationService.GoBack());
			ViewTermsCommand = new RelayCommand((p) => ViewTermsAndConditions());
        }
        private async Task CreateUser()
        {
           string plainPassword = new System.Net.NetworkCredential(string.Empty, Password).Password;
            var newUser = new User()
            {
                FullName = FullName,
                PhoneNumber = PhoneNumber,
                Email = Email,
                UserName = UserName,
                Password = plainPassword,
                TermsAccepted = TermsAccepted
            };

            var errorMessage = await _apiService.RegisterUserAsync(newUser);

            if (errorMessage == null)
            {
                MessageBox.Show("Usuario registrado exitosamente.", "Registro Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                _navigationService.GoBack(); // Volver a la pantalla de login
            }
            else
            {
                MessageBox.Show(errorMessage, "Error de Registro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Limpiar campos después de intentar el registro (opcional)
            Password = new SecureString();
            RepeatPassword = new SecureString();
            OnPropertyChanged(nameof(Password)); // Asegurar que la UI se actualice
            OnPropertyChanged(nameof(RepeatPassword));
        }
        private bool CanCreateUser()
        {
            // Todos los campos obligatorios
            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(PhoneNumber) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(UserName) ||
                Password.Length == 0 ||
                RepeatPassword.Length == 0)
            {
                return false;
            }
            // Validación de teléfono Colombia
            // Asumiendo formato de 10 dígitos que comienza con 3
            if (!Regex.IsMatch(PhoneNumber, @"^3\d{9}$"))
            {
                //MessageBox.Show("El número de teléfono debe ser un número de celular válido en Colombia (10 dígitos, inicia con 3).", "Error de Validación");
                return false;
            }
            // Validación de correo electrónico 
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                //MessageBox.Show("El formato del correo electrónico no es válido.", "Error de Validación");
                return false;
            }
            // Validación de contraseña: al menos 1 mayúscula, 1 minúscula, 1 número 
            string plainPassword = new System.Net.NetworkCredential(string.Empty, Password).Password;
            if (!Regex.IsMatch(plainPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")) // Mínimo 8 caracteres, etc.
            {
                //MessageBox.Show("La contraseña debe contener al menos una mayúscula, una minúscula, un número y tener un mínimo de 8 caracteres.", "Error de Validación");
                return false;
            }
            // Confirmación de contraseña 
            string plainRepeatPassword = new System.Net.NetworkCredential(string.Empty, RepeatPassword).Password;
            if (plainPassword != plainRepeatPassword)
            {
                //MessageBox.Show("Las contraseñas no coinciden.", "Error de Validación");
                return false;
            }
            // Aceptación de términos y condiciones
            if (!TermsAccepted)
            {
                return false;
            }

            return true; // Si todas las validaciones pasan
        }

        
        private void ViewTermsAndConditions()
        {
            // Aquí puedes mostrar una nueva ventana de diálogo con los términos
            _navigationService.NavigateTo(new TermsAndConditionsViewModel()); // Asumiendo que crearás este ViewModel

        }

        
    }
}
