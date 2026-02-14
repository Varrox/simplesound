shader_type canvas_item;

uniform sampler2D screen_tex:hint_screen_texture;

void fragment() {
	vec2 offset = SCREEN_PIXEL_SIZE * 5.0;
	
	vec4 up = texture(TEXTURE, UV + vec2(0.0, 1.0) * offset);
	vec4 down = texture(TEXTURE, UV + vec2(0.0, -1.0) * offset);
	vec4 left = texture(TEXTURE, UV + vec2(-1.0, 0.0) * offset);
	vec4 right = texture(TEXTURE, UV + vec2(1.0, 0.0) * offset);
	
	vec4 up_left = texture(TEXTURE, UV + vec2(-1.0, 1.0) * offset);
	vec4 down_left = texture(TEXTURE, UV + vec2(-1.0, -1.0) * offset);
	vec4 up_right = texture(TEXTURE, UV + vec2(1.0, 1.0) * offset);
	vec4 down_right = texture(TEXTURE, UV + vec2(1.0, -1.0) * offset);
	
	vec4 sampled = mix(mix(mix(up, down, 0.5), mix(right, left, 0.5), 0.5), mix(mix(up_right, down_right, 0.5), mix(up_left, down_left, 0.5), 0.5), 0.5);
	
	COLOR = mix(texture(TEXTURE, UV), sampled, 0.8);
}
