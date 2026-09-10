using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace Celeste.Mod.MintChocolateHelper.MintUtils;

public static class HookUtils
{
    [UsedImplicitly]
    private static VariableDefinition AddVariable(this MethodBody self, TypeReference type)
    {
        VariableDefinition variable = new(type);
        self.Variables.Add(variable);
        return variable;
    }
}