extends Panel

@export var open_min_size:int = 303
@export var closed_min_size:int = 60

var target_min_size:float

@export var split:HSplitContainer

@export var open_button:Button
@export var closed_button:Button

@export var open_element:Control
@export var closed_element:Control

var closed = false

func _ready() -> void:
	open_button.button_up.connect(press)
	closed_button.button_up.connect(press)
	target_min_size = open_min_size

func press():
	closed = !closed

	target_min_size = closed_min_size if closed else open_min_size
	
	closed_element.visible = closed
	open_element.visible = !closed
	
	split.collapsed = true
	
	var tween = get_tree().create_tween()
	
	tween.tween_property(self, "custom_minimum_size:x", target_min_size, 0.1).set_trans(Tween.TRANS_QUAD)
	
	await tween.finished
	
	split.collapsed = closed
	split.split_offset = int(custom_minimum_size.x)
