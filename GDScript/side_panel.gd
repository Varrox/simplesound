extends Panel

@export var open_min_size:int = 303.0
@export var closed_min_size:int = 60

@export var split:HSplitContainer

@export var open_button:Button
@export var closed_button:Button

@export var other_elements:Array[Control]
@export var closed_element:Control

var closed = false

func _ready() -> void:
	open_button.button_up.connect(press)
	closed_button.button_up.connect(press)

func press():
	closed = !closed
	
	custom_minimum_size.x = closed_min_size if closed else open_min_size
	split.dragging_enabled = !closed
	split.collapsed = closed
	closed_element.visible = closed
	
	for i in other_elements:
		i.visible = !closed
