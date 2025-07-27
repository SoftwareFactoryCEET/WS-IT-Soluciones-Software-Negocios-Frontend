using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionVoluntariadoEventosGUI.Commands;
using GestionVoluntariadoEventosGUI.Models; // Para AvailabilitySlot (modelo local del frontend)
using GestionVoluntariadoEventosGUI.Servicios;

// IMPORTANTE: Asegúrate de que este 'using' apunte al DTO de tu proyecto de API,
// no a un posible DTO duplicado en el frontend si es que existe.
// Si tu proyecto de API se llama "GestionVoluntariadoEventosAPI", entonces debería ser así:
using ApiVolunteerDto = GestionVoluntariadoEventosGUI.Models.DTO.VolunteerDto;
using ApiAvailabilitySlotDto = GestionVoluntariadoEventosGUI.Models.DTO.AvailabilitySlotDto;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Text.RegularExpressions;

namespace GestionVoluntariadoEventosGUI.ViewModels
{
    public class VolunteerCreationViewModel: BaseViewModel
    {
        // ====================================================================
        // PROPIEDADES DE DATOS DEL VOLUNTARIO
        // ====================================================================

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
                // Asegurarse de que CreateVolunteerCommand esté inicializado antes de llamarlo
                if (CreateVolunteerCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        private DateOnly _birthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-18)); // Por defecto, una edad razonable
        public DateOnly BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(YearsOld)); // Actualiza la edad calculada
                // Asegurarse de que CreateVolunteerCommand esté inicializado antes de llamarlo
                if (CreateVolunteerCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        public int YearsOld => (int)((DateTime.Now - BirthDate.ToDateTime(TimeOnly.MinValue)).TotalDays / 365.25);

        private string _skills = string.Empty;
        public string Skills
        {
            get => _skills;
            set
            {
                _skills = value;
                OnPropertyChanged();
                // Asegurarse de que CreateVolunteerCommand esté inicializado antes de llamarlo
                if (CreateVolunteerCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                // Asegurarse de que CreateVolunteerCommand esté inicializado antes de llamarlo
                if (CreateVolunteerCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
                // Asegurarse de que CreateVolunteerCommand esté inicializado antes de llamarlo
                if (CreateVolunteerCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        // ====================================================================
        // PROPIEDADES PARA AÑADIR DISPONIBILIDAD
        // ====================================================================

        public ObservableCollection<string> DaysOfWeek { get; set; } // Para el ComboBox de días

        private string _selectedDayOfWeek = "Monday"; // Valor por defecto
        public string SelectedDayOfWeek
        {
            get => _selectedDayOfWeek;
            set
            {
                _selectedDayOfWeek = value;
                OnPropertyChanged();
                // Asegurarse de que AddAvailabilityCommand esté inicializado antes de llamarlo
                if (AddAvailabilityCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        private TimeSpan? _selectedStartTime; // Hora por defecto (ej. 09:00:00)
        public TimeSpan? SelectedStartTime // ¡Es nullable!
        {
            get => _selectedStartTime;
            set
            {
                _selectedStartTime = value;
                OnPropertyChanged();
                // Asegurarse de que AddAvailabilityCommand esté inicializado antes de llamarlo
                if (AddAvailabilityCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        private TimeSpan? _selectedEndTime; // Hora por defecto (ej. 10:00:00)
        public TimeSpan? SelectedEndTime // ¡Es nullable!
        {
            get => _selectedEndTime;
            set
            {
                _selectedEndTime = value;
                OnPropertyChanged();
                // Asegurarse de que AddAvailabilityCommand esté inicializado antes de llamarlo
                if (AddAvailabilityCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<AvailabilitySlot> AvailabilitySlots { get; set; } = new ObservableCollection<AvailabilitySlot>();

        private AvailabilitySlot? _selectedAvailabilitySlot;
        public AvailabilitySlot? SelectedAvailabilitySlot
        {
            get => _selectedAvailabilitySlot;
            set
            {
                _selectedAvailabilitySlot = value;
                OnPropertyChanged();
                // Asegurarse de que DeleteAvailabilityCommand esté inicializado antes de llamarlo
                if (DeleteAvailabilityCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        // ====================================================================
        // COMANDOS Y SERVICIOS
        // ====================================================================

        public ICommand CreateVolunteerCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand AddAvailabilityCommand { get; }
        public ICommand DeleteAvailabilityCommand { get; }

        private readonly ApiService _apiService;
        private readonly NavigationService _navigationService;


        // ESTE ES EL CONSTRUCTOR QUE DEBE USARSE SIEMPRE
        public VolunteerCreationViewModel(ApiService apiService, NavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;

            // Inicialización de comandos
            CreateVolunteerCommand = new RelayCommand(async (p) => await CreateVolunteer(), (p) => CanCreateVolunteer());
            BackCommand = new RelayCommand((p) => _navigationService.GoBack());
            AddAvailabilityCommand = new RelayCommand((p) => AddAvailability(), (p) => CanAddAvailability());
            DeleteAvailabilityCommand = new RelayCommand((p) => DeleteAvailability(), (p) => CanDeleteAvailability());

            DaysOfWeek = new ObservableCollection<string>(Enum.GetNames(typeof(DayOfWeek)).ToList());

            // Asegurarse de que SelectedStartTime y SelectedEndTime tengan valores iniciales válidos
            // Estos son los valores por defecto al cargar la vista, antes de que el usuario interactúe.
            SelectedStartTime = new TimeSpan(9, 0, 0);
            SelectedEndTime = new TimeSpan(10, 0, 0);
        }


        // ====================================================================
        // MÉTODOS DE LÓGICA DE NEGOCIO (COMANDOS)
        // ====================================================================

        private async Task CreateVolunteer()
        {
            // Nota: Aquí se usa ApiVolunteerDto y ApiAvailabilitySlotDto del namespace de tu API.
            // Asegúrate de que los 'using' alias (ApiVolunteerDto, ApiAvailabilitySlotDto) al principio del archivo
            // apunten correctamente a los DTOs de tu proyecto de API.
            var volunteerDto = new ApiVolunteerDto
            {
                FullName = FullName,
                BirthDate = BirthDate, // BirthDate ya es DateOnly, se envía directamente
                Skills = Skills,
                Email = Email,
                PhoneNumber = PhoneNumber,
                AvailabilitySlots = AvailabilitySlots.Select(s => new ApiAvailabilitySlotDto
                {
                    DayOfWeek = s.DayOfWeek,
                    // Convertir TimeSpan (del modelo local) a TimeOnly (para el DTO de la API)
                    // Manejar la posibilidad de que StartTime/EndTime del slot no sean nulos
                    StartTime = TimeOnly.FromTimeSpan(s.StartTime),
                    EndTime = TimeOnly.FromTimeSpan(s.EndTime)
                }).ToList()
            };

            var errorMessage = await _apiService.RegisterVolunteerAsync(volunteerDto);

            if (errorMessage == null)
            {
                MessageBox.Show("Voluntario registrado exitosamente.", "Registro Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                _navigationService.GoBack();
            }
            else
            {
                MessageBox.Show(errorMessage, "Error de Registro de Voluntario", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool CanCreateVolunteer()
        {
            // Validaciones de campos obligatorios
            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(Skills) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(PhoneNumber) ||
                BirthDate == default || // Verifica que BirthDate no sea su valor predeterminado
                AvailabilitySlots.Count == 0) // Debe haber al menos una franja horaria
            {
                return false;
            }

            // Validación de edad mínima (ej. 16 años, ajusta si es necesario)
            if (YearsOld < 16)
            {
                return false;
            }

            // Validación de formato de correo electrónico
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return false;
            }

            // Validación de formato de teléfono Colombia (10 dígitos, inicia con 3)
            if (!Regex.IsMatch(PhoneNumber, @"^3\d{9}$"))
            {
                return false;
            }

            // Validaciones de franjas horarias (se refuerzan aquí aunque se validan al añadir)
            if (AvailabilitySlots.Any(slot => slot.StartTime >= slot.EndTime ||
                                              (slot.EndTime - slot.StartTime).TotalMinutes < 60 ||
                                              slot.StartTime.Minutes != 0 ||
                                              slot.EndTime.Minutes != 0))
            {
                return false;
            }

            return true; // Si todas las validaciones pasan
        }

        private void AddAvailability()
        {
            // Validar que las horas seleccionadas no sean nulas antes de operar con ellas
            if (!SelectedStartTime.HasValue || !SelectedEndTime.HasValue)
            {
                MessageBox.Show("Debe seleccionar una hora de inicio y una hora de fin válidas.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validaciones de franja horaria usando .Value para acceder al TimeSpan
            if (SelectedStartTime.Value >= SelectedEndTime.Value)
            {
                MessageBox.Show("La hora de inicio debe ser menor que la hora de finalización.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((SelectedEndTime.Value - SelectedStartTime.Value).TotalMinutes < 60)
            {
                MessageBox.Show("Las franjas horarias deben tener una duración mínima de una hora.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedStartTime.Value.Minutes != 0 || SelectedEndTime.Value.Minutes != 0)
            {
                MessageBox.Show("Solo se considerarán horas exactas (ej. 12:00, 13:00, etc.).", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar franjas horarias duplicadas
            if (AvailabilitySlots.Any(s =>
                s.DayOfWeek == SelectedDayOfWeek &&
                s.StartTime == SelectedStartTime.Value &&
                s.EndTime == SelectedEndTime.Value))
            {
                MessageBox.Show("No se pueden registrar franjas horarias duplicadas.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Añadir la nueva franja de disponibilidad a la colección observable
            AvailabilitySlots.Add(new Models.AvailabilitySlot
            {
                DayOfWeek = SelectedDayOfWeek,
                StartTime = SelectedStartTime.Value,
                EndTime = SelectedEndTime.Value
            });

            // Limpiar los TimePicker para la próxima adición (opcional pero mejora UX)
            SelectedStartTime = new TimeSpan(9, 0, 0); // Asigna un TimeSpan, que se convierte a TimeSpan?
            SelectedEndTime = new TimeSpan(10, 0, 0);  // Asigna un TimeSpan, que se convierte a TimeSpan?
            OnPropertyChanged(nameof(SelectedStartTime)); // Forzar actualización de la UI
            OnPropertyChanged(nameof(SelectedEndTime));   // Forzar actualización de la UI

            // Forzar reevaluación del comando CreateVolunteer para habilitar/deshabilitar el botón
            if (CreateVolunteerCommand is RelayCommand createCmd) createCmd.RaiseCanExecuteChanged();
        }

        private bool CanAddAvailability()
        {
            // Solo permitir añadir si se ha seleccionado un día y las horas son válidas y no nulas
            return !string.IsNullOrEmpty(SelectedDayOfWeek) &&
                   SelectedStartTime.HasValue && SelectedEndTime.HasValue && // Comprobar HasValue
                   SelectedStartTime.Value < SelectedEndTime.Value &&        // Usar .Value
                   (SelectedEndTime.Value - SelectedStartTime.Value).TotalMinutes >= 60 && // Usar .Value
                   SelectedStartTime.Value.Minutes == 0 && SelectedEndTime.Value.Minutes == 0; // Usar .Value
        }

        private void DeleteAvailability()
        {
            if (SelectedAvailabilitySlot != null)
            {
                AvailabilitySlots.Remove(SelectedAvailabilitySlot);
                SelectedAvailabilitySlot = null; // Limpiar selección

                // Forzar reevaluación del comando CreateVolunteer para habilitar/deshabilitar el botón
                if (CreateVolunteerCommand is RelayCommand cmd) cmd.RaiseCanExecuteChanged();
            }
        }

        private bool CanDeleteAvailability()
        {
            return SelectedAvailabilitySlot != null;
        }
    }
}
