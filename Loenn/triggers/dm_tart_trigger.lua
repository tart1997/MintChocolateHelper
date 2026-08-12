local DMtartTrigger = {}

DMtartTrigger.name = "MintChocolateHelper/DMtartTrigger"

DMtartTrigger.placements = {
    name = "dm_tart_trigger",
    data = {
        width = 16,
        height = 16,
        identifier = "",
        message = "",
        dialog = false
    }
}

DMtartTrigger.fieldOrder = {
    "x", "y",
    "width", "height",
    "identifier", "message",
    "dialog"
}

return DMtartTrigger