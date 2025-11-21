namespace UGL.Util.Extensions;

public static class AttributeExtensions
{
    public static bool HasAttribute<T>(this Type type)
    {
        return Attribute.GetCustomAttribute(type, typeof(T)) != null;
    }
}