namespace Celeste.Mod.MintChocolateHelper.MintUtils;

public static class ReflectionUtils
{
    [UsedImplicitly]
    internal static Type[] MintChocolateHelperTypes => typeof(MintChocolateHelperModule).Assembly.GetTypes();
    
    [UsedImplicitly]
    internal const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
    
    extension(Type type)
    {
        [UsedImplicitly]
        internal bool HasAttribute<T>() where T : Attribute
        {
            return type.GetCustomAttributes<T>().Any();
        }

        [UsedImplicitly]
        internal bool HasAttribute<T>(out T attribute) where T : Attribute
        {
            attribute = type.GetCustomAttributes<T>().FirstOrDefault();
            return attribute is {};
        }

        [CanBeNull]
        [UsedImplicitly]
        internal MethodInfo GetMethodWith<T>() where T : Attribute => type.GetMethods(All).FirstOrDefault(method => method.GetCustomAttributes<T>().Any());
    }

    extension(MethodInfo method)
    {
        [UsedImplicitly]
        internal bool HasAttribute<T>() where T : Attribute
        {
            return method.GetCustomAttributes<T>().Any();
        }

        [UsedImplicitly]
        internal bool HasAttribute<T>(out T attribute) where T : Attribute
        {
            attribute = method.GetCustomAttributes<T>().FirstOrDefault();
            return attribute is {};
        }
        
        [UsedImplicitly]
        internal void SimpleInvoke()
        {
            method.Invoke(null, null);
        }

        [UsedImplicitly]
        internal void SimpleInvokeIf(bool condition)
        {
            if (condition) method.SimpleInvoke();
        }
    }
}