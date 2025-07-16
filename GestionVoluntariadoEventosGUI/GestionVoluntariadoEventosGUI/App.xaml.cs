using GestionVoluntariadoEventosGUI.Servicios;
using GestionVoluntariadoEventosGUI.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GestionVoluntariadoEventosGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public ApiService? ApiService { get; private set; }
        public NavigationService? NavigationService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // 1. Instanciar ApiService con la URL de tu backend
            string baseApiUrl = "https://localhost:7121/"; // ¡Asegúrate de que esta URL sea correcta para tu API!
            ApiService = new ApiService(baseApiUrl);

            // 2. Instanciar MainWindow
            var mainWindow = new MainWindow();

            // 3. Crear el NavigationService, pasándole el ContentControl de MainWindow
            // Asegúrate de que MainContentControl esté definido en MainWindow.xaml
            NavigationService = new NavigationService(mainWindow.FindName("MainContentControl") as ContentControl);

            // 4. Iniciar con el LoginViewModel
            NavigationService.NavigateTo(new LoginViewModel(ApiService, NavigationService));

            mainWindow.Show();
        }
    }

}
