namespace Celeste.Mod.MintChocolateHelper.Extras;

public static class MiscExtensions
{
    //All credits to JaThePlayer for this one of course I would never be evil and steal code without saying anything why would you ever think that?
    public static T GetEnum<T>(this XmlAttributeCollection xml, string attr, T def) where T : struct, Enum
    {
        XmlAttribute xmlAttribute = xml[attr];
        if (xmlAttribute is null) return def;
        return Enum.TryParse(xmlAttribute.Value, true, out T value) ? value : def;
    }
}