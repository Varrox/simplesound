shader_type texture_blit;

uniform sampler2D past_image:hint_blit_source0;
uniform sampler2D new_image:hint_blit_source1;

uniform float t;

vec2 calculate_uv(vec2 uv, sampler2D tex) {
	vec2 output = uv;
	vec2 tsize = vec2(textureSize(tex, 0));
	
	float x_y = tsize.x / tsize.y;
	output.x += (x_y - 1.0) / 2.0;
	output.x /= x_y;
	
	return output;
}

void blit() {
	vec2 uv1 = calculate_uv(UV, past_image);
	vec2 uv2 = calculate_uv(UV, new_image);
	COLOR0 = mix(texture(past_image, uv1), texture(new_image, uv2), t);
}
