namespace GestionVoluntariadoEventosGUI.Models.DTO
{
    public class AvailabilitySlotDto
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; } // <-- ¡TimeOnly aquí!
        public TimeOnly EndTime { get; set; }   // <-- ¡TimeOnly aquí!
    }
}