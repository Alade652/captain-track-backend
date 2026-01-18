namespace CaptainTrackBackend.Application.DTO.MapModels
{
    public class ReverseGeocodingResponse
    {
        public string status { get; set; }
        public List<Result> results { get; set; }
    }
}