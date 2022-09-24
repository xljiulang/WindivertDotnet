namespace WindivertDotnet
{
    public unsafe struct WinDivertDataFlow
    {
        [NativeTypeName("UINT64")]
        public ulong EndpointId;

        [NativeTypeName("UINT64")]
        public ulong ParentEndpointId;

        [NativeTypeName("UINT32")]
        public uint ProcessId;

        [NativeTypeName("UINT32[4]")]
        public fixed uint LocalAddr[4];

        [NativeTypeName("UINT32[4]")]
        public fixed uint RemoteAddr[4];

        [NativeTypeName("UINT16")]
        public ushort LocalPort;

        [NativeTypeName("UINT16")]
        public ushort RemotePort;

        [NativeTypeName("UINT8")]
        public byte Protocol;
    }
}
