using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GestionVoluntariadoEventosGUI.Converters
{
    public class BirthDateToAgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Este método se usa para calcular la edad a partir de la fecha de nacimiento.
            // La fecha de nacimiento en el ViewModel (BirthDate) es DateOnly.
            // Si el binding es directamente a BirthDate, entonces el 'value' aquí es DateOnly.
            // Si el binding es a la propiedad que YA ES DateTime, entonces el 'value' aquí es DateTime.

            // Asumiendo que 'value' viene de DateOnly BirthDate (VM) -> Convertidor -> int YearsOld (UI)
            if (value is DateOnly birthDateOnly) // Si la fuente es DateOnly
            {
                int age = (int)((DateTime.Now - birthDateOnly.ToDateTime(TimeOnly.MinValue)).TotalDays / 365.25);
                return age;
            }
            // Si el valor de entrada es DateTime (menos probable si BirthDate es DateOnly en VM)
            if (value is DateTime birthDateDateTime)
            {
                int age = (int)((DateTime.Now - birthDateDateTime).TotalDays / 365.25);
                return age;
            }
            return 0; // Valor por defecto si la entrada no es una fecha válida
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Este método NO se llama si el binding es Mode=OneWay.
            // La propiedad YearsOld es de solo lectura y solo para mostrar.
            // Por lo tanto, no hay una manera lógica de convertir una 'edad' (int) de nuevo a una 'fecha de nacimiento' (DateOnly/DateTime).
            // Lo más apropiado es lanzar una excepción, ya que no se espera que se utilice este camino.
            throw new NotImplementedException("Converting age back to birth date is not supported.");
            // O podrías devolver Binding.DoNothing si no quieres una excepción, pero eso es menos informativo.
            // return Binding.DoNothing;
        }
    }
}
