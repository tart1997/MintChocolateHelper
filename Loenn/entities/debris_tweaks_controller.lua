local DebrisTweaksController = {}

DebrisTweaksController.name = "MintChocolateHelper/DebrisTweaksController"
DebrisTweaksController.justification = {0.5, 0.5}

DebrisTweaksController.placements = {
    {
        name = "debris_tweaks_controller",
        data = {
            alternateFadeout = false,
            windAffected = false
        }
    }
}

DebrisTweaksController.fieldOrder = {
    "x",
    "y",
    "alternateFadeout",
    "windAffected"
}

DebrisTweaksController.texture = "loenn/mintchocolatehelper/DebrisTweaksController"

return DebrisTweaksController