// TODO: Done Here :)
using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace Celeste.Mod.MintChocolateHelper.MintUtils;

public static class HookUtils
{
    [UsedImplicitly]
    internal static VariableDefinition AddVariable(this MethodBody self, TypeReference type)
    {
        VariableDefinition variable = new(type);
        self.Variables.Add(variable);
        return variable;
    }

    [UsedImplicitly]
    internal static bool LogHookOnFailure(this bool attemptedHook, ILContext il)
    {
        if (!attemptedHook)
        {
            Utils.Log(LogLevel.Error, $"\n\nIL hook application on method {il.Method.FullName} failed: Dumb Fuck!\n\n");
        }

        return !attemptedHook;
    }
}