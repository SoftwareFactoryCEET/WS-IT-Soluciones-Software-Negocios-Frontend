using GestionVoluntariadoEventosGUI.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestionVoluntariadoEventosGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var app = (App)Application.Current;

            // Asegurarse de que MainContentControl no sea nulo.
            if (MainContentControl == null)
            {
                throw new InvalidOperationException("MainContentControl no se encontró en MainWindow.xaml.");
            }

            // Inicializar NavigationService a través de un método en App
            app.SetNavigationService(MainContentControl);

            // Realizar la primera navegación a la vista de Login
            if (app.ApiService == null)
            {
                throw new InvalidOperationException("ApiService no está inicializado en App.xaml.cs.");
            }
            if (app.NavigationService == null)
            {
                throw new InvalidOperationException("NavigationService no está inicializado correctamente.");
            }

            // Aquí se pasa el NavigationService ya inicializado y accesible desde app.NavigationService
            app.NavigationService.NavigateTo(new LoginViewModel(app.ApiService, app.NavigationService));
        }
    }
}