using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GestionVoluntariadoEventosGUI.Converters
{
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
            {
                // Convertir DateOnly a DateTime para el DatePicker.
                // Usamos MinValue para la hora, ya que DatePicker solo maneja la fecha.
                return dateOnly.ToDateTime(TimeOnly.MinValue);
            }
            return null; // Si el valor es null o no es DateOnly, devolver null
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Convertir DateTime (del DatePicker) a DateOnly para el ViewModel.
                return DateOnly.FromDateTime(dateTime);
            }
            return default(DateOnly); // Si el valor es null o no es DateTime, devolver el valor por defecto de DateOnly
        }
    }
}
