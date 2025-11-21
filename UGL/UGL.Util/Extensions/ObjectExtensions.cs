using System.Linq.Expressions;
using System.Reflection;

namespace UGL.Util.Extensions;

public static class ObjectExtensions
{
    public static Action CreateAction(this object instance, MethodInfo method)
    {
        var instanceExpr = Expression.Constant(instance);
        var call = Expression.Call(instanceExpr, method);
        return Expression.Lambda<Action>(call).Compile();
    }
}