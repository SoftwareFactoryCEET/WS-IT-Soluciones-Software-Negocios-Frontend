using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GestionVoluntariadoEventosGUI.Converters
{
    public class TimeSpanToNullableTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convierte del ViewModel (TimeSpan?) a la UI (TimeSpan? esperado por TimePicker).
            // Si el ViewModel tiene un TimeSpan (no nulo), lo devolvemos como TimeSpan?.
            if (value is TimeSpan timeSpan)
            {
                return (TimeSpan?)timeSpan;
            }
            // Si el valor del ViewModel es nulo (o no es TimeSpan), devolvemos null.
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Este método convierte de la UI (TimePicker) de vuelta al ViewModel (TimeSpan?).
            // El TimePicker de Material Design puede devolver DateTime o TimeSpan.

            // Caso 1: El valor de la UI es DateTime (parte de la hora)
            if (value is DateTime dateTimeValue) // <-- ¡Correcto! Usar DateTime (no DateTime?)
            {
                // Extraer la parte de la hora como TimeSpan.
                return dateTimeValue.TimeOfDay;
            }

            // Caso 2: El valor de la UI es TimeSpan (directamente, como se espera idealmente de TimePicker)
            if (value is TimeSpan timeSpanValue) // <-- ¡Correcto! Usar TimeSpan (no TimeSpan?)
            {
                return timeSpanValue; // Devolver el TimeSpan directamente
            }

            // Si el valor de entrada no es DateTime, ni TimeSpan (ej. es null), devolver null para TimeSpan? del ViewModel.
            return null;
        }
    }
}
