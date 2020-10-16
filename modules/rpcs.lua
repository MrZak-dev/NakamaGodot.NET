-- Defines remote procedures accessible for clients to call to get information before joining the
-- game world.

local nakama = require("nakama")

-- Returns the first existing match in namaka's match list or creates one if there is none.
local function init_matches(context,payload)
    local matches = nakama.match_list()

    --Check if there is a match . create 2 matches
    if matches[0] == nil then
        nakama.match_create("two_match", {})
    end
    return "Matches has been created"
end

local function two_match(_,_)
    local matches = nakama.match_list()
    return matches[1].match_id
end


-- RPC registered to Nakama
nakama.register_rpc(init_matches, "init_matches")
nakama.register_rpc(two_match, "two_match")
