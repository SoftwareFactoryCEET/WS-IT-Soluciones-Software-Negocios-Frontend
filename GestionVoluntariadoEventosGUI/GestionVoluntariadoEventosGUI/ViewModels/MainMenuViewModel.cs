using GestionVoluntariadoEventosGUI.Commands;
using GestionVoluntariadoEventosGUI.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GestionVoluntariadoEventosGUI.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        // Propiedades y comandos para la pantalla principal irán aquí
        // Por ejemplo, comandos para navegar a la creación de eventos, ver voluntarios, etc.
        public ICommand NavigateToEventCreationCommand { get; }
        public ICommand NavigateToVolunteerCreationCommand { get; } // Nuevo
        public ICommand NavigateToVolunteerAssignmentCommand { get; } // Nuevo
        public ICommand LogoutCommand { get; }

        public MainMenuViewModel(ApiService apiService, NavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            // Inicializar comandos
            NavigateToEventCreationCommand = new RelayCommand((p) => NavigateToEventCreation());
            NavigateToVolunteerCreationCommand = new RelayCommand((p) => NavigateToVolunteerCreation()); // Inicializar
            NavigateToVolunteerAssignmentCommand = new RelayCommand((p) => NavigateToVolunteerAssignment()); // Inicializar

            LogoutCommand = new RelayCommand((p) => Logout());
        }

        private void NavigateToVolunteerAssignment()
        {
            _navigationService.NavigateTo(new VolunteerAssignmentViewModel(_apiService, _navigationService));
        }

        private void NavigateToVolunteerCreation()
        {
            _navigationService.NavigateTo(new VolunteerCreationViewModel(_apiService, _navigationService));
        }

        private void NavigateToEventCreation()
        {
            _navigationService.NavigateTo(new EventCreationViewModel(_apiService, _navigationService));
        }

        private void Logout()
        {
            // Lógica para cerrar sesión (por ejemplo, limpiar datos de usuario, volver a la pantalla de login)
            _navigationService.NavigateTo(new LoginViewModel(_apiService, _navigationService));
        }
    }
}
