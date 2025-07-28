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
        private readonly ContentControl _contentControl;
        private readonly Stack<BaseViewModel> _history = new Stack<BaseViewModel>();

        public NavigationService(ContentControl contentControl)
        {
            _contentControl = contentControl ?? throw new ArgumentNullException(nameof(contentControl), "ContentControl no puede ser nulo.");
        }

        public void NavigateTo(BaseViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel), "No se puede navegar a un ViewModel nulo.");
            }

            // Guarda el ViewModel actual si existe y es diferente del nuevo
            if (_contentControl.Content is UserControl currentView && currentView.DataContext is BaseViewModel currentViewModel && currentViewModel != viewModel)
            {
                _history.Push(currentViewModel);
            }

            // Crea la nueva vista
            UserControl newView = CreateViewForViewModel(viewModel);
            if (newView == null)
            {
                throw new InvalidOperationException($"No se pudo crear una vista para el ViewModel de tipo: {viewModel.GetType().Name}");
            }

            // *** FIX: Asegurar que el DataContext de la vista recién creada sea el ViewModel correcto ***
            newView.DataContext = viewModel; // Establece el DataContext DIRECTAMENTE en la vista

            _contentControl.Content = newView; // Asigna la vista al ContentControl
            // _contentControl.DataContext = viewModel; // Ya no es estrictamente necesario aquí si lo asignas a newView.DataContext
        }

        public void GoBack()
        {
            if (_history.Any())
            {
                var previousViewModel = _history.Pop();

                UserControl previousView = CreateViewForViewModel(previousViewModel);
                if (previousView == null)
                {
                    throw new InvalidOperationException($"No se pudo recrear la vista para el ViewModel del historial: {previousViewModel.GetType().Name}");
                }

                // *** FIX: Asegurar que el DataContext de la vista anterior sea el ViewModel correcto ***
                previousView.DataContext = previousViewModel; // Establece el DataContext DIRECTAMENTE en la vista

                _contentControl.Content = previousView;
                // _contentControl.DataContext = previousViewModel; // Ya no es estrictamente necesario aquí
            }
            else
            {
                var appInstance = Application.Current as App;
                if (appInstance == null || appInstance.ApiService == null || appInstance.NavigationService == null) // Add check for NavigationService
                {
                    throw new InvalidOperationException("Los servicios de la aplicación no están inicializados correctamente.");
                }
                NavigateTo(new LoginViewModel(appInstance.ApiService, appInstance.NavigationService));
            }
        }

        private UserControl CreateViewForViewModel(BaseViewModel viewModel)
        {
            // La lógica de creación de la vista es correcta.
            return viewModel switch
            {
                LoginViewModel => new LoginView(),
                UserCreationViewModel => new UserCreationView(),
                EventCreationViewModel => new EventCreationView(),
                TermsAndConditionsViewModel => new TermsAndConditionsView(),
                MainMenuViewModel => new MainMenuView(),
                VolunteerCreationViewModel => new VolunteerCreationView(),
                VolunteerAssignmentViewModel => new VolunteerAssignmentView(),
                _ => null
            };
        }
    }
}
