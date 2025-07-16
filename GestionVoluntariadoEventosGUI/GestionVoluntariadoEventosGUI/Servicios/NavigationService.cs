using GestionVoluntariadoEventosGUI.ViewModels;
using GestionVoluntariadoEventosGUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GestionVoluntariadoEventosGUI.Servicios
{
    public class NavigationService : INavigationService
    {
        private ContentControl _contentControl; // El control en MainWindow que mostrará las vistas
        private Stack<BaseViewModel> _history = new Stack<BaseViewModel>(); // Para el botón "Back"

        public NavigationService(ContentControl contentControl)
        {
            _contentControl = contentControl;
        }
        public void NavigateTo(BaseViewModel viewModel)
        {
            if (_contentControl.Content is UserControl currentView && currentView.DataContext is BaseViewModel currentViewModel)
            {
                _history.Push(currentViewModel); // Guarda el ViewModel actual para "Back"
            }
            _contentControl.Content = CreateViewForViewModel(viewModel);
            _contentControl.DataContext = viewModel; // Establece el DataContext de la vista actual
        }

      
        public void GoBack()
        {
            if (_history.Any())
            {
                var previousViewModel = _history.Pop();
                _contentControl.Content = CreateViewForViewModel(previousViewModel);
                _contentControl.DataContext = previousViewModel;
            }

            // Si no hay historial, podrías volver a la pantalla de login por defecto o cerrar la app
            else
            {
                // Ejemplo: volver a Login si no hay más historial
                NavigateTo(new LoginViewModel(
                    (Application.Current as App)?.ApiService!,
                    (Application.Current as App)?.NavigationService!
                ));
            }
        }
        // Helper para instanciar la vista correcta para un ViewModel dado
        private object CreateViewForViewModel(BaseViewModel viewModel)
        {
            // Puedes usar reflexión o un mapeo directo para esto
            // Esto es un ejemplo simple, para un proyecto más grande considera un IoC container
            if (viewModel is LoginViewModel) {
                
                return new LoginView(); 
            }
            return null; // Deberías manejar esto apropiadamente
        }



    }
}
