extends VSeparator

@export var scroll_container:ScrollContainer

func _ready() -> void:
	add_theme_constant_override("separation", 0)
	add_theme_stylebox_override("separator", StyleBoxEmpty.new())

func _process(_delta: float) -> void:
	visible = scroll_container.get_v_scroll_bar().visible
