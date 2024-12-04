extends Node

@export var MainController:Node

@export var Play:Button
@export var PlayIcon:Texture2D
@export var PauseIcon:Texture2D
@export var Next:Button
@export var Back:Button
@export var Loop:Button
@export var LoopOn:Texture2D
@export var LoopOff:Texture2D
@export var Progress:Slider
@export var CurrentTime:Label
@export var TotalTime:Label
@export var SongName:Label
@export var SongArtist:Label
@export var SongCover:TextureRect
@export var DefaultCover:Texture2D
@export var VolumeSlider:Slider

@export var EditAttributes:Button
@export var AttributeEditor:Node

var canSetTime = false
var attributesBeingedited = false

func _ready() -> void:
	Loop.button_down.connect(setLoop)
	Play.button_down.connect(setPlay)
	Next.button_down.connect(next)
	Back.button_down.connect(back)
	MainController.OnLoadSong.connect(onLoadSong)
	Progress.drag_ended.connect(setTime)
	Progress.drag_started.connect(onDrag)
	EditAttributes.button_down.connect(editAttributes)
	AttributeEditor.onSubmitdata.connect(submitmeta)
	get_tree().root.min_size = Vector2(850,350)
	AttributeEditor.AttributeWindow.hide()
	setUpDiscord()
	
func editAttributes():
	if !attributesBeingedited:
		AttributeEditor.open(SongName.text, SongArtist.text)
		attributesBeingedited = true
		if MainController.playing:
			setPlay()
	
func setLoop():
	MainController.loop = !MainController.loop
	Loop.icon = LoopOn if MainController.loop else LoopOff
	
func setPlay():
	MainController.Play()
	Play.icon = PlayIcon if !MainController.playing else PauseIcon
	
func next():
	MainController.MoveSong(1)
	Play.icon = PlayIcon if !MainController.playing else PauseIcon

func back():
	MainController.MoveSong(-1)
	Play.icon = PlayIcon if !MainController.playing else PauseIcon
	
func onLoadSong():
	SongName.text = MainController.getName()
	SongArtist.text = MainController.getArtist()
	TotalTime.text = MainController.getSongLength()
	Progress.max_value = MainController.totalTime()
	var cover = MainController.getCover()
	SongCover.texture = cover if cover else DefaultCover
	DiscordRPC.details = "Listening to " + SongName.text
	DiscordRPC.refresh()
	Play.icon = PlayIcon if !MainController.playing else PauseIcon #bug
	
func setTime(val):
	MainController.SetTime(Progress.value)
	canSetTime = false
	
func onDrag():
	canSetTime = true
	
func submitmeta():
	MainController.EditMeta(AttributeEditor.songname, AttributeEditor.artist, AttributeEditor.coverpath)
	onLoadSong()
	attributesBeingedited = false

func _process(delta: float) -> void:
	if MainController.player.stream:
		CurrentTime.text = MainController.getTime()
	
	if !canSetTime:
		Progress.value = MainController.time
	elif !MainController.playing:
		MainController.time = Progress.value
	
	MainController.player.volume_db = VolumeSlider.value if VolumeSlider.value != -50 else -80
	DiscordRPC.state = CurrentTime.text + (" (Paused)" if !MainController.playing else "")
	DiscordRPC.refresh()
	
func setUpDiscord():
	DiscordRPC.app_id = 1313226375007703160
	DiscordRPC.refresh()
