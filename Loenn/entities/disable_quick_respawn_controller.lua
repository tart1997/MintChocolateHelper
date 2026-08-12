local DisableQuickRespawnController = {}

DisableQuickRespawnController.name = "MintChocolateHelper/DisableQuickRespawnController"
DisableQuickRespawnController.justification = {0.5, 0.5}

DisableQuickRespawnController.placements = {
    {
        name = "disable_quick_respawn_controller",
        data = {
            disableFlag = "",
        }
    }
}

DisableQuickRespawnController.fieldOrder = {
    "x",
    "y",
    "disableFlag"
}

DisableQuickRespawnController.texture = "loenn/mintchocolatehelper/DisableQuickRespawnController"

return DisableQuickRespawnController