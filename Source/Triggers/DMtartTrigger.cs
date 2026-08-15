namespace Celeste.Mod.MintChocolateHelper.Triggers;

[CustomEntity("MintChocolateHelper/DMtartTrigger")]
[UsedImplicitly]
public class DMtartTrigger : Trigger
{
    private readonly string Identifier;
    private readonly string Message;
    private readonly bool Txt;

    public DMtartTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Identifier = data.Attr("identifier");
        Message = data.Attr("message");
        Txt = data.Bool("dialog");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        SendMessage(Identifier, Message, Txt);
    }

    private static void SendMessage(string identifier, string message, bool txt)
    {
        if (MintChocolateHelperModule.SaveData.LastTartDM == message || identifier == "") return;

        string filename = SaveData.GetFilename();
        string mapname = SaveData.Instance.CurrentSession.Area.SID;
        string payload;

        if (txt)
        {
            string v = Dialog.Clean(message);
            v = v.Replace("\n", "\\n");
            payload = "{\"content\": \"" + "<@1336045663389089872> " + "\\nFrom: " + identifier + "\\nFilename: " + filename + "\\nSID: " + mapname + @"\n\n" + v + "\"}";
        }
        else
        {
            payload = "{\"content\": \"" + "<@1336045663389089872> " + "\\nFrom: " + identifier + "\\nFilename: " + filename + "\\nSID: " + mapname + @"\n\n" + message + "\"}";
        }

        DiscordWebhook discordWebhook = new(Extras.Commands.DecryptWebHook("dJIHMa:\\xi5DSWPpQTm_fvC/Pfff8wlM/5+);0?)11!68$7</.E,/X.MhTNWoB\"4yc/HgB9l'HyI%-IGw5fDWLssW/k!CrFO1)wL!-51%Fy6UC`-p45EyV<yJ"));
        _ = discordWebhook.SendMessageAsync(payload);

        MintChocolateHelperModule.SaveData.LastTartDM = message;
    }
}