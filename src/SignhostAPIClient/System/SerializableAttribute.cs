namespace System
{
#if !SERIALIZABLE
	internal sealed class SerializableAttribute
		: Attribute
	{
	}
#endif
}
