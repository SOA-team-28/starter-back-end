namespace Explorer.Encounters.API.Dtos
{
    public class EncounterDto
    {
        public long AuthorId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int XP { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string? Image { get; set; }
        public double? Range { get; set; }
        public int? RequiredPeople { get; set; }
        public int CheckPointId { get; set; }
      //  public List<int>? ActiveTouristsIds { get; set; }

        public double? LocationLongitude { get; set; }
        public double? LocationLatitude { get; set; }


    }
}
