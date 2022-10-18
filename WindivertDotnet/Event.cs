namespace WindivertDotnet
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public sealed class Event
    {
        private readonly string name;

        private Event(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 转换为文本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.name;
        }

        /// <summary>
        /// 新的网络数据包
        /// </summary> 
        public static Event PACKET { get; } = new("PACKET");

        /// <summary>
        /// 将创建一个新流
        /// </summary> 
        public static Event ESTABLISHED { get; } = new("ESTABLISHED");

        /// <summary>
        /// 旧流将被删除
        /// </summary> 
        public static Event DELETED { get; } = new("DELETED");

        /// <summary>
        /// bind操作
        /// </summary> 
        public static Event BIND { get; } = new("BIND");

        /// <summary>
        /// connect操作
        /// </summary> 
        public static Event CONNECT { get; } = new("CONNECT");

        /// <summary>
        /// listen操作
        /// </summary> 
        public static Event LISTEN { get; } = new("LISTEN");

        /// <summary>
        /// accept操作
        /// </summary> 
        public static Event ACCEPT { get; } = new("ACCEPT");

        /// <summary>
        /// 打开了新的WinDivert实例
        /// </summary> 
        public static Event OPEN { get; } = new("OPEN");

        /// <summary>
        /// 旧的WinDivert实例被关闭或套接字终结点已关闭
        /// </summary> 
        public static Event CLOSE { get; } = new("CLOSE");
    }
}
