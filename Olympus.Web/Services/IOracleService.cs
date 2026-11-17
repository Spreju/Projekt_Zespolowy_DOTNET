namespace Olympus.Web.Services;

public interface IOracleService
{
    /// <summary>
    /// Odczytuje znaki z nieba (prawdziwą pogodę z zewnętrznego API)
    /// i na ich podstawie wylicza wskaźnik Równowagi Świata oraz przepowiednię.
    /// </summary>
    Task<OracleReading> ConsultAsync(string city, CancellationToken cancellationToken = default);
}
