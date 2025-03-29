using System.Linq.Expressions;
using System.Reflection;

namespace KoiChocoSteamSecondarySubtitle;

internal static class FastMemberAccessor<TClass, TValue> where TClass : class
{
    private static readonly Dictionary<string, Func<TClass, TValue>> _getters = new();
    private static readonly Dictionary<string, Action<TClass, TValue>> _setters = new();

    private const BindingFlags BindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

    public static TValue Get(TClass instance, string memberName)
    {
        if (!_getters.TryGetValue(memberName, out var getter))
        {
            var member = FindMember(memberName, out var isStatic);
            if (member == null)
                throw new Exception($"Couldn't find member {memberName} for type {typeof(TClass).Name}");

            var instanceParameterExpression = Expression.Parameter(typeof(TClass), "instance");
            var memberExpression = Expression.MakeMemberAccess(isStatic ? null : instanceParameterExpression, member);
            var lambda = Expression.Lambda<Func<TClass, TValue>>(memberExpression, instanceParameterExpression);
            getter = lambda.Compile();
            _getters.Add(memberName, getter);
        }

        return getter(instance);
    }

    public static void Set(TClass instance, string memberName, TValue value)
    {
        if (!_setters.TryGetValue(memberName, out var setter))
        {
            var member = FindMember(memberName, out var isStatic);
            if (member == null)
                throw new Exception($"Couldn't find member {memberName} for type {typeof(TClass).Name}");
            if (member is PropertyInfo { CanRead: true })
                throw new Exception($"Property {memberName} in type {typeof(TClass).Name} have no setter");

            var instanceParameterExpression = Expression.Parameter(typeof(TClass), "instance");
            var valueParameterExpression = Expression.Parameter(typeof(TValue), "value");
            var memberExpression = Expression.MakeMemberAccess(isStatic ? null : instanceParameterExpression, member);
            var assignExpression = Expression.Assign(memberExpression, valueParameterExpression);
            var lambda = Expression.Lambda<Action<TClass, TValue>>(assignExpression, [instanceParameterExpression, valueParameterExpression]);
            setter = lambda.Compile();
            _setters.Add(memberName, setter);
        }

        setter(instance, value);
    }

    private static MemberInfo FindMember(string memberName, out bool isStatic)
    {
        if (typeof(TClass).GetProperty(memberName, BindingAttr) is { } property)
        {
            isStatic = property.GetGetMethod().IsStatic;
            return property;
        }

        if (typeof(TClass).GetField(memberName, BindingAttr) is { } field)
        {
            isStatic = field.IsStatic;
            return field;
        }

        isStatic = false;
        return null;
    }
}
