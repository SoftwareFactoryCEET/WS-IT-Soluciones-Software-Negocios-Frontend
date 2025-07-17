using GestionVoluntariadoEventosGUI.Commands;
using GestionVoluntariadoEventosGUI.Models;
using GestionVoluntariadoEventosGUI.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GestionVoluntariadoEventosGUI.ViewModels
{
    public class EventCreationViewModel : BaseViewModel
    {
		private string _eventName = string.Empty;

		public string EventName
        {
			get { return _eventName; }
			set { _eventName = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
		}

        private DateTime _eventDate = DateTime.Now;
        public DateTime EventDate
        {
            get => _eventDate;
            set
            {
                _eventDate = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }
        private TimeSpan _eventTime = DateTime.Now.TimeOfDay;
        public TimeSpan EventTime
        {
            get => _eventTime;
            set
            {
                _eventTime = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

        private string _eventLocation = string.Empty;
        public string EventLocation
        {
            get => _eventLocation;
            set
            {
                _eventLocation = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

        private string _eventDescription = string.Empty;
        public string EventDescription
        {
            get => _eventDescription;
            set
            {
                _eventDescription = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DescriptionCharCount));
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

        public int DescriptionCharCount => _eventDescription.Length; // 

        private int _estimatedDuration = 5; // Valor inicial según requisito 
        public int EstimatedDuration
        {
            get => _estimatedDuration;
            set
            {
                _estimatedDuration = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

        private string _specialRequirements = string.Empty;
        public string SpecialRequirements
        {
            get => _specialRequirements;
            set
            {
                _specialRequirements = value;
                OnPropertyChanged();
            }
        }

        private int _volunteersRequired = 1; // Valor inicial según requisito 
        public int VolunteersRequired
        {
            get => _volunteersRequired;
            set
            {
                _volunteersRequired = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

        private string _organizerContact = string.Empty;
        public string OrganizerContact
        {
            get => _organizerContact;
            set
            {
                _organizerContact = value;
                OnPropertyChanged();
                ((RelayCommand)CreateCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand CreateCommand { get; }
        public ICommand BackCommand { get; }

        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        //Constructor
        public EventCreationViewModel(ApiService apiService, NavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            CreateCommand = new RelayCommand(async (p) => await CreateEvent(), (p) => CanCreateEvent());
            BackCommand = new RelayCommand((p) => _navigationService.GoBack());
        }

        private async Task CreateEvent()
        {
            DateTime combinedDateTime = EventDate.Date + EventTime;

            var newEvent = new Event
            {
                Name = EventName,
                DateTime = combinedDateTime,
                Location = EventLocation,
                Description = EventDescription,
                DurationMinutes = EstimatedDuration,
                SpecialRequirements = SpecialRequirements,
                VolunteersRequired = VolunteersRequired,
                OrganizerContact = OrganizerContact
            };

            var errorMessage = await _apiService.CreateEventAsync(newEvent);
            if (errorMessage == null)
            {
                MessageBox.Show("Evento registrado exitosamente.", "Registro Exitoso", MessageBoxButton.OK, MessageBoxImage.Information); 
                // Opcional: limpiar los campos o navegar a otra vista
                _navigationService.GoBack(); // O navegar a una vista de lista de eventos
            }
            else
            {
                MessageBox.Show(errorMessage, "Error de Registro de Evento", MessageBoxButton.OK, MessageBoxImage.Error); // [cite: 106]
            }
        }
        private bool CanCreateEvent()
        {
            // Todos los campos son obligatorios excepto Requerimientos Especiales
            if (string.IsNullOrWhiteSpace(EventName) ||
                EventDate == default || // Verifica que se haya seleccionado una fecha
                EventTime == default || // Verifica que se haya seleccionado una hora
                string.IsNullOrWhiteSpace(EventLocation) ||
                string.IsNullOrWhiteSpace(EventDescription) ||                   
                    EstimatedDuration < 5 || // Mínimo 5 minutos 
                VolunteersRequired < 1 || // Mínimo 1 voluntario 
                string.IsNullOrWhiteSpace(OrganizerContact))
            {
                return false;
            }
            // Restricción de fechas: No se pueden registrar eventos en fechas pasadas. 
            if (EventDate.Date + EventTime < DateTime.Now)
            {
                MessageBox.Show("No se puede registrar un evento en una fecha pasada.", "Error de Validación");
                return false;
            }
            // Descripción del evento: máximo 250 caracteres 
            if (EventDescription.Length > 250)
            {
                MessageBox.Show("La descripción del evento no puede exceder los 250 caracteres.", "Error de Validación");
                return false;
            }
            return true; // Si todas las validaciones pasan
        }

        
    }
}
