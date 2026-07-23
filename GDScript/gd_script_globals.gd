class_name GDScriptGlobals extends Node

@export var _audio_source: AudioStreamPlayer
static var audio_source: AudioStreamPlayer

@export var _globals: Globals
static var globals: Globals

func _ready() -> void:
	audio_source = _audio_source
	globals = _globals
