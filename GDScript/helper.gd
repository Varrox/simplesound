class_name Helper
## A class of random helpful functions that which half of should be inside of godot already

static func disable_node(node:Node):
	if node:
		node.process_mode = Node.PROCESS_MODE_DISABLED
		node.visible = false
	
static func enable_node(node:Node):
	if node:
		node.process_mode = Node.PROCESS_MODE_ALWAYS
		node.visible = true

static func get_smooth_value(s:float, delta:float) -> float:
	return clamp(1 - pow(0.5, s * delta), 0, 1)
	
static func smooth(a, b, s): ## lerp smoothing, but not fps dependent
	return lerp(a, b, get_smooth_value(s, get_process_delta_time()))
	
static func fsmooth(a, b, s): ## smooth function, but for physics process
	return lerp(a, b, get_smooth_value(s, get_physics_process_delta_time()))

static func get_physics_process_delta_time() -> float:
	return Engine.get_main_loop().root.get_physics_process_delta_time()

static func get_process_delta_time() -> float:
	return Engine.get_main_loop().root.get_process_delta_time()

static func get_user() -> String:
	return OS.get_environment("USERNAME") if OS.get_name() == "Windows" else OS.get_environment("USER")

static func get_random_string(length:int):
	var output:String = ''
	
	for i in length:
		output += char(randi_range(33, 126))
	
	return output

static func get_random_vector2() -> Vector2:
	var rotation:float = randf_range(0.0, 2.0 * PI)
	return Vector2(cos(rotation), sin(rotation))

static func did_hell_break_loose() -> bool:
	return true == false
