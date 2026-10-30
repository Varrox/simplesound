// This shader is based off of this shader: https://www.shadertoy.com/view/4tSyzy
// I have modified it to be sample count independent, so you can get the same visuals with 16 samples as the original with 30 samples.

shader_type texture_blit;
render_mode blend_disabled;

uniform sampler2D blur_texture:hint_blit_source0, filter_linear;

uniform float blur_size = 0.22;
uniform float dim = 0.85;

uniform bool horizontal = false;

const int samples = 5;
global uniform int blur_quality;

void blit() {
	vec3 result = vec3(0.0);
	float accum = 0.0;
	
	float size = blur_size / 2.0;
	float steps = size / float(samples * blur_quality);
	ivec2 texture_size = textureSize(blur_texture, 0);
	vec2 direction = horizontal ? vec2(1.0, 0.0) : vec2(0.0, float(texture_size.x) / float(texture_size.y));
	
	float sigma = blur_size * blur_size;
	float two_sigma_squared = 2.0 * sigma * sigma;
	float gaussian_denominator = sqrt(PI * two_sigma_squared);
	
	for(float x = -size; x < size; x += steps) {
		vec2 uv = UV + direction * x * size;
		if (uv.x >= 0.0 && uv.x <= 1.0) {
			float weight = exp(-(x * x) / two_sigma_squared) / gaussian_denominator;
			result += texture(blur_texture, uv).rgb * weight;
			accum += weight;
		}
	}
	
	vec3 blur = result / accum;
	
	COLOR0 = vec4(blur * (horizontal ? dim : 1.0), 1.0);
}
