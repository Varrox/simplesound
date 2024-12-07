extends Control

@export var Cover:TextureRect
@export var Name:Label
@export var Songs:Label
@export var Register:Button
@export var LightPanel:Panel

var PlaylistIndex:int

func _ready() -> void:
	Register.button_down.connect(setSong)
	
func setSong():
	var MainController = get_tree().current_scene
	if MainController != null && MainController.currentPlaylist != PlaylistIndex:
		MainController.currentSong = 0
		MainController.LoadPlaylist(PlaylistIndex)
		MainController.time = 0
		MainController.playing = true
		MainController.InitSong()
		MainController.playing = false
		MainController.Play()
		Register.flat = true
		LightPanel.show()
		
func clearSelected(index):
	if index != PlaylistIndex:
		Register.flat = false
		LightPanel.hide()
		
func init(playlistname, Coverpath, songcount, index:int):
	var img = Image.new()
	if Coverpath != "":
		if img.load(Coverpath) == OK:
			Cover.texture = ImageTexture.create_from_image(img)
	else:
		Cover.texture = load("res://Icons/DefaultCover.png")
	Name.text = playlistname
	Songs.text = str(songcount) + " songs"
	PlaylistIndex = index
	get_tree().current_scene.OnLoadPlaylist.connect(clearSelected)
