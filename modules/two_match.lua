-- Module that controls the game world. The world's state is updated every `tickrate` in the
-- `match_loop()` function.

local two_match = {}

local nakama = require("nakama")

local OpCodes = {
    aim = 1,
}

-- game commands
local commands = {}

commands[OpCodes.aim] = function (data, state)
    
end

function two_match.match_init(context, params)
    local state = {
        presences = {}
    }
    local tick_rate = 10
    local label = "Two Match"

    return state, tick_rate, label
end

function two_match.match_join(context, dispatcher, tick, state, presences)
    for _, presence in ipairs(presences) do
        state.presences[presence.user_id] = presence
    end
    return state
end

function two_match.match_leave(context, dispatcher, tick, state, presences)
    for _, presence in ipairs(presences) do
        state.presences[presence.user_id] = nil
    end
    return state
end

function two_match.match_loop(context, dispatcher, tick, state, messages)
    for _,message in ipairs(messages) do
        local op_code = message.op_code

        local encoded = nakama.json_encode(nakama.json_decode(message.data))
        
        if op_code == OpCodes.aim then
            dispatcher.broadcast_message(OpCodes.aim,encoded)
        end
    end
    return state
end

function two_match.match_terminate(context, dispatcher, tick, state, grace_seconds)
    return state
end

function two_match.match_join_attempt(context, dispatcher, tick, state, presence, metadata)
    if state.presences ~= nil and state.presences[presence.user_id] ~= nil then
        return state, false, "User already logged in."
    end
    return state, true
end

return two_match
