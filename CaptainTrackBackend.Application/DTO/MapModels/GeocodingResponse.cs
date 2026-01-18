namespace CaptainTrackBackend.Application.DTO.MapModels
{
    public class GeocodingResponse
    {
        public string status { get; set; }
        public List<Result> results { get; set; }
    }

    public class Result
    {
        public Geometry geometry { get; set; }
        public string formatted_address { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}