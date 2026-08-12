local StopInteractingTrigger = {}

StopInteractingTrigger.name = "MintChocolateHelper/StopInteractingTrigger"

StopInteractingTrigger.placements = {
    name = "stop_interacting_trigger",
    data = {
        width = 16,
        height = 16,
    }
}

StopInteractingTrigger.fieldOrder = {
    "x", "y",
    "width", "height"
}

return StopInteractingTrigger