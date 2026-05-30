namespace PricingEngine.Infrastructure.Messaging.Options;

/// <summary>
/// Strongly-typed settings for the RabbitMQ connection, bound from the <c>RabbitMq</c> configuration section.
/// </summary>
public class RabbitMqOptions
{
    /// <summary>Gets or sets the RabbitMQ broker hostname or IP address.</summary>
    public string Host        { get; set; } = "localhost";

    /// <summary>Gets or sets the RabbitMQ virtual host to connect to.</summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>Gets or sets the username used for broker authentication.</summary>
    public string Username    { get; set; } = "guest";

    /// <summary>Gets or sets the password used for broker authentication.</summary>
    public string Password    { get; set; } = "guest";
}
