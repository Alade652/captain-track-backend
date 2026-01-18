namespace CaptainTrackBackend.Application.DTO.MapModels
{
    public class TimeZoneResponse
    {
        public string status { get; set; }
        public string timeZoneId { get; set; }
        public string timeZoneName { get; set; }
        public long dstOffset { get; set; }
        public long rawOffset { get; set; }
    }
}