namespace Celeste.Mod.MintChocolateHelper.Extras;

public static class Commands
{
    #region Encrypt Webhook Setup
    private static int AlphabetLength;
    private static readonly Dictionary<char, byte> Encoder = new();
    private static readonly Dictionary<byte, char> Decoder = new();

    private static void InitializeAlphabet()
    {
        Encoder.Clear();
        Decoder.Clear();

        byte code = 0;
        Encoder.Add(' ', code);
        Decoder.Add(code++, ' ');
        for (char c = 'a'; c <= 'z'; ++c)
        {
            Encoder.Add(c, code);
            Decoder.Add(code++, c);
        }
        for (char c = 'A'; c <= 'Z'; ++c)
        {
            Encoder.Add(c, code);
            Decoder.Add(code++, c);
        }
        for (char c = '0'; c <= '9'; ++c)
        {
            Encoder.Add(c, code);
            Decoder.Add(code++, c);
        }
        for (char c = '!'; c <= '/'; ++c)
        {
            Encoder.Add(c, code);
            Decoder.Add(code++, c);
        }
        for (char c = ':'; c <= '@'; ++c)
        {
            Encoder.Add(c, code);
            Decoder.Add(code++, c);
        }
        for (char c = '['; c <= '`'; ++c)
        {
            Encoder.Add(c, code);
            Decoder.Add(code++, c);
        }
        for (char c = '{'; c <= '~'; ++c)
        {
            Encoder.Add(c, code);
            Decoder.Add(code++, c);
        }
        AlphabetLength = Encoder.Count;
    }
    #endregion

    [UsedImplicitly]
    [Command("EncryptWebHook", "")]
    internal static void EncryptWebHook(string webhook)
    {
        if (webhook is null)
        {
            Engine.Commands.Log("Emtpy Webhook!");
            return;
        }
        InitializeAlphabet();
        webhook = string.Concat(webhook.Where(c => Encoder.ContainsKey(c)));
        string key = string.Concat(Dialog.Get("APP_OLDLADY_LOCKED").Where(c => Encoder.ContainsKey(c)));
        if (webhook.Length == 0)
        {
            Engine.Commands.Log("Invalid Webhook!");
            return;
        }
        StringBuilder ciphertext = new(webhook.Length);
        int k = 0;
        foreach (char m in webhook)
        {
            ciphertext.Append(Decoder[(byte)((Encoder[m] + Encoder[key[k++]]) % AlphabetLength)]);
            if (k == key.Length) k = 0;
        }
        Engine.Commands.Log("Encrypted webhook copied to clipboard!");
        ClipboardService.SetText(ciphertext.ToString());
    }

    [CanBeNull]
    public static string DecryptWebHook(string webhook)
    {
        InitializeAlphabet();
        webhook = string.Concat(webhook.Where(c => Encoder.ContainsKey(c)));
        string key = string.Concat(Dialog.Get("APP_OLDLADY_LOCKED").Where(c => Encoder.ContainsKey(c)));
        if (webhook.Length == 0)
        {
            Utils.LogInfo("broken webhook");
            return null;
        }
        key = string.Concat(key.Where(c => Encoder.ContainsKey(c)));
        if (key.Length == 0)
        {
            Utils.LogInfo("broken key");
            return null;
        }
        StringBuilder decrypted = new(webhook.Length);
        int k = 0;
        foreach (var c in webhook)
        {
            decrypted.Append(Decoder[(byte)((AlphabetLength + Encoder[c] - Encoder[key[k++]]) % AlphabetLength)]);
            if (k == key.Length) k = 0;
        }
        return decrypted.ToString();
    }
}