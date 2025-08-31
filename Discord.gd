class_name DiscordUpdater extends Node

@export var Player:Node
@export var Main:Node

var dev:bool = false


var details:String
var state:String

signal update
signal update_details
signal update_state

func _ready() -> void:
	DiscordRPC.app_id = 1313226375007703160
	DiscordRPC.large_image = "logotrans"
	DiscordRPC.refresh()
	
	await get_tree().create_timer(0.35).timeout
	
	check_for_dev()

func check_for_dev():
	var username:String = DiscordRPC.get_current_user().get("username")
	
	if username != '':
		dev = username == "varrox"
		
		var splash:String = ("btw I (%s) made this software" % (username) if dev else "simplesound is an open source music player")
		DiscordRPC.large_image_text = splash

func _process(_delta: float) -> void:
	if !DiscordRPC.get_is_discord_working():
		return
	
	var new_details = "Listening to " + Player.SongName.text
	var new_state = Player.CurrentTime.text + (" (Paused)" if !Main.playing else "")
	
	if details != new_details || state != new_state:
		if details != new_details:
			update_details.emit()
			details = new_details
		
		if state != new_state:
			update_state.emit()
			state = new_state
		
		update.emit()
		
		DiscordRPC.details = details
		DiscordRPC.state = state
		
		DiscordRPC.refresh()
