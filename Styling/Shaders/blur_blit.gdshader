// This shader is based off of this shader: https://www.shadertoy.com/view/4tSyzy
// I have modified it to be sample count independent, so you can get the same visuals with 16 samples as the original with 30 samples.

shader_type texture_blit;
render_mode blend_disabled;

uniform sampler2D blur_texture:hint_blit_source0;

uniform float blur_size = 0.22;
uniform float dim = 0.85;

uniform bool horizontal = false;

const int samples = 8;
global uniform int blur_quality;

#define pow2(x) (x * x)

void blit() {
	float size = blur_size * 0.5;
	
	int total_samples = samples * blur_quality;
	
	float step_size = size / float(total_samples);
	ivec2 texture_size = textureSize(blur_texture, 0);
	vec2 direction = horizontal ? vec2(1.0, 0.0) : vec2(0.0, float(texture_size.x) / float(texture_size.y));
	
	float sigma = pow2(float(blur_size) * 0.25);
	float two_sigma = (2.0 * sigma);
	float gaussian_denominator = sqrt(2.0 * PI * sigma * sigma);
	
	float accum = 1.0 / gaussian_denominator;
	vec3 result = texture(blur_texture, UV).rgb * accum;
	
	for(int i = -total_samples; i <= total_samples; i++) {
		if (i == 0) continue;
		
		float x = (float(i) / float(total_samples)) * size;
		vec2 uv = UV + (direction * x * size);
		
		if ((uv.x < 0.0 || uv.x > 1.0) || (uv.y < 0.0 || uv.y > 1.0)) continue;
		
		float distance_squared = x * x;
		float weight = exp(-distance_squared / two_sigma) / gaussian_denominator;
		result += texture(blur_texture, uv).rgb * weight;
		accum += weight;
	}
	
	result /= accum;
	
	COLOR0 = vec4(result * (horizontal ? dim : 1.0), 1.0);
}
