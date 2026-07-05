namespace Celeste.Mod.MintChocolateHelper.Registry.ThisIsJustStolenFromJa;

//All credits to JaThePlayer of course I would never be evil and steal code without saying anything why would you ever think that?

internal static class Extensions {
    extension(XmlAttributeCollection xml) {
        public T GetEnum<T>(string attr, T def) where T : struct, Enum {
            XmlAttribute xmlAttribute = xml[attr];
            if (xmlAttribute is null)
                return def;

            return Enum.TryParse(xmlAttribute.Value, true, out T value) ? value : def;
        }
    }
}