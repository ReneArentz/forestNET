namespace ForestNET.Lib.Net.Sock.Com
{
    /// <summary>
    /// Cardinality relation in communication structure between message boxes and sockets.
    /// 
    /// Equal							same amount of message boxes are handled by the same amount of sockets.
    /// EqualBidirectional				two message boxes are handled by one socket. one message box is for receiving data the other for sending data.
    /// OneMessageBoxToManySockets		one message box send or receive all messages for an amount of sockets.
    /// ManyMessageBoxesToOneSocket		many message boxes send or receive all message for one socket.
    /// </summary>
    public enum Cardinality
    {
        Equal, EqualBidirectional, OneMessageBoxToManySockets, ManyMessageBoxesToOneSocket
    }
}
