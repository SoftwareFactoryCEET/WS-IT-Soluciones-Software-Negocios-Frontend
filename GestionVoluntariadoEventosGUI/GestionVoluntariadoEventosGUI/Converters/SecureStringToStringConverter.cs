using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GestionVoluntariadoEventosGUI.Converters
{
    public class SecureStringToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SecureString secureString)
            {
                // ¡Advertencia de seguridad!
                // Exponer SecureString a String es una vulnerabilidad potencial.
                // Esto solo debe usarse para fines de depuración o para la funcionalidad "Show Password"
                // bajo la clara responsabilidad del usuario de entender el riesgo.
                // En producción, reevalúa si esta funcionalidad es realmente necesaria de esta manera.
                return new System.Net.NetworkCredential(string.Empty, secureString).Password;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
