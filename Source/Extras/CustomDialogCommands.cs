namespace Celeste.Mod.MintChocolateHelper.Extras;

internal class McTrigger : FancyText.Trigger
{
    internal delegate void CDCAction(DynamicData data, params string[] args);

    internal CDCAction ParseCommandAction;
    internal CDCAction ParseNewPageAction;
    internal CDCAction WhileOnPageAction;
    internal CDCAction OnReadAction;

    public string[] Params;
    internal McTrigger(string[] rawParams)
    {
        Silent = true;
        Params = rawParams;
    }
}

public static class CustomDialogCommands
{
    private static readonly Dictionary<string, McTrigger> Triggers = [];

    extension((string, McTrigger)[] triggers)
    {
        internal void Register()
        {
            foreach ((string commandString, McTrigger trigger) in triggers)
            {
                Triggers.Add(commandString, trigger);
            }
        }

        internal void Unregister()
        {
            foreach ((string commandString, McTrigger _) in triggers)
            {
                Triggers.Remove(commandString);
            }
        }
    }

    private static ILHook DoOnReadAction;

    [UsedImplicitly]
    internal static void Load()
    {
        Utils.LogDebug($"Loading {nameof(CustomDialogCommands)} Hooks...");
        IL.Celeste.FancyText.Parse += ParseCommands;
        On.Celeste.Textbox.Render += TextboxOnRender;
        DoOnReadAction ??= new ILHook(typeof(Textbox).GetMethod(nameof(Textbox.RunRoutine), BindingFlags.NonPublic | BindingFlags.Instance)!.GetStateMachineTarget()!, TextboxOnRunRoutine);
    }

    [OnUnload]
    internal static void Unload()
    {
        IL.Celeste.FancyText.Parse -= ParseCommands;
        On.Celeste.Textbox.Render -= TextboxOnRender;
        DoOnReadAction?.Dispose();
        DoOnReadAction = null;
    }

    private static void ParseCommands(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_0231: ldarg.0
        IL_0232: ldfld class Celeste.FancyText/Text Celeste.FancyText::group
        IL_0237: ldfld class [mscorlib]System.Collections.Generic.List`1<class Celeste.FancyText/Node> Celeste.FancyText/Text::Nodes
        IL_023c: newobj instance void Celeste.FancyText/NewPage::.ctor()
        IL_0241: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class Celeste.FancyText/Node>::Add(!0)
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdfld<FancyText>("group"),
                static instr => instr.MatchLdfld<FancyText.Text>("Nodes"),
                static instr => instr.MatchNewobj<FancyText.NewPage>(".ctor"),
                static instr => instr.MatchCallvirt<List<FancyText.Node>>("Add")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitLdarg0();
        cursor.EmitLdloc(il.Method.Body.Variables[7]);
        cursor.EmitLdloc(il.Method.Body.Variables[8]);
        cursor.EmitDelegate(ParseNewPage);


        /*
        IL_02bc: ldstr "/>>"
        IL_02c1: callvirt instance bool [mscorlib]System.String::Equals(string)
        */

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdstr("/>>"),
                static instr => instr.MatchCallvirt<string>("Equals")).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitLdarg0();
        cursor.EmitLdloc(il.Method.Body.Variables[7]);
        cursor.EmitLdloc(il.Method.Body.Variables[8]);
        cursor.EmitDelegate(AddTriggers);
    }

    private static void AddTriggers(FancyText text, string commandString, string[] parameters)
    {
        DynamicData textData = DynamicData.For(text);
        FancyText.Text? group = textData.Get<FancyText.Text?>("group");
        if (group is null) return;

        foreach (string registeredCommandString in Triggers.Keys.Where(registeredCommandString => registeredCommandString == commandString))
        {
            McTrigger trigger = Triggers[registeredCommandString];

            trigger.Params = parameters;
            group.Nodes.Add(trigger);
            trigger.ParseCommandAction?.Invoke(textData, parameters);
        }
    }

    private static void ParseNewPage(FancyText text, string commandString, string[] parameters)
    {
        foreach (string registeredCommandString in Triggers.Keys.Where(registeredCommandString => registeredCommandString == commandString))
        {
            Triggers[registeredCommandString].ParseNewPageAction?.Invoke(DynamicData.For(text), parameters);
        }
    }

    private static void TextboxOnRender(On.Celeste.Textbox.orig_Render orig, Textbox self)
    {
        FancyText.Text text = self.text;
        List<McTrigger> knownTriggers = [];

        for (int i = self.Start; i < text.Nodes.Count; i++)
        {
            if (text.Nodes[i] is McTrigger trigger)
            {
                if (knownTriggers.Any(t => t == trigger)) continue;
                knownTriggers.Add(trigger);
            }
            else if (text.Nodes[i] is FancyText.NewPage)
            {
                break;
            }
        }

        foreach (McTrigger trigger in knownTriggers)
        {
            trigger.WhileOnPageAction?.Invoke(DynamicData.For(text), trigger.Params);
        }

        orig(self);
    }

    private static void TextboxOnRunRoutine(ILContext il)
    {
        ILCursor cursor = new(il);


        /*
        IL_008b: ldarg.0
        IL_008c: ldc.r4 0.0
        IL_0091: stfld float32 Celeste.Textbox/'<RunRoutine>d__67'::'<delay>5__5'
        */

        if (cursor.TryGotoNextBestFit(MoveType.After,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdcR4(0.0f),
                static instr => instr.MatchStfld(out _)).LogHookOnFailure(il))
        {
            return;
        }

        if (cursor.TryGotoNextBestFit(MoveType.Before,
                static instr => instr.MatchLdarg0(),
                static instr => instr.MatchLdcR4(0.0f),
                static instr => instr.MatchStfld(out _)).LogHookOnFailure(il))
        {
            return;
        }

        cursor.EmitLdloc1();
        cursor.EmitDelegate(CheckForOnReadAction);
    }

    private static void CheckForOnReadAction(Textbox self)
    {
        foreach (McTrigger trigger in Triggers.Values.Where(t => t == self.Nodes[self.index]))
        {
            trigger.OnReadAction?.Invoke(DynamicData.For(self.text), trigger.Params);
        }
    }
}