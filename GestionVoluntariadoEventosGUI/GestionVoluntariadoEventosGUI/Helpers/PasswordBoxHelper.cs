using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GestionVoluntariadoEventosGUI.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(SecureString),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(null, OnBoundPasswordChanged)
            );

        public static SecureString GetBoundPassword(DependencyObject obj)
        {
            return (SecureString)obj.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject obj, SecureString value)
        {
            obj.SetValue(BoundPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if (e.NewValue is SecureString newPassword && !AreSecureStringsEqual(passwordBox.SecurePassword, newPassword))
                {
                    passwordBox.Password = new System.Net.NetworkCredential(string.Empty, newPassword).Password;
                }
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged; // Evita suscripciones múltiples
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetBoundPassword(passwordBox, passwordBox.SecurePassword);
            }
        }

        private static bool AreSecureStringsEqual(SecureString ss1, SecureString ss2)
        {
            if (ss1 == null && ss2 == null) return true;
            if (ss1 == null || ss2 == null) return false;

            if (ss1.Length != ss2.Length) return false;

            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;
            try
            {
                bstr1 = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(ss1);
                bstr2 = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(ss2);
                return System.Runtime.InteropServices.Marshal.ReadInt32(bstr1) == System.Runtime.InteropServices.Marshal.ReadInt32(bstr2);
            }
            finally
            {
                if (bstr1 != IntPtr.Zero) System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(bstr1);
                if (bstr2 != IntPtr.Zero) System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(bstr2);
            }
        }
    }
}
