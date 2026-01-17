using System.Text.Json;
using System.Text.Json.Serialization;
using Quickaid.Models.DTO;

namespace Quickaid.Utils
{
    public class AedGeoJsonUtils
    {
        private readonly string _url = "https://openaedmap.org/api/v1/countries/PL.geojson";
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<ExternalAedDto>> FetchExternalAedsAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync(_url);

            var geoJson = JsonSerializer.Deserialize<GeoJsonRoot>(response, _jsonOptions);

            var list = new List<ExternalAedDto>();

            if (geoJson?.Features != null)
            {
                foreach (var feature in geoJson.Features)
                {
                    var coords = feature.Geometry.Coordinates;
                    list.Add(new ExternalAedDto
                    {
                        ExternalId = feature.Properties.OsmId,
                        Longitude = (decimal)coords[0],
                        Latitude = (decimal)coords[1],
                        Description = feature.Properties.Location ?? feature.Properties.Name ?? feature.Properties.Description
                    });
                }
            }

            return list;
        }

        private class GeoJsonRoot
        {
            public string Type { get; set; } = "FeatureCollection";
            public List<GeoJsonFeature>? Features { get; set; }
        }

        private class GeoJsonFeature
        {
            public Geometry Geometry { get; set; } = new Geometry();
            public Properties Properties { get; set; } = new Properties();

        }

        private class Geometry
        {
            public string Type { get; set; } = "Point";
            public List<double> Coordinates { get; set; } = [];
        }

        private class Properties
        {
            [JsonPropertyName("@osm_id")]
            public long OsmId { get; set; }

            public string? Name { get; set; }
            public string? Description { get; set; }

            [JsonPropertyName("defibrillator:location")]
            public string? Location { get; set; }
        }
    }
}
