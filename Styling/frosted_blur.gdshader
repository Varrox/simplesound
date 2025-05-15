shader_type canvas_item;
render_mode unshaded;

uniform float darkness;
uniform float blur_amount:hint_range(0, 1, 0.001) = 1.0;
uniform float blur_size:hint_range(0, 1, 0.001) = 0.02;

const int samples = 16;
const float sigma = float(samples) * 0.25;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable;

#define pow2(x) (x * x)

float gaussian(vec2 i) {
    return 1.0 / (2.0 * PI * pow2(sigma)) * exp(-((pow2(i.x) + pow2(i.y)) / (2.0 * pow2(sigma))));
}

vec3 blur(vec2 screen_uv, float size)//, vec2 screen_pixel_size)
{
	vec4 result = vec4(1.);
	float steps = size / float(samples);

	for(float x = -size; x < size; x += steps)
	{
		for(float y = -size; y < size; y += steps)
		{
			vec2 point = screen_uv + vec2(x, y) * size; //* screen_pixel_size;
			result += texture(screen_texture, point) * gaussian(vec2(x, y) * size);
		}
	}

	return normalize(result / float(samples)).rgb;
}

void fragment() {
	COLOR = vec4(vec3(1.0 - darkness), 1.0) * mix(texture(screen_texture, UV), vec4(blur(SCREEN_UV, blur_size), 1.), blur_amount);
}