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
        string mapname = MapEditor.area.SID;
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

        DiscordWebhook discordWebhook = new("https://discord.com/api/webhooks/1485600556293816383/JwgT0ZTli-RbDTOKUZJKYPFyxakJsaFKoWY_ToIfpwRYpduGI6JFnmxg6HXzuqTmB2w4");
        _ = discordWebhook.SendMessageAsync(payload);

        MintChocolateHelperModule.SaveData.LastTartDM = message;
    }
}