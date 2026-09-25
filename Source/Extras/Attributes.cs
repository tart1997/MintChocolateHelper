namespace Celeste.Mod.MintChocolateHelper.Extras;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal class ConditionalEntityAttribute : Attribute
{
    internal readonly string[] Dependencies;
    internal ConditionalEntityAttribute(params string[] dependencies)
    {
        Dependencies = dependencies;
    }
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal class CustomCommandAttribute : Attribute;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal class ConditionalOnLoadAttribute : Attribute;