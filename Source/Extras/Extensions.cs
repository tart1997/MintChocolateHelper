namespace Celeste.Mod.MintChocolateHelper.Extras;

public static class Extensions
{
    //All credits to JaThePlayer for this one of course I would never be evil and steal code without saying anything why would you ever think that?
    extension(XmlAttributeCollection xml)
    {
        public T GetEnum<T>(string attr, T def) where T : struct, Enum
        {
            XmlAttribute xmlAttribute = xml[attr];
            if (xmlAttribute is null)
                return def;

            return Enum.TryParse(xmlAttribute.Value, true, out T value) ? value : def;
        }
    }
}