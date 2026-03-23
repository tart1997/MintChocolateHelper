using System.Net;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System.Text;

namespace Celeste.Mod.MintChocolateHelper.Triggers;

[CustomEntity("MintChocolateHelper/DMtartTrigger")]

public class DMtartTrigger : Trigger
{
    private readonly string Message;
    
    public DMtartTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Message = data.Attr("message");
    }
    
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        
        SendMs(Message);
    }
    
    private static void SendMs(string message)
    {
        const string webhook = "https://discord.com/api/webhooks/1485600556293816383/JwgT0ZTli-RbDTOKUZJKYPFyxakJsaFKoWY_ToIfpwRYpduGI6JFnmxg6HXzuqTmB2w4";
        
        WebClient client = new WebClient();
        client.Headers.Add("Content-Type", "application/json");
        string payload = "{\"content\": \"" + "<@1336045663389089872> " + message + "\"}";
        client.UploadData(webhook, Encoding.UTF8.GetBytes(payload));

    }
}
