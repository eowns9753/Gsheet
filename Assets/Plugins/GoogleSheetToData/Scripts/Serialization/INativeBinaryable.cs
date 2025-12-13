namespace Rui.IO.Serialization
{
    public interface INativeBinaryable
    {
        void NativeWrite(NativeBinaryWriter writer);
        void NativeRead(NativeBinaryReader reader);
    }

    public interface INativeBinaryWithStringAccess
    {
        void NativeWrite(NativeBinaryWriter writer,StringAccessSerializer serializer);
        void NativeRead(NativeBinaryReader reader,StringAccessSerializer serializer);
    }
}