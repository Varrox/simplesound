// This shader is based off of this shader: https://www.shadertoy.com/view/4tSyzy
// I have modified it to be sample count independent, so you can get the same visuals with 16 samples as the original with 30 samples.

shader_type texture_blit;

uniform sampler2D blur_texture:hint_blit_source0;

uniform float blur_size = 0.25;
uniform float dim = 0.7;

uniform bool horizontal = false;

#define pow2(x) (x * x)

const int samples = 5;
global uniform int blur_quality;

float gaussian(float x, float sigma) {
	return 1.0 / (2.0 * PI * sigma) * exp(-((pow2(x)) / (2.0 * sigma)));
}

vec3 blur(vec2 screen_uv, sampler2D input_texture, float size) {
	vec3 result = vec3(1.0);
	float steps = size / float(samples * blur_quality);
	float sigma = pow2(float(blur_size) * 0.25);
	float accum = 0.0;
	float weight;
	vec2 direction = horizontal ? vec2(1.0, 0.0) : vec2(0.0, 1.0);

	for(float x = -size; x < size; x += steps) {
		weight = gaussian(x, sigma);
		result += texture(input_texture, screen_uv + direction * x * size).rgb * weight;
		accum += weight;
	}
	
	return result / accum;
}

void blit() {
	COLOR0 = vec4(blur(UV, blur_texture, blur_size / 2.0) * dim, 1.);
}
