local CancelDeathTrigger = {}

CancelDeathTrigger.name = "MintChocolateHelper/CancelDeathTrigger"

CancelDeathTrigger.placements = {
    name = "cancel_death_trigger",
    data = {
        width = 16,
        height = 16,
        flag = "",
        delay = 0,
        unregisterDeathInStats = false
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
    "unregisterDeathInStats"
}

return CancelDeathTrigger