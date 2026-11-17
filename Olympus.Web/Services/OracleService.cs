using System.Net.Http.Json;
using System.Text.Json;

namespace Olympus.Web.Services;

// Integracja z zewnętrznym API: Open-Meteo (darmowe, bez klucza API)
// https://open-meteo.com/
public class OracleService : IOracleService
{
    private const string GeocodingUrl = "https://geocoding-api.open-meteo.com/v1/search";
    private const string ForecastUrl = "https://api.open-meteo.com/v1/forecast";

    private readonly HttpClient _httpClient;
    private readonly ILogger<OracleService> _logger;

    public OracleService(HttpClient httpClient, ILogger<OracleService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OracleReading> ConsultAsync(string city, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new OracleException("Wyrocznia musi znać nazwę krainy, by odczytać znaki.");
        }

        try
        {
            var location = await FindLocationAsync(city.Trim(), cancellationToken);
            var weather = await GetCurrentWeatherAsync(location, cancellationToken);

            var balance = CalculateWorldBalance(weather);

            return new OracleReading
            {
                City = location.Name,
                Country = location.Country ?? string.Empty,
                Temperature = weather.Temperature,
                WindSpeed = weather.WindSpeed,
                Precipitation = weather.Precipitation,
                WeatherCode = weather.WeatherCode,
                WeatherDescription = DescribeWeather(weather.WeatherCode),
                WorldBalance = balance,
                BalanceLevel = DescribeBalance(balance),
                Prophecy = CreateProphecy(weather.WeatherCode, location.Name),
                Recommendation = CreateRecommendation(balance),
                ReadAt = DateTime.Now
            };
        }
        catch (OracleException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Błąd połączenia z API pogodowym dla miasta {City}", city);
            throw new OracleException("Mgła zasłoniła Delfy – nie udało się połączyć z zewnętrznym API. Spróbuj później.", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Przekroczono czas oczekiwania na API pogodowe dla miasta {City}", city);
            throw new OracleException("Bogowie milczą zbyt długo – API nie odpowiedziało na czas.", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Nieprawidłowa odpowiedź API pogodowego dla miasta {City}", city);
            throw new OracleException("Znaki są nieczytelne – API zwróciło niepoprawne dane.", ex);
        }
    }

    private async Task<GeocodingResult> FindLocationAsync(string city, CancellationToken cancellationToken)
    {
        var url = $"{GeocodingUrl}?name={Uri.EscapeDataString(city)}&count=1&language=pl&format=json";
        var response = await _httpClient.GetFromJsonAsync<GeocodingResponse>(url, cancellationToken);

        var location = response?.Results?.FirstOrDefault();
        if (location == null)
        {
            throw new OracleException($"Wyrocznia nie zna krainy o nazwie „{city}”.");
        }

        return location;
    }

    private async Task<CurrentWeather> GetCurrentWeatherAsync(GeocodingResult location, CancellationToken cancellationToken)
    {
        var lat = location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var lon = location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var url = $"{ForecastUrl}?latitude={lat}&longitude={lon}" +
                  "&current=temperature_2m,wind_speed_10m,precipitation,weather_code";

        var response = await _httpClient.GetFromJsonAsync<ForecastResponse>(url, cancellationToken);

        return response?.Current
            ?? throw new OracleException("Niebo nad tą krainą jest puste – brak danych pogodowych.");
    }

    // Im spokojniejsza pogoda, tym większa Równowaga Świata
    private static int CalculateWorldBalance(CurrentWeather weather)
    {
        double balance = 100;

        // Skrajne temperatury zaburzają równowagę
        if (weather.Temperature > 30) balance -= (weather.Temperature - 30) * 3;
        if (weather.Temperature < 0) balance -= Math.Abs(weather.Temperature) * 2;

        // Silny wiatr – gniew Eola
        if (weather.WindSpeed > 20) balance -= (weather.WindSpeed - 20) * 1.5;

        // Opady – łzy bogów
        balance -= weather.Precipitation * 5;

        // Burza to bezpośrednia interwencja Zeusa
        if (weather.WeatherCode >= 95) balance -= 30;

        return (int)Math.Clamp(Math.Round(balance), 0, 100);
    }

    private static string DescribeBalance(int balance) => balance switch
    {
        >= 80 => "Harmonia",
        >= 60 => "Spokój",
        >= 40 => "Niepokój",
        >= 20 => "Zamęt",
        _ => "Chaos"
    };

    // Kody pogodowe WMO używane przez Open-Meteo
    private static string DescribeWeather(int code) => code switch
    {
        0 => "Bezchmurne niebo",
        1 or 2 => "Lekkie zachmurzenie",
        3 => "Pochmurno",
        45 or 48 => "Mgła",
        >= 51 and <= 57 => "Mżawka",
        >= 61 and <= 67 => "Deszcz",
        >= 71 and <= 77 => "Śnieg",
        >= 80 and <= 82 => "Przelotne ulewy",
        85 or 86 => "Przelotne opady śniegu",
        >= 95 => "Burza",
        _ => "Nieznane zjawisko"
    };

    private static string CreateProphecy(int code, string city) => code switch
    {
        0 => $"Helios jaśnieje nad {city}. Bogowie są zadowoleni – nadchodzi czas pomyślności.",
        1 or 2 or 3 => $"Chmury Zeusa wiszą nad {city}. Bogowie obserwują i czekają na ofiary.",
        45 or 48 => $"Mgła okryła {city}. Prawda jest ukryta – nie podejmuj pochopnych decyzji.",
        >= 51 and <= 67 or >= 80 and <= 82 => $"Posejdon zsyła wody na {city}. Ktoś zapomniał o należnej ofierze.",
        >= 71 and <= 77 or 85 or 86 => $"Chłód Boreasza ogarnął {city}. Nadchodzi czas próby dla śmiertelników.",
        >= 95 => $"Pioruny Zeusa biją w {city}! Gniew Olimpu jest wielki – potrzebna natychmiastowa interwencja.",
        _ => $"Znaki nad {city} są niejasne. Wyrocznia potrzebuje więcej czasu."
    };

    private static string CreateRecommendation(int balance) => balance switch
    {
        >= 80 => "Brak potrzeby interwencji. Można rozważyć błogosławieństwo dla krainy.",
        >= 60 => "Zalecane drobne błogosławieństwo, aby utrzymać spokój.",
        >= 40 => "Zalecane zebranie modlitw i ofiar od śmiertelników z tej krainy.",
        >= 20 => "Zalecana interwencja boska w regionie.",
        _ => "Stan krytyczny! Zalecana interwencja o skali globalnej – wymaga zatwierdzenia przez Zeusa."
    };
}
