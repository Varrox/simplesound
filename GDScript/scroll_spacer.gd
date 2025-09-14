extends VSeparator

@export var scroll_container:ScrollContainer

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	visible = scroll_container.get_v_scroll_bar().visible
