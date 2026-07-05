local WaitUntilTrueTrigger = {}

WaitUntilTrueTrigger.name = "MintChocolateHelper/WaitUntilTrueTrigger"

WaitUntilTrueTrigger.nodeLimits = {1, -1}
WaitUntilTrueTrigger.nodeLineRenderType = "line"

WaitUntilTrueTrigger.placements = {
    name = "wait_until_true_trigger",
    data = {
        width = 16,
        height = 16,
        flag = "",
        delay = 0,
        oneUse = false
    }
}

WaitUntilTrueTrigger.fieldOrder = {
    "x", "y",
    "width", "height",
    "flag", "delay",
    "oneUse"
}

return WaitUntilTrueTrigger