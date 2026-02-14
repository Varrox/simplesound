extends Button

@export var link:String

func _ready() -> void:
	button_up.connect(open_link)

func open_link():
	OS.shell_open(link)
