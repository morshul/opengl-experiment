#version 330 core

uniform vec2 u_Viewport;
uniform vec2 u_Position;

uniform sampler2D u_Sprite;

layout (location = 0) in vec2 a_VertexPosition;

void main() {
    vec2 relation = vec2(textureSize(u_Sprite, 0)) / u_Viewport;
    vec2 spriteVertexPosition = ((relation * a_VertexPosition) - 1.0) + relation;

    gl_Position = vec4(spriteVertexPosition + ((u_Position / u_Viewport) * 2.0), 0.0, 1.0);
}
