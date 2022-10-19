namespace WindivertDotnet
{
    /// <summary>
    /// IcmpV6错误码
    /// </summary>
    public enum IcmpV6UnreachableCode : byte
    {
#pragma warning disable 1591
        NoRouteToDestination = 0,
        CommunicationAdministrativelyProhibited = 1,
        BeyondScopeOfSourceAddress = 2,
        AddressUnreachable = 3,
        PortUnreachable = 4,
        SourceAddressFailedPolicy = 5,
        RejectRouteToDestination = 6,
        SourceRoutingHeaderError = 7,
#pragma warning restore 1591
    }
}
