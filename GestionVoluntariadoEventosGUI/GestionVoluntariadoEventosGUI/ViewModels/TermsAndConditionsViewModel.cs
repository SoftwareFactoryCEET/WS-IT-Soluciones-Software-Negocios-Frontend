using GestionVoluntariadoEventosGUI.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GestionVoluntariadoEventosGUI.ViewModels
{
    public class TermsAndConditionsViewModel : BaseViewModel
    {
		private string _termsContent = string.Empty;

		public string TermsContent
		{
			get { return _termsContent; }
			set {
				_termsContent = value;
                OnPropertyChanged();
            }
		}
        public ICommand	CloseCommand { get; }

        //Constructor
        public TermsAndConditionsViewModel()
        {
            LoadTermsContent();
            CloseCommand = new RelayCommand((p) => CloseWindow());

        }
       
        private void LoadTermsContent()
        {
            try
            {
                // Carga el contenido del archivo terms.txt
                // Asegúrate de que terms.txt esté en tu proyecto y tenga la propiedad "Copy to Output Directory" = "Copy if newer"
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"assets\docs\terms.txt");
                //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "docs", "terms.txt");
                if (File.Exists(filePath))
                {
                    TermsContent = File.ReadAllText(filePath);
                }
                else
                {
                    TermsContent = "El archivo terms.txt no se encontró.";
                }

            }
            catch (Exception ex)
            {

                TermsContent = $"Error al cargar los términos: {ex.Message}";
            }
        }

        private void CloseWindow()
        {
            ((App)Application.Current).NavigationService?.GoBack();
        }

    }
}
