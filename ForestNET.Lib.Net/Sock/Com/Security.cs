namespace ForestNET.Lib.Net.Sock.Com
{
    /// <summary>
    /// All security settings for network communication within the framework.
    /// 
    /// ASYMMETRIC					using standard tls tools for secure communication, like trust store and ssl context instances.
    /// SYMMETRIC_128_BIT_LOW		framework coded 128 bit encryption. encryption instance will be unchanged for the whole lifetime of the socket instance, so this is very low security.
    /// SYMMETRIC_256_BIT_LOW		framework coded 256 bit encryption. encryption instance will be unchanged for the whole lifetime of the socket instance, so this is very low security.
    /// SYMMETRIC_128_BIT_HIGH		framework coded 128 bit encryption. encryption instance will be changed any time data will be sent or received, so this has higher security.
    /// SYMMETRIC_256_BIT_HIGH		framework coded 256 bit encryption. encryption instance will be changed any time data will be sent or received, so this has higher security.
    /// </summary>
    public enum Security
    {
        ASYMMETRIC,
        SYMMETRIC_128_BIT_LOW,
        SYMMETRIC_256_BIT_LOW,
        SYMMETRIC_128_BIT_HIGH,
        SYMMETRIC_256_BIT_HIGH
    }
}
