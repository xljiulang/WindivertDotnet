namespace WindivertDotnet
{
    public partial struct WINDIVERT_DATA_REFLECT
    {
        [NativeTypeName("INT64")]
        public long Timestamp;

        [NativeTypeName("UINT32")]
        public uint ProcessId;

        public WINDIVERT_LAYER Layer;

        [NativeTypeName("UINT64")]
        public ulong Flags;

        [NativeTypeName("INT16")]
        public short Priority;
    }
}
