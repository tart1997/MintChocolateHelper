local JesusRefill = {}

JesusRefill.name = "MintChocolateHelper/JesusRefill"
JesusRefill.justification = {0.5, 0.5}

JesusRefill.placements = {
    {
        name = "jesus_refill",
		alternativeName = {"unkill_refill", "revive_refill"},
        data = {
            respawnTime = 2.5,
            oneUse = false,
            disableQuickRespawn = false,
			skipEverestEventDie = false,
			affectRetries = false,
            dontRegisterDeathInStats = false,
		    keepFollowers = false,
            teleportToRefill = false,
			storeSpeed = false,
            redirectable = false
        }
    }
}

JesusRefill.fieldOrder = {
    "x",
    "y",
    "respawnTime",
    "disableQuickRespawn",
	"skipEverestEventDie",
	"affectRetries",
    "dontRegisterDeathInStats",
	"keepFollowers",
	"teleportToRefill",
	"storeSpeed",
	"redirectable",
    "oneUse"
}

JesusRefill.texture = "objects/MintChocolateHelper/Refills/JesusRefill/idle00"

return JesusRefill