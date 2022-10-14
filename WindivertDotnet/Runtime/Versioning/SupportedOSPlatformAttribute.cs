#if NETCOREAPP3_1
namespace System.Runtime.Versioning
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public sealed class SupportedOSPlatformAttribute : Attribute
    {
        public string PlatformName { get; }

        public SupportedOSPlatformAttribute(string platformName)
        {
            this.PlatformName = platformName;
        }
    }
}
#endif