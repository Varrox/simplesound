extends Node

@export var zoom_display:Label

var tweening:bool
var tween:Tween
var last_time:float

func _ready() -> void:
	zoom_display.hide()

func _process(delta: float) -> void:
	var zoom = get_tree().root.content_scale_factor
	
	if Input.is_action_just_pressed("scale_up"):
		zoom += 0.1
	if Input.is_action_just_pressed("scale_down"):
		zoom -= 0.1
	
	zoom = clamp(zoom, 0.5, 1.5)
	
	last_time -= delta
	
	if zoom != get_tree().root.content_scale_factor:
		get_tree().root.content_scale_factor = zoom
		
		zoom_display.text = str(round(100 * zoom), '%')
		
		if last_time < 0.0:
			zoom_display.show()
			zoom_display.self_modulate.a = 0.0
			
			if tween:
				tween.stop()
			
			tween = get_tree().create_tween()
			tween.finished.connect(disable_zoom_display)
			tween.tween_property(zoom_display, "self_modulate", Color(1.0, 1.0, 1.0, 1.0), 0.3)
		
		last_time = 0.5 # Reset

func disable_zoom_display():
	while last_time > 0:
		await get_tree().create_timer(last_time).timeout
	
	tween = get_tree().create_tween()
	tween.finished.connect(func(): zoom_display.hide())
	tween.tween_property(zoom_display, "self_modulate", Color(1.0, 1.0, 1.0, 0.0), 1.0)
