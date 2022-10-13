namespace WindivertDotnet
{
    /// <summary>
    /// 网络信息
    /// </summary>
    public struct WinDivertDataNetwork
    {
        /// <summary>
        /// 数据包到达的网络接口索引
        /// </summary>
        public int IfIdx;

        /// <summary>
        /// 数据包到达的网络子接口索引
        /// </summary>
        public int SubIfIdx;
    }
}
