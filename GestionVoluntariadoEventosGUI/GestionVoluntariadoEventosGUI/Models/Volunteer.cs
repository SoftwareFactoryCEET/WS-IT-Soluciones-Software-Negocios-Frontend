using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionVoluntariadoEventosGUI.Models
{
    public class Volunteer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;//
        public DateOnly BirthDate { get; set; } // 
        public string Skills { get; set; } = string.Empty; // 
        public string Email { get; set; } = string.Empty; // 
        public string PhoneNumber { get; set; } = string.Empty; // 
        public ICollection<AvailabilitySlot> AvailabilitySlots { get; set; } = new List<AvailabilitySlot>();
    }
}
