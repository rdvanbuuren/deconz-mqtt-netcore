namespace DeConzToMqtt.Domain.DeConz
{
    /// <summary>
    /// Options that configure the conneciton to deCONZ.
    /// </summary>
    public class DeConzOptions
    {
        /// <summary>
        /// Gets or set the host (ip address or hostname).
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the api key.
        /// </summary>
        public string ApiKey { get; set; }
    }
}