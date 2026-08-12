local PulseFlagTrigger = {}

PulseFlagTrigger.name = "MintChocolateHelper/PulseFlagTrigger"

PulseFlagTrigger.placements = {
    name = "pulse_flag_trigger",
    data = {
        width = 16,
        height = 16,
        flag = "",
        frames = 1,
        invert = false,
    }
}

PulseFlagTrigger.fieldInformation = {
    frames = {
        fieldType = "integer",
    }
}

PulseFlagTrigger.fieldOrder = {
    "x", "y",
    "width", "height",
    "flag", "frames",
    "invert"
}

return PulseFlagTrigger