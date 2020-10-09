shader_type canvas_item;

uniform float amplitude = 0.01;
uniform float period = 8.0;
uniform float phaseShift = 0.0;
uniform float horizontalShift = 0.0;

void fragment()
{
	vec2 uv = UV;
	float value = uv.x + TIME;

	uv.y += amplitude * sin(period * value + phaseShift) + horizontalShift;
	COLOR = texture(TEXTURE, uv);
}