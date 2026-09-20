local CancelDeathTrigger = {}

CancelDeathTrigger.name = "MintChocolateHelper/CancelDeathTrigger"

CancelDeathTrigger.placements = {
    name = "cancel_death_trigger",
	alternativeName = {"unkill_player_trigger", "revive_player_trigger"},
    data = {
        width = 16,
        height = 16,
        flag = "",
        delay = 0,
		disableQuickRespawn = false,
		skipEverestEventDie = false,
		affectRetries = false,
        dontRegisterDeathInStats = false,
		keepFollowers = false
    }
}

CancelDeathTrigger.fieldInformation = {
    delay = {
        fieldType = "integer",
    }
}

CancelDeathTrigger.fieldOrder = {
    "x", "y",
    "width", "height",
    "flag", "delay",
    "disableQuickRespawn", "skipEverestEventDie",
	"affectRetries", "dontRegisterDeathInStats",
	"keepFollowers"
}

return CancelDeathTrigger