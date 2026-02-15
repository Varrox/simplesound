extends VSeparator

@export var scroll_container:ScrollContainer

func _process(_delta: float) -> void:
	visible = scroll_container.get_v_scroll_bar().visible
