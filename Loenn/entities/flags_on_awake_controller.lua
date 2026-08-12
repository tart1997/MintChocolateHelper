local FlagsOnAwakeController = {}

FlagsOnAwakeController.name = "MintChocolateHelper/FlagsOnAwakeController"
FlagsOnAwakeController.justification = {0.5, 0.5}

FlagsOnAwakeController.placements = {
    {
        name = "flags_on_awake_controller",
        data = {
            flags = "Flag1",
            value = true,
        }
    }
}

FlagsOnAwakeController.fieldInformation = {
    flags = {
        fieldType = "list",
        elementOptions = {
            fieldType = "string"
        }
    }
}

FlagsOnAwakeController.fieldOrder = {
    "x",
    "y",
    "flags",
    "value"
}

FlagsOnAwakeController.texture = "loenn/mintchocolatehelper/FlagsOnAwakeController"

return FlagsOnAwakeController