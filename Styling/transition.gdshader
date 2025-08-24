shader_type canvas_item;

uniform sampler2D past_image;
uniform sampler2D new_image;
uniform float t;

vec2 calculate_uv(vec2 uv, sampler2D tex)
{
	vec2 output = uv;
	vec2 tsize = vec2(textureSize(tex, 0));
	
	float x_y = tsize.x / tsize.y;
	output.x += (x_y - 1.0) / 2.0;
	output.x /= x_y;
	
	return output;
}

void fragment() {
	vec2 uv1 = calculate_uv(UV, past_image);
	vec2 uv2 = calculate_uv(UV, new_image);
	COLOR = mix(texture(past_image, uv1), texture(new_image, uv2), t);
}
