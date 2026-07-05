local CancelDeathTrigger = {}

CancelDeathTrigger.name = "MintChocolateHelper/CancelDeathTrigger"

CancelDeathTrigger.placements = {
    name = "cancel_death_trigger",
    data = {
        width = 16,
        height = 16,
        delay = 0,
        unregisterDeathInStats = false
    }
}

CancelDeathTrigger.fieldInformation = {
    delay = {
        fieldType = "integer",
    }
}

return CancelDeathTrigger