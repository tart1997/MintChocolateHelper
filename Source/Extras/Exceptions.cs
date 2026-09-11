// TODO: Done Here :)
namespace Celeste.Mod.MintChocolateHelper.Extras;

[Serializable]
public class ImpossibleEnumException : Exception
{
    public ImpossibleEnumException(Exception inner = null) : base("Impossible Enum Value! How did you do that?", inner)
    {
    }

    public ImpossibleEnumException(string message, Exception inner = null) : base($"Impossible Enum Value!: {message}", inner)
    {
    }
}

[Serializable]
public class HookException : Exception
{
    public HookException(string message, Exception inner = null) : base($"Hook application failed: {message}", inner)
    {
    }

    public HookException(ILContext il, string message, Exception inner = null) : base($"IL hook application on method {il.Method.FullName} failed: {message}", inner)
    {
    }
}