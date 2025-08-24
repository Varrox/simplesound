extends SubViewport

@export var transitioner:ColorRect
@export var target_texture_rect:TextureRect

var transition_value:float

var target_texture:Texture2D:
	set(value):
		past_target_texture = target_texture
		target_texture = value
		
		transition_value = 0.0
		
		var mix_tween = get_tree().create_tween()
		mix_tween.set_trans(Tween.TRANS_SINE)
		mix_tween.tween_property(self, "transition_value", 1.0, 0.7)

var past_target_texture:Texture2D

func _process(delta: float) -> void:
	var shader:ShaderMaterial = transitioner.material as ShaderMaterial
	
	shader.set_shader_parameter("past_image", past_target_texture)
	shader.set_shader_parameter("new_image", target_texture)
	shader.set_shader_parameter("t", transition_value)
	
	target_texture_rect.texture = get_texture()
	
