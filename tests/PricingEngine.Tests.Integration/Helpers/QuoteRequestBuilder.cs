using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PricingEngine.Tests.Integration.Helpers;

/// <summary>
/// Produces <see cref="StringContent"/> (application/json) for the
/// <c>POST /api/v1/quotes</c> endpoint.
/// </summary>
public static class QuoteRequestBuilder
{
    private static StringContent Json(object payload)
        => new(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    /// <summary>HOME_V1 — insuredSum drives the net premium (tariff 0.5%).</summary>
    public static StringContent ForHomeV1(decimal insuredSum = 100_000m)
        => Json(new
        {
            productCode = "HOME_V1",
            payload     = new { insuredSum },
        });

    /// <summary>AUTO_V1 — combines vehicleValue, driverAge, coverageType, and an optional rider.</summary>
    public static StringContent ForAutoV1(
        decimal vehicleValue = 20_000m,
        int     driverAge    = 30,
        string  coverageType = "Third Party",
        bool    rider        = false)
        => Json(new
        {
            productCode = "AUTO_V1",
            payload     = new
            {
                vehicleValue,
                driverAge,
                coverageType,
                comprehensiveRider = rider,
            },
        });

    /// <summary>Request whose payload is a raw string instead of a JSON object.</summary>
    public static StringContent InvalidPayload(string productCode = "HOME_V1")
    {
        // Manually compose JSON so the payload value is a string, not an object.
        var raw = $$"""{"productCode":"{{productCode}}","payload":"not-an-object"}""";
        return new StringContent(raw, Encoding.UTF8, "application/json");
    }

    /// <summary>Request with an empty product code string.</summary>
    public static StringContent EmptyProductCode()
        => Json(new
        {
            productCode = "",
            payload     = new { insuredSum = 100_000m },
        });
}
