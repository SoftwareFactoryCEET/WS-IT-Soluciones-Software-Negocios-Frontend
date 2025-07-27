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

        // Propiedad pública para acceder al ApiService desde cualquier parte de la aplicación
        public ApiService? ApiService { get; private set; }

        // Campo privado para almacenar la instancia de NavigationService
        private NavigationService? _navigationService;

        // Propiedad pública de solo lectura para acceder al NavigationService
        // Su valor se establece a través del método SetNavigationService
        public NavigationService? NavigationService => _navigationService;

        // Este método se ejecuta cuando la aplicación se inicia
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e); // Llama a la implementación base del método OnStartup

            // Inicializa el ApiService con la URL de tu backend
            // ¡Importante! Asegúrate de que esta URL sea la correcta y que tu API esté ejecutándose.
            ApiService = new ApiService("https://localhost:7121/"); // Reemplaza con la URL de tu API

            // La MainWindow se crea automáticamente debido a StartupUri="MainWindow.xaml" en App.xaml.
            // El NavigationService se inicializará en el constructor de MainWindow.xaml.cs
            // una vez que el MainContentControl de MainWindow esté disponible.
        }

        /// <summary>
        /// Establece la instancia de NavigationService de la aplicación.
        /// Este método debe ser llamado una vez desde MainWindow.xaml.cs.
        /// </summary>
        /// <param name="mainContentControl">El ContentControl principal de MainWindow para la navegación.</param>
        /// <exception cref="InvalidOperationException">Se lanza si el NavigationService ya ha sido inicializado.</exception>
        public void SetNavigationService(ContentControl mainContentControl)
        {
            // Verifica si el NavigationService ya fue inicializado para evitar duplicaciones
            if (_navigationService != null)
            {
                throw new InvalidOperationException("NavigationService ya ha sido inicializado. Este método solo debe llamarse una vez.");
            }
            // Verifica que el ContentControl proporcionado no sea nulo
            if (mainContentControl == null)
            {
                throw new ArgumentNullException(nameof(mainContentControl), "El ContentControl para NavigationService no puede ser nulo.");
            }

            // Inicializa el NavigationService con el ContentControl proporcionado
            _navigationService = new NavigationService(mainContentControl);
        }
    }

}
