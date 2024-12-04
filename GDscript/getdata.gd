extends Control

@export var Name:TextEdit
@export var Artist:TextEdit
@export var CoverButton:Button
@export var CoverFileDialog:FileDialog
@export var CoverLabel:Label
@export var SubmitButton:Button
@export var AttributeWindow:Window

signal onSubmitdata()

var songname:String
var artist:String
var coverpath:String

func _ready() -> void:
	SubmitButton.button_down.connect(submit)
	CoverButton.button_down.connect(cover)

func open(currentSong, currentArtist):
	Name.text = currentSong
	Artist.text = currentArtist
	AttributeWindow.show()
	AttributeWindow.visible = true
	CoverLabel.text = ""
	coverpath = ""

func submit():
	songname = Name.text
	artist = Artist.text
	AttributeWindow.visible = false
	AttributeWindow.hide()
	emit_signal("onSubmitdata")
	
func cover():
	CoverFileDialog.popup()
	coverpath = CoverFileDialog.current_file
	CoverLabel.text = coverpath
