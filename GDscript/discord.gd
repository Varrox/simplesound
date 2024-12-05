extends Node

func _ready() -> void:
	setUpDiscord()
	
func setdetails(song):
	DiscordRPC.details = "Listening to " + song
	DiscordRPC.refresh()

func setstate(time, paused):
	DiscordRPC.state = time + (" (Paused)" if paused else "")
	DiscordRPC.refresh()
	
func setUpDiscord():
	DiscordRPC.app_id = 1313226375007703160
	DiscordRPC.refresh()
