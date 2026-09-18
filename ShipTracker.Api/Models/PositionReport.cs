namespace ShipTracker.Api.Models
{
    public class AisMessage
    {
        public required MessageContainer Message { get; set; }
        public MetaData? MetaData { get; internal set; }
    }

    public class MetaData
    {
        public long MMSI { get; set; }
        public string? MMSI_String { get; set; }
        public string? ShipName { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? time_utc { get; set; }
        public string? MessageType { get; set; }
    }

    public class MessageContainer
    {
        public PositionReport PositionReport { get; set; }
    }

    public class PositionReport
    {
        public int MessageID { get; set; }
        public int RepeatIndicator { get; set; }
        public long UserID { get; set; }
        public bool Valid { get; set; }
        public int NavigationalStatus { get; set; }
        public int RateOfTurn { get; set; }
        public double Sog { get; set; }
        public bool PositionAccuracy { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Cog { get; set; }
        public int TrueHeading { get; set; }
        public int Timestamp { get; set; }
        public int SpecialManoeuvreIndicator { get; set; }
        public int Spare { get; set; }
        public bool Rain { get; set; }
        public int CommunicationState { get; set; }
    }
}
