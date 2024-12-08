extends Control

@export var Cover:TextureRect
@export var Name:Label
@export var Songs:Label
@export var Register:Button
@export var SelectedColor:Color
@export var More:Button

var PlaylistIndex:int

func _ready() -> void:
	Register.button_down.connect(setSong)
	Register.mouse_entered.connect(onEnter)
	Register.mouse_exited.connect(onExit)
	More.mouse_entered.connect(onEnter)
	
func onEnter():
	More.show()
	
func onExit():
	More.hide()
	
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
		Register.self_modulate = SelectedColor
		
func clearSelected(index):
	if index != PlaylistIndex:
		Register.self_modulate = Color(1, 1, 1, 1)
		
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
