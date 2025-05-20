extends Node

@export var Player:Node
@export var Main:Node

var dev:bool = false
var devset:bool = false

func _ready() -> void:
	DiscordRPC.app_id = 1313226375007703160
	DiscordRPC.large_image = "logotrans"
	DiscordRPC.refresh()
	
func _process(_delta: float) -> void:
	if DiscordRPC.get_is_discord_working():
		if !devset:
			var username:String = DiscordRPC.get_current_user().get("username")
			if username != '':
				dev = username == "varrox"
				devset = true
		DiscordRPC.details = "Listening to " + Player.SongName.text
		DiscordRPC.state = Player.CurrentTime.text + (" (Paused)" if !Main.playing else "")
		var splash:String = ("btw I made this software" if dev else "simplesound is an open source music player")
		DiscordRPC.large_image_text = splash
		DiscordRPC.refresh()
