namespace DeconzToMqtt.Deconz
{
    /// <summary>
    /// Options that configure the websocket.
    /// </summary>
    public class DeconzOptions
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