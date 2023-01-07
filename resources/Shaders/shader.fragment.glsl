#version 330 core

uniform vec2 u_Viewport;
uniform vec2 u_Position;

uniform sampler2D u_Sprite;

layout (location = 0) out vec4 a_ColorOutput;

void main() {
    ivec2 relativePixelPosition = ivec2(gl_FragCoord.xy - u_Position);
    vec4 color = texelFetch(u_Sprite, relativePixelPosition, 0);

    if (color.a == 0) {
        discard;
    }

    a_ColorOutput = color;
}
