// This shader is based off of this shader: https://www.shadertoy.com/view/4tSyzy
// I have modified it to be sample count independent, so you can get the same visuals with 16 samples as the original with 30 samples.

shader_type canvas_item;
render_mode unshaded;

uniform int samples:hint_range(1, 32, 1) = 8;
uniform float blur_size:hint_range(0.001, 1, 0.001) = 0.25;

#define pow2(x) (x * x)

varying float sigma;

global uniform int blur_quality;

float gaussian(vec2 i) {
    return 1.0 / (2.0 * PI * sigma) * exp(-((pow2(i.x) + pow2(i.y)) / (2.0 * sigma)));
}

vec3 blur(vec2 screen_uv, sampler2D input_texture, float size, vec2 scale) {
	vec3 result = vec3(1.);
	float steps = size / float(samples * blur_quality);
	float accum = 0.0;
	float weight;
	vec2 offset;

	for(float x = -size; x < size; x += steps)
	{
		for(float y = -size; y < size; y += steps)
		{
			offset = vec2(x, y);
			weight = gaussian(offset);
			result += texture(input_texture, screen_uv + offset * size).rgb * weight;
			accum += weight;
		}
	}
	
	return result / accum;
}

void fragment() {
	sigma = pow2(float(blur_size) * 0.25);
	COLOR = vec4(blur(UV, TEXTURE, blur_size / 2.0, vec2(1.0) / 1.0 / SCREEN_PIXEL_SIZE * .000001 * blur_size) * 0.7, 1.);
}