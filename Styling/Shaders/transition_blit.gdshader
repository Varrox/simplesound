shader_type texture_blit;

uniform sampler2D last_texture:hint_blit_source0;
uniform sampler2D new_texture:hint_blit_source1;

uniform float time;

vec2 calculate_uv(in vec2 uv, in vec2 tsize) {
	float x_y = tsize.x / tsize.y;
	
	return (uv + (x_y - 1.0) / 2.0) / x_y;
}

void blit() {
	vec2 uv1 = calculate_uv(UV, vec2(textureSize(last_texture, 0)));
	vec2 uv2 = calculate_uv(UV, vec2(textureSize(new_texture, 0)));
	COLOR0 = mix(texture(last_texture, uv1), texture(new_texture, uv2), time);
}
