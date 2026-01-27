using System.Text.Json;
using System.Text.Json.Serialization;
using Quickaid.Models.DTO;

namespace Quickaid.Utils
{
    // Narzędzie do pobierania AED z zewnętrznego API w formacie GeoJSON
    // Obsługuje cache i synchronizację wielowątkową
    public class AedGeoJsonUtils
    {
        private readonly string _url = "https://openaedmap.org/api/v1/countries/PL.geojson";

        // Ustawienia serializacji JSON
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // Cache AED, żeby nie pobierać z API za często
        private static List<ExternalAedDto>? _cachedAeds;
        private static DateTime _cacheTimestamp;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(2);

        // Semafor do synchronizacji pobierania danych
        private static readonly SemaphoreSlim _lock = new(1, 1);

        // Pobiera AED z API lub z cache, jeśli dane są aktualne
        public async Task<List<ExternalAedDto>> FetchExternalAedsAsync()
        {
            // Jeśli cache jest ważny, zwracamy dane z cache
            if (_cachedAeds != null && DateTime.UtcNow - _cacheTimestamp < CacheTtl)
                return _cachedAeds;

            // Lock, żeby tylko jeden request naraz mógł pobierać dane z API
            await _lock.WaitAsync();
            try
            {
                // Drugi check po locku: jeśli cache został już odświeżony przez inny request
                if (_cachedAeds != null && DateTime.UtcNow - _cacheTimestamp < CacheTtl)
                    return _cachedAeds;

                using var client = new HttpClient();
                var response = await client.GetStringAsync(_url);

                // Deserializacja GeoJSON
                var geoJson = JsonSerializer.Deserialize<GeoJsonRoot>(response, _jsonOptions);

                var list = new List<ExternalAedDto>();

                if (geoJson?.Features != null)
                {
                    foreach (var feature in geoJson.Features)
                    {
                        var coords = feature.Geometry.Coordinates;

                        // Konwersja feature GeoJSON na ExternalAedDto
                        list.Add(new ExternalAedDto
                        {
                            ExternalId = feature.Properties.OsmId,
                            Longitude = (decimal)coords[0],
                            Latitude = (decimal)coords[1],
                            Description = feature.Properties.Location
                                ?? feature.Properties.Name
                                ?? feature.Properties.Description,
                            Type = Quickaid.Enums.AedType.External
                        });
                    }
                }

                // Aktualizacja cache
                _cachedAeds = list;
                _cacheTimestamp = DateTime.UtcNow;
                
                return list;
            }
            finally
            {
                _lock.Release();
            }
        }

        // Klasy pomocnicze do deserializacji GeoJSON

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
