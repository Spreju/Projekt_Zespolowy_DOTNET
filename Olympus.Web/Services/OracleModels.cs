using System.Text.Json.Serialization;

namespace Olympus.Web.Services;

// Wynik pracy Wyroczni zwracany do kontrolera
public class OracleReading
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public double Precipitation { get; set; }
    public int WeatherCode { get; set; }
    public string WeatherDescription { get; set; } = string.Empty;

    // Wskaźnik Równowagi Świata: 0 = chaos, 100 = pełna harmonia
    public int WorldBalance { get; set; }
    public string BalanceLevel { get; set; } = string.Empty;
    public string Prophecy { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public DateTime ReadAt { get; set; }
}

// Wyjątek rzucany, gdy Wyrocznia nie może odczytać znaków
public class OracleException : Exception
{
    public OracleException(string message) : base(message) { }
    public OracleException(string message, Exception inner) : base(message, inner) { }
}

// ===== Modele odpowiedzi z Open-Meteo =====

internal class GeocodingResponse
{
    [JsonPropertyName("results")]
    public List<GeocodingResult>? Results { get; set; }
}

internal class GeocodingResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}

internal class ForecastResponse
{
    [JsonPropertyName("current")]
    public CurrentWeather? Current { get; set; }
}

internal class CurrentWeather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed { get; set; }

    [JsonPropertyName("precipitation")]
    public double Precipitation { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }
}
