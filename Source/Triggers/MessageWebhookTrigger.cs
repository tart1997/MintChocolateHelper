using System.Net;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System.Text;

namespace Celeste.Mod.MintChocolateHelper.Triggers;

[CustomEntity("MintChocolateHelper/MessageWebhookTrigger")]

public class MessageWebhookTrigger : Trigger
{
    private readonly string Webhook;
    private readonly string Message;
    private readonly string User;
    
    public MessageWebhookTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Webhook = data.Attr("webhook");
        Message = data.Attr("message");
        User = data.Attr("user");
    }
    
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        
        SendMs(Webhook, Message, User);
    }
    
    private static void SendMs(string webhook, string message, string user)
    {
        WebClient client = new WebClient();
        client.Headers.Add("Content-Type", "application/json");

        string payload;
        
        if (user != "")
        {
            payload = "{\"content\": \"" + "<@" + user + "> " + message + "\"}";
        }
        else
        {
            payload = "{\"content\": \"" + message + "\"}";
        }

        client.UploadData(webhook, Encoding.UTF8.GetBytes(payload));
    }
}
