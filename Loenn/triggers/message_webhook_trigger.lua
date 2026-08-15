local MessageWebhookTrigger = {}

MessageWebhookTrigger.name = "MintChocolateHelper/MessageWebhookTrigger"

MessageWebhookTrigger.placements = {
    name = "message_webhook_trigger",
    data = {
        width = 16,
        height = 16,
        webhook = "",
        message = "",
        user = "",
        dialog = false,
        encrypted = false
    }
}

MessageWebhookTrigger.fieldOrder = {
    "x", "y",
    "width", "height",
    "message", "user",
    "webhook", "dialog",
    "encrypted"
}

return MessageWebhookTrigger