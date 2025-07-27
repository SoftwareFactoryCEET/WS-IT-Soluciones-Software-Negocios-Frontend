using GestionVoluntariadoEventosGUI.Commands;
using GestionVoluntariadoEventosGUI.Models;
using GestionVoluntariadoEventosGUI.Servicios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace GestionVoluntariadoEventosGUI.ViewModels
{
    public class VolunteerAssignmentViewModel : BaseViewModel
    {

        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;

        public ObservableCollection<Event> AvailableEvents { get; set; } = new ObservableCollection<Event>();
        public ObservableCollection<Volunteer> AvailableVolunteers { get; set; } = new ObservableCollection<Volunteer>();

        private Event? _selectedEvent;
        public Event? SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                _selectedEvent = value;
                OnPropertyChanged();
                ((RelayCommand)AssignVolunteerCommand).RaiseCanExecuteChanged();
                // Puedes cargar más detalles del evento si es necesario
            }
        }

        private Volunteer? _selectedVolunteer;
        public Volunteer? SelectedVolunteer
        {
            get => _selectedVolunteer;
            set
            {
                _selectedVolunteer = value;
                OnPropertyChanged();
                ((RelayCommand)AssignVolunteerCommand).RaiseCanExecuteChanged();
                // Puedes cargar más detalles del voluntario si es necesario
            }
        }

        public ICommand AssignVolunteerCommand { get; }
        public ICommand BackCommand { get; }


        public VolunteerAssignmentViewModel(ApiService apiService, NavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;

            AssignVolunteerCommand = new RelayCommand(async (p) => await AssignVolunteer(), (p) => CanAssignVolunteer());
            BackCommand = new RelayCommand((p) => _navigationService.GoBack());

            _ = LoadData(); // Cargar eventos y voluntarios al iniciar el ViewModel
        }

        private async Task LoadData()
        {
            // Cargar eventos
            var (events, eventError) = await _apiService.GetEventsAsync();
            if (events != null)
            {
                AvailableEvents.Clear();
                foreach (var ev in events)
                {
                    AvailableEvents.Add(ev);
                }
            }
            else
            {
                MessageBox.Show($"Error al cargar eventos: {eventError}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Cargar voluntarios
            var (volunteers, volunteerError) = await _apiService.GetVolunteersAsync();
            if (volunteers != null)
            {
                AvailableVolunteers.Clear();
                foreach (var vol in volunteers)
                {
                    AvailableVolunteers.Add(vol);
                }
            }
            else
            {
                MessageBox.Show($"Error al cargar voluntarios: {volunteerError}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AssignVolunteer()
        {
            if (SelectedEvent == null || SelectedVolunteer == null)
            {
                MessageBox.Show("Debe seleccionar un evento y un voluntario para realizar la asignación.", "Error de Asignación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validaciones de negocio (algunas ya las hace el backend, pero se pueden pre-validar aquí para mejor UX)
            // 1. El evento tiene un número de voluntarios requeridos mayor a cero
            if (SelectedEvent.VolunteersRequired <= 0)
            {
                MessageBox.Show($"El evento '{SelectedEvent.Name}' ya no requiere más voluntarios.", "Error de Asignación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Las fechas y horas disponibles del voluntario coinciden con las del evento
            var eventDateTime = SelectedEvent.DateTime;
            var eventDayOfWeek = eventDateTime.DayOfWeek.ToString();
            var eventStartTime = TimeOnly.FromDateTime(eventDateTime);
            var eventEndTime = TimeOnly.FromDateTime(eventDateTime.AddMinutes(SelectedEvent.DurationMinutes));

            // Verificar si el voluntario tiene una franja de disponibilidad que cubra el evento
            var isVolunteerAvailable = SelectedVolunteer.AvailabilitySlots.Any(slot =>
                string.Equals(slot.DayOfWeek, eventDayOfWeek, StringComparison.OrdinalIgnoreCase) &&
                // Convertir TimeSpan del slot a TimeOnly para la comparación con las horas del evento
                eventStartTime >= TimeOnly.FromTimeSpan(slot.StartTime) &&
                eventEndTime <= TimeOnly.FromTimeSpan(slot.EndTime)
            );

            if (!isVolunteerAvailable)
            {
                MessageBox.Show($"El voluntario '{SelectedVolunteer.FullName}' no está disponible para el evento '{SelectedEvent.Name}' en la fecha y hora especificadas.", "Error de Asignación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Si todas las validaciones pasan, intentar asignar a través de la API
            var errorMessage = await _apiService.AssignVolunteerToEventAsync(SelectedEvent.Id, SelectedVolunteer.Id);

            if (errorMessage == null)
            {
                MessageBox.Show($"Voluntario '{SelectedVolunteer.FullName}' asignado exitosamente al evento '{SelectedEvent.Name}'.", "Asignación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                // Opcional: Recargar los datos o actualizar el número de voluntarios requeridos en el evento seleccionado
                // Esto último es importante para que la UI refleje el cambio inmediatamente.
                if (SelectedEvent != null)
                {
                    SelectedEvent.VolunteersRequired--; // Decrementar localmente
                    OnPropertyChanged(nameof(SelectedEvent)); // Notificar cambio para que la UI se actualice
                }
                _ = LoadData(); // Recargar todos los datos para asegurar consistencia
            }
            else
            {
                MessageBox.Show(errorMessage, "Error de Asignación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAssignVolunteer()
        {
            return SelectedEvent != null && SelectedVolunteer != null;
        }
    }
}
