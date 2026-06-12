using System;
using System.Net;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System.Text;
using Celeste.Editor;

namespace Celeste.Mod.MintChocolateHelper.Triggers;

[CustomEntity("MintChocolateHelper/DMtartTrigger")]

public class DMtartTrigger : Trigger
{
    private readonly string Identifier;
    private readonly string Message;
    private readonly bool Txt;
    
    private string LastMessage;
    
    public DMtartTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Identifier = data.Attr("identifier");
        Message = data.Attr("message");
        Txt = data.Bool("dialog");
        
        LastMessage = "";
    }
    
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        
        SendMs(Identifier, Message, Txt);
    }
    
    private void SendMs(string identifier, string message, bool txt)
    {
        if (LastMessage != message && identifier != "")
        {
            const string webhook = "https://discord.com/api/webhooks/1485600556293816383/JwgT0ZTli-RbDTOKUZJKYPFyxakJsaFKoWY_ToIfpwRYpduGI6JFnmxg6HXzuqTmB2w4";
            string filename = SaveData.GetFilename();
            string mapname = MapEditor.area.SID;
            string payload;
        
            #pragma warning disable SYSLIB0014
            WebClient client = new();
            #pragma warning restore SYSLIB0014
            client.Headers.Add("Content-Type", "application/json");

            if (!txt)
            {
                payload = "{\"content\": \"" + "<@1336045663389089872> " + "\\nFrom: " + identifier + "\\nFilename: " + filename + "\\nSID: " + mapname + @"\n\n" + message + "\"}";
                Logger.Info("debug", payload);
            }
            else
            {
                string v = Dialog.Clean(message);
                v = v.Replace("\n", "\\n");
                payload = "{\"content\": \"" + "<@1336045663389089872> " + "\\nFrom: " + identifier + "\\nFilename: " + filename + "\\nSID: " + mapname + @"\n\n" + v + "\"}";
                
            }
            
            client.UploadDataAsync(new Uri(webhook), Encoding.UTF8.GetBytes(payload));
        
            LastMessage = message;
        }
    }
}
