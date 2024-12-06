extends Node

func _ready() -> void:
	DiscordRPC.app_id = 1313226375007703160
	DiscordRPC.refresh()
	
func setdetails(song):
	DiscordRPC.details = "Listening to " + song
	DiscordRPC.refresh()

func setstate(time, paused):
	DiscordRPC.state = time + (" (Paused)" if paused else "")
	DiscordRPC.refresh()
