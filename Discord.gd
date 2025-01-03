extends Node

@export var Player:Node
@export var Main:Node

func _ready() -> void:
	DiscordRPC.app_id = 1313226375007703160
	DiscordRPC.large_image = "logotrans"
	DiscordRPC.refresh()
	
func _process(delta: float) -> void:
	DiscordRPC.details = "Listening to " + Player.SongName.text
	DiscordRPC.state = Player.CurrentTime.text + (" (Paused)" if !Main.playing else "")
	DiscordRPC.refresh()
