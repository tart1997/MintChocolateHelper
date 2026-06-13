local state = require("loaded_state")

local script = {
    name = "closeStylegroundGroups",
    displayName = "Close Styleground Groups",
    tooltip = "Minimizes any Stylegrounds Groups that exist"
}

function script.prerun(args)
    for _, style in ipairs(state.map.stylesFg) do
        if style._type == "apply" then
            style._collapsed = true
        end
    end
	for _, style in ipairs(state.map.stylesBg) do
        if style._type == "apply" then
            style._collapsed = true
        end
    end
end

return script