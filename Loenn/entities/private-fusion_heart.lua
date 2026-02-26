local drawableSprite = require "structs.drawable_sprite"

local FusionHeart = {}

FusionHeart.name = "MintChocolateHelper/FusionHeart"

FusionHeart.placements = {
    {
        name = "fusion_heart",
        data = {
            color = "ff4fed",
			bloom = 0.75,
			light = true,
        }
    }
}

FusionHeart.fieldInformation = {
	color = {
        fieldType = "color",
    }
}

function FusionHeart.sprite(room, entity)

	local sprite

	sprite = drawableSprite.fromTexture("objects/MintChocolateHelper/FusionHeart/00", {x = entity.x, y = entity.y})
	
    sprite:setColor(entity.color)
    return sprite
end

return FusionHeart