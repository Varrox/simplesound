// This shader is based off of this shader: https://www.shadertoy.com/view/4tSyzy
// I have modified it to be sample count independent, so you can get the same visuals with 16 samples as the original with 30 samples.

shader_type canvas_item;
render_mode unshaded;

uniform float blur_size:hint_range(0.001, 1, 0.001) = 0.25;

#define pow2(x) (x * x)

const int samples = 5;
global uniform int blur_quality;

void fragment() {
	vec3 result = vec3(1.0);
	float accum = 0.0;
	
	float size = blur_size * 0.5;
	float steps = size / float(samples * blur_quality);
	
	float sigma = pow2(float(blur_size) * 0.25);
	
	for(float x = -size; x < size; x += steps) {
		for(float y = -size; y < size; y += steps) {
			vec2 uv = UV + vec2(x, y) * size;
			if ((uv.x >= 0.0 && uv.x <= 1.0) && (uv.y >= 0.0 && uv.y <= 1.0)) {
				float weight = exp(-((pow2(x) + pow2(y)) / (2.0 * sigma))) / (2.0 * PI * sigma);
				result += texture(TEXTURE, uv).rgb * weight;
				accum += weight;
			}
		}
	}
	
	result /= accum;
	
	COLOR = vec4(result * 0.7, 1.0);
}
