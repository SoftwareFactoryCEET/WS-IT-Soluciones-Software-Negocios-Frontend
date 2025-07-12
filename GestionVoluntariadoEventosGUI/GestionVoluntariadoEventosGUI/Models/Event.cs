using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVoluntariadoEventosGUI.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string? SpecialRequirements { get; set; }
        public int VolunteersRequired { get; set; }
        public string OrganizerContact { get; set; } = string.Empty;
    }
}
