local FlagsOnAwake = {}

FlagsOnAwake.name = "MintChocolateHelper/FlagsOnAwake"
FlagsOnAwake.justification = {0.5, 0.5}

FlagsOnAwake.placements = {
    {
        name = "flags_on_awake",
        data = {
            flags = "Flag1",
            value = true,
        }
    }
}

FlagsOnAwake.fieldInformation = {
    flags = {
        fieldType = "list",
        elementOptions = {
            fieldType = "string"
        }
    }
}

FlagsOnAwake.texture = "loenn/mintchocolatehelper/FlagsOnAwake"

return FlagsOnAwake