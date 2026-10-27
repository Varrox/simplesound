// This shader is based off of this shader: https://www.shadertoy.com/view/4tSyzy
// I have modified it to be sample count independent, so you can get the same visuals with 16 samples as the original with 30 samples.

shader_type texture_blit;

uniform sampler2D blur_texture:hint_blit_source0;

uniform float blur_size:hint_range(0.001, 1, 0.001) = 0.25;
uniform bool horizontal = false;

#define pow2(x) (x * x)

const int samples = 5;
global uniform int blur_quality;

float gaussian(vec2 i, float sigma) {
	return 1.0 / (2.0 * PI * sigma) * exp(-((pow2(i.x) + pow2(i.y)) / (2.0 * sigma)));
}

vec3 blur(vec2 screen_uv, sampler2D input_texture, float size) {
	vec3 result = vec3(1.);
	float steps = size / float(samples * blur_quality);
	float sigma = pow2(float(blur_size) * 0.25);
	float accum = 0.0;
	float weight;
	vec2 offset;

	for(float x = -size; x < size; x += steps)
	{
		offset = horizontal ? vec2(x, 0.0) : vec2(0.0, x);
		weight = gaussian(offset, sigma);
		result += texture(input_texture, screen_uv + offset * size).rgb * weight;
		accum += weight;
	}
	
	return result / accum;
}

void blit() {
	vec3 b = blur(UV, blur_texture, blur_size / 2.0) * 0.7;
	
	COLOR0 = vec4(b, 1.);
}
