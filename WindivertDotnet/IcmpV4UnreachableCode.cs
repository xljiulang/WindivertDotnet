namespace WindivertDotnet
{
    /// <summary>
    /// IcmpV4错误码
    /// </summary>
    public enum IcmpV4UnreachableCode : byte
    {
#pragma warning disable 1591
        DestinationNetworkUnreachable = 0,
        DestinationHostUnreachable = 1,
        DestinationProtocolUnreachable = 2,
        DestinationPortUnreachable = 3,
        FragmentationRequiredAndDFFlagSet = 4,
        SourceRouteFailed = 5,
        DestinationNetworkUnknown = 6,
        DestinationHostUnknown = 7,
        SourceHostIsolated = 8,
        NetworkAdministrativelyProhibited = 9,
        HostAdministrativelyProhibited = 10,
        NetworkUnreachableForTos = 11,
        HostUnreachableForTos = 12,
        CommunicationAdministrativelyProhibited = 13,
        HostPrecedenceViolation = 14,
        PrecedenceCutoffInEffect = 15,
#pragma warning restore 1591
    }
}
