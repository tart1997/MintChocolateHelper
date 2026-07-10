namespace Celeste.Mod.MintChocolateHelper.Triggers;

[CustomEntity("MintChocolateHelper/MessageWebhookTrigger")]
public class MessageWebhookTrigger : Trigger
{
    private readonly string Webhook;
    private readonly string Message;
    private readonly string User;
    private readonly bool Txt;

    public MessageWebhookTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Webhook = data.Attr("webhook");
        Message = data.Attr("message");
        User = data.Attr("user");
        Txt = data.Bool("dialog");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        SendMessage(Webhook, Message, User, Txt);
    }

    private static void SendMessage(string webhook, string message, string user, bool txt)
    {
        string payload;

        if (user != "")
        {
            if (txt)
            {
                string v = Dialog.Clean(message);
                v = v.Replace("\n", "\\n");
                payload = "{\"content\": \"" + "<@" + user + "> " + @"\n\n" + v + "\"}";
            }
            else
            {
                payload = "{\"content\": \"" + "<@" + user + "> " + @"\n\n" + message + "\"}";
            }
        }
        else
        {
            if (txt)
            {
                string v = Dialog.Clean(message);
                v = v.Replace("\n", "\\n");
                payload = "{\"content\": \"" + v + "\"}";
            }
            else
            {
                payload = "{\"content\": \"" + message + "\"}";
            }
        }

        DiscordWebhook discordWebhook = new(webhook);
        _ = discordWebhook.SendMessageAsync(payload);
    }
}