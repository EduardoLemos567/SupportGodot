float imix(in float minValue, in float maxValue, in float value) {
	return (value - minValue) / (maxValue - minValue);
}

float remap(in float value, in float inRangeMin , in float inRangeMax , in float outRangeMin , in float outRangeMax ) {
	return mix(outRangeMin, outRangeMax, imix(inRangeMin, inRangeMax, value));
}

float remapV(in float value, in vec2 inRange, in vec2 outRange) {
	return remap(value, inRange.x, inRange.y, outRange.x, outRange.y);
}

vec3 CalculateWorldVertex(in mat4 view_matrix, in vec3 vertex) {
	return (view_matrix * vec4(vertex, 1)).xyz;
}