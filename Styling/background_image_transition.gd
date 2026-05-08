extends SubViewport

@export var transitioner:ColorRect
@export var target_texture_rect:TextureRect

var transition_value:float

var target_texture:Texture2D:
	set(value):
		past_target_texture = target_texture
		target_texture = value
		
		var shader:ShaderMaterial = transitioner.material as ShaderMaterial
		
		shader.set_shader_parameter("past_image", past_target_texture)
		shader.set_shader_parameter("new_image", target_texture)
		
		transition_value = 0.0 # Reset

var past_target_texture:Texture2D

func _ready() -> void:
	target_texture_rect.texture = get_texture()

func _process(_delta: float) -> void:
	if transition_value != 1.0:
		transition_value = Helper.smooth(transition_value, 1.0, 4)
		
		(transitioner.material as ShaderMaterial).set_shader_parameter("t", transition_value)
	
	transitioner.process_mode = Node.PROCESS_MODE_ALWAYS if transition_value != 1.0 else Node.PROCESS_MODE_DISABLED
