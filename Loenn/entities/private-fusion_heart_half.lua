local drawableSprite = require "structs.drawable_sprite"

local FusionHeartHalf = {}

FusionHeartHalf.name = "MintChocolateHelper/FusionHeartHalf"

FusionHeartHalf.placements = {
    {
        name = "fusion_heart_half",
        data = {
            color = "9a9ddb",
            bloom = 0.75,
            collisionSpeedX = 1,
            collisionSpeedY = 1,
            frictionX = 1,
            frictionY = 1,
            light = true,
            rightHalf = false,
        }
    }
}

FusionHeartHalf.fieldOrder = {
    "x",
    "y",
    "bloom",
    "color",
    "collisionSpeedX",
    "collisionSpeedY",
    "frictionX",
    "frictionY",
    "light",
    "rightHalf"
}

FusionHeartHalf.fieldInformation = {
    color = {
         fieldType = "color",
    }
}

function FusionHeartHalf.sprite(room, entity)

    local sprite

    if entity.rightHalf then
        sprite = drawableSprite.fromTexture("objects/MintChocolateHelper/FusionHeartHalf/RightHalf/idle00", {x = entity.x, y = entity.y})
    else
        sprite = drawableSprite.fromTexture("objects/MintChocolateHelper/FusionHeartHalf/LeftHalf/idle00", {x = entity.x, y = entity.y})
    end

    sprite:setColor(entity.color)
    return sprite
end

function FusionHeartHalf.flip(room, entity, horizontal, vertical)
    if horizontal then
        entity.rightHalf = not entity.rightHalf
    end

    return horizontal
end

return FusionHeartHalf