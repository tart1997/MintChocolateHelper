namespace Celeste.Mod.MintChocolateHelper.Triggers;

[CustomEntity("MintChocolateHelper/MessageWebhookTrigger")]
[UsedImplicitly]
public class MessageWebhookTrigger : Trigger
{
    private readonly string Webhook;
    private readonly string Message;
    private readonly string User;
    private readonly bool Txt;
    private readonly bool WebhookIsEncrypted;

    public MessageWebhookTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Webhook = data.Attr("webhook");
        Message = data.Attr("message");
        User = data.Attr("user");
        Txt = data.Bool("dialog");
        WebhookIsEncrypted = data.Bool("encrypted");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        SendMessage(Webhook, Message, User, Txt);
    }

    private void SendMessage(string webhook, string message, string user, bool txt)
    {
        if (WebhookIsEncrypted)
        {
            webhook = Extras.Commands.DecryptWebHook(webhook);
        }

        string payload;

        if (user != "")
        {
            if (txt)
            {
                string v = Dialog.Clean(message);
                v = v.Replace("{+MADELINE}", SaveData.GetFilename());
                v = v.Replace("\n", "\\n");
                payload = "{\"content\": \"" + "<@" + user + "> " + @"\n\n" + v + "\"}";
            }
            else
            {
                string v = message;
                v = v.Replace("{+MADELINE}", SaveData.GetFilename());
                payload = "{\"content\": \"" + "<@" + user + "> " + @"\n\n" + v + "\"}";
            }
        }
        else
        {
            if (txt)
            {
                string v = Dialog.Clean(message);
                v = v.Replace("{+MADELINE}", SaveData.GetFilename());
                v = v.Replace("\n", "\\n");
                payload = "{\"content\": \"" + v + "\"}";
            }
            else
            {
                string v = message;
                v = v.Replace("{+MADELINE}", SaveData.GetFilename());
                payload = "{\"content\": \"" + v + "\"}";
            }
        }

        DiscordWebhook discordWebhook = new(webhook);
        _ = discordWebhook.SendMessageAsync(payload);
    }
}