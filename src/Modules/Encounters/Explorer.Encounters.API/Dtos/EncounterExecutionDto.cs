namespace Explorer.Encounters.API.Dtos
{
    public class EncounterExecutionDto
    {
        public long Id { get; set; }
        public long EncounterId { get; set; }
        public EncounterDto Encounter { get; set; }
        public long TouristId { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
    }
}
