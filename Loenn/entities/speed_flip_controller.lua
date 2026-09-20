local SpeedFlipController = {}

SpeedFlipController.name = "MintChocolateHelper/SpeedFlipController"
SpeedFlipController.justification = {0.5, 0.5}

SpeedFlipController.placements = {
    {
        name = "speed_flip_controller",
		alternativeName = {"sonafleki_controller", "ozone_controller"},
        data = {
		    extraMultiplier = 1.03
        }
    }
}

SpeedFlipController.fieldOrder = {
    "x",
    "y",
    "extraMultiplier"
}

SpeedFlipController.texture = "loenn/mintchocolatehelper/SpeedFlipController"

return SpeedFlipController