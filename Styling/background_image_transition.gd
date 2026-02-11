extends SubViewport

@export var transitioner:ColorRect
@export var target_texture_rect:TextureRect

var transition_value:float
var target_transition_value:float

var target_texture:Texture2D:
	set(value):
		past_target_texture = target_texture
		target_texture = value
		
		transition_value = 0.0
		target_transition_value = 1.0

var past_target_texture:Texture2D

func _process(_delta: float) -> void:
	var shader:ShaderMaterial = transitioner.material as ShaderMaterial
	
	if transition_value != 1.0:
		transition_value = Helper.smooth(transition_value, target_transition_value, 4)
		
		shader.set_shader_parameter("past_image", past_target_texture)
		shader.set_shader_parameter("new_image", target_texture)
		shader.set_shader_parameter("t", transition_value)
		
		target_texture_rect.texture = get_texture()
	
	transitioner.process_mode = Node.PROCESS_MODE_ALWAYS if transition_value != 1.0 else Node.PROCESS_MODE_DISABLED
