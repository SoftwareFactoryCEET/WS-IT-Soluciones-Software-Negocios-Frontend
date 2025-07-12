using GestionVoluntariadoEventosGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVoluntariadoEventosGUI.Servicios
{
    public interface INavigationService
    {
        void NavigateTo(BaseViewModel viewModel);
        void GoBack();
    }
}
