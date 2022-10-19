namespace WindivertDotnet
{
    /// <summary>
    /// IcmpV6消息类型
    /// </summary>
    public enum IcmpV6MessageType : byte
    {
#pragma warning disable 1591
        DestinationUnreachable = 1,
        PacketTooBig = 2,
        TimeExceeded = 3,
        ParameterProblem = 4,
        EchoRequest = 128,
        EchoReply = 129,
#pragma warning restore 1591
    }
}
