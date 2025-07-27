using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GestionVoluntariadoEventosGUI.Models
{
    public class AvailabilitySlot : INotifyPropertyChanged
    {
        private string _dayOfWeek = string.Empty;
        public string DayOfWeek
        {
            get => _dayOfWeek;
            set
            {
                _dayOfWeek = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplaySlot)); // Actualizar la propiedad de visualización
            }
        }

        // Cambiar a TimeSpan
        private TimeSpan _startTime;
        public TimeSpan StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplaySlot));
            }
        }

        // Cambiar a TimeSpan
        private TimeSpan _endTime;
        public TimeSpan EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplaySlot));
            }
        }

        // Ajustar el formato de DisplaySlot para TimeSpan
        public string DisplaySlot => $"{DayOfWeek} - From {StartTime:hh\\:mm} to {EndTime:hh\\:mm}"; // Formato para TimeSpan (hh:mm)


        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
