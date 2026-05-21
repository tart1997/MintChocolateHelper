using System;
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
    private readonly bool Txt;
    
    private string LastMessage;
    
    public MessageWebhookTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Webhook = data.Attr("webhook");
        Message = data.Attr("message");
        User = data.Attr("user");
        Txt = data.Bool("dialog");
        
        LastMessage = "";
    }
    
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        
        SendMs(Webhook, Message, User, Txt);
    }
    
    private void SendMs(string webhook, string message, string user, bool txt)
    {
        if (LastMessage != message)
        {
            #pragma warning disable SYSLIB0014
            WebClient client = new WebClient();
            #pragma warning restore SYSLIB0014
            client.Headers.Add("Content-Type", "application/json");

            string payload;
            
            if (user != "")
            {
                if (!txt)
                {
                    payload = "{\"content\": \"" + "<@" + user + "> " + @"\n\n" + message + "\"}";
                }
                else
                {
                    string v = Dialog.Clean(message);
                    v = v.Replace("\n", "\\n");
                    payload = "{\"content\": \"" + "<@" + user + "> " + @"\n\n" + v + "\"}";
                
                }
            }
            else
            {
                if (!txt)
                {
                    payload = "{\"content\": \"" + message + "\"}";
                }
                else
                {
                    string v = Dialog.Clean(message);
                    v = v.Replace("\n", "\\n");
                    payload = "{\"content\": \"" + v + "\"}";
                
                }
            }

            client.UploadDataAsync(new Uri(webhook), Encoding.UTF8.GetBytes(payload));
            
            LastMessage = message;
        }
    }
}
