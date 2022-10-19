namespace WindivertDotnet
{
    /// <summary>
    /// IcmpV4消息类型
    /// </summary>
    public enum IcmpV4MessageType : byte
    {
#pragma warning disable 1591
        EchoReply = 0,
        DestinationUnreachable = 3,
        SourceQuench = 4,
        RedirectMessage = 5,
        EchoRequest = 8,
        RouterAdvertisement = 9,
        RouterSolicitation = 10,
        TimeExceeded = 11,
        ParameterProblemBadIPHeader = 12,
        Timestamp = 13,
        TimestampReply = 14,
        InformationRequest = 15,
        InformationReply = 16,
        AddressMaskRequest = 17,
        AddressMaskReply = 18,
        Traceroute = 30,
#pragma warning restore 1591
    }
}
