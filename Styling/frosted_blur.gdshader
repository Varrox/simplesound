shader_type canvas_item;
render_mode unshaded;

uniform float darkness;
uniform int samples:hint_range(1, 35, 1) = 16;
uniform float blur_amount:hint_range(0, 1, 0.001) = 1.0;
uniform float blur_size:hint_range(0, 1, 0.001) = 0.02;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable;

vec3 blur(vec2 screen_uv, float size)
{
	vec4 result = vec4(1.);
	float steps = size / float(samples);

	for(float x = -size; x < size; x += steps)
	{
		for(float y = -size; y < size; y += steps)
		{
			vec2 point = screen_uv + vec2(x, y) * size;
			result += texture(screen_texture, point);
		}
	}

	return normalize(result / float(samples)).rgb;
}

void fragment() {
	COLOR = vec4(vec3(1.0 - darkness), 1.0) * mix(texture(screen_texture, UV), vec4(blur(SCREEN_UV, blur_size), 1.), blur_amount);
}