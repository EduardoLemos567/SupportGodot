shader_type canvas_item;
render_mode unshaded;

/*
	Generate a tilemap, where each tile has blended borders, using biome textures.
*/

const int MAX_BIOMES = 6;

uniform sampler2D noise1 : repeat_enable, filter_nearest_mipmap;
uniform sampler2D albedoWater : repeat_enable, filter_linear_mipmap;
uniform sampler2D albedoSand : repeat_enable, filter_linear_mipmap;
uniform sampler2D albedoDryGrass : repeat_enable, filter_linear_mipmap;
uniform sampler2D albedoGrass : repeat_enable, filter_linear_mipmap;
uniform sampler2D albedoRock : repeat_enable, filter_linear_mipmap;
uniform sampler2D albedoSnow : repeat_enable, filter_linear_mipmap;
uniform int squareGrid;
uniform float smoother : hint_range(-0.1, .5, .01);
uniform bool drawEdge;
uniform vec3 edgeColor : source_color;
uniform float edgeSize : hint_range(0.01, 0.95, 0.01);
uniform float zoom : hint_range(0.01, 10.0, 0.001) = 1;
uniform vec2 position;

float imix(in float imin, in float imax, in float value) {
	return (value - imin) / (imax - imin);
}

vec3 mix3(in vec3 c1, in vec3 c2, in vec3 c3, in float v) {
	if(v < 0.5) { return mix(c1, c2, v * 2.); }
	return mix(c2, c3, (v - .5) * 2.);
}

float remap(in float imin, in float imax, in float omin, in float omax, in float value) {
	return mix(omin, omax, imix(imin, imax, value));
}

vec2 getSquarePixel(in vec2 pixel) {
	return fract(pixel * float(squareGrid));
}

ivec2 getCoord(in vec2 pixel) {
	return ivec2(floor(pixel * float(squareGrid)));
}
// Given a square coord, find the square center in UV space
vec2 getCenter(in ivec2 coord) {
	return vec2(coord) / float(squareGrid) + 0.5 / float(squareGrid);
}

vec2 getSector(in vec2 center, in vec2 pixel) {
	/*
	(-,-)(+,-)
	(-,+)(+,+)
	*/
	return sign(pixel - center);
}
// Given a coord, find its biome id in a given data texture.
int getBiome(in ivec2 coord) {
	return int(textureLod(noise1, vec2(coord) / float(squareGrid), 0).r * float(MAX_BIOMES));
}
// Given a pixel uv and biome id, find the pixel from availiable textures
vec3 biomeToPixel(vec2 pixel, in int biome) {
	pixel *= float(squareGrid);
	switch(biome) {
		case 0:
			return texture(albedoWater, pixel).rgb;
		case 1:
			return texture(albedoSand, pixel).rgb;
		case 2:
			return texture(albedoDryGrass, pixel).rgb;
		case 3:
			return texture(albedoGrass, pixel).rgb;
		case 4:
			return texture(albedoRock, pixel).rgb;
		case 5:
			return texture(albedoSnow, pixel).rgb;
	}
}

vec3 calculatePixel(in vec2 pixel) {
	ivec2 selfCoord = getCoord(pixel);
	vec2 selfCenter = getCenter(selfCoord);
	vec2 sector = getSector(selfCenter, pixel);
	// coordinates of the interpolation square where the pixel is within
	// 0:tl, 1:tr, 2:bl, 3:br
	ivec2 coords[4] = {
		selfCoord + ivec2(0, int(sector.y)),
		selfCoord + ivec2(sector),
		selfCoord,
		selfCoord + ivec2(int(sector.x), 0)
	};
	vec2 diagonalCenter = getCenter(coords[1]);
	// colors of each coordinate, found by biome, to interpolate
	vec3 colors[4] = {
		biomeToPixel(pixel, getBiome(coords[0])),
		biomeToPixel(pixel, getBiome(coords[1])),
		biomeToPixel(pixel, getBiome(coords[2])),
		biomeToPixel(pixel, getBiome(coords[3]))
	};
	vec2 p = smoothstep(smoother, 1. - smoother, (pixel - selfCenter) / (diagonalCenter - selfCenter));
	vec3 a = mix(colors[0], colors[1], p.x);
	vec3 b = mix(colors[2], colors[3], p.x);
	return mix(a, b, 1. - p.y);
}

float getEdgeDistance(in vec2 squarePixel) {
    vec2 p = abs(squarePixel - .5) * 2.;
	return 1. - max(p.x, p.y);
}

void fragment() {
	vec2 uv = UV * (1./zoom) + position;
	COLOR.rgb = calculatePixel(uv);
	if(drawEdge) {
		COLOR.rgb = mix(COLOR.rgb, edgeColor, step(getEdgeDistance(getSquarePixel(uv)), edgeSize));
	}
}
