local SpeedFlipRefill = {}

SpeedFlipRefill.name = "MintChocolateHelper/SpeedFlipRefill"
SpeedFlipRefill.justification = {0.5, 0.5}

SpeedFlipRefill.placements = {
    {
        name = "speed_flip_refill",
        data = {
			respawnTime = 2.5,
			extraMultiplier = 1.03,
			disableAmbientEffects = false,
			disableCollectEffects = false,
            oneUse = false,
        }
    }
}

SpeedFlipRefill.texture = "objects/MintChocolateHelper/Refills/SpeedFlipRefill/idle00"

return SpeedFlipRefill