using System;

namespace WindivertDotnet
{
    sealed class NativeTypeNameAttribute : Attribute
    {
        public string TypeName { get; }

        public NativeTypeNameAttribute(string typeName)
        {
            this.TypeName = typeName;
        }
    }
}
