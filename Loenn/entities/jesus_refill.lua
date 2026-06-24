local JesusRefill = {}

JesusRefill.name = "MintChocolateHelper/JesusRefill"
JesusRefill.justification = {0.5, 0.5}

JesusRefill.placements = {
    {
        name = "jesus_refill",
        data = {
			respawnTime = 2.5,
            oneUse = false,
            unregisterDeathInStats = false
        }
    }
}

JesusRefill.texture = "objects/MintChocolateHelper/Refills/JesusRefill/idle00"

return JesusRefill