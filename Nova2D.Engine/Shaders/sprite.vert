#version 330 core

layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;

uniform mat4 uMVP;
uniform bool uFlipX;
uniform bool uFlipY;

void main()
{
    vec2 coord = aTexCoord;
    if (uFlipX) coord.x = 1.0 - coord.x;
    if (uFlipY) coord.y = 1.0 - coord.y;

    TexCoord = coord;
    gl_Position = uMVP * vec4(aPos, 0.0, 1.0);
}
