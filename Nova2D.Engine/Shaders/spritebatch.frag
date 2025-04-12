#version 330 core

in vec2 TexCoord;
in vec4 Color;

out vec4 FragColor;

uniform sampler2D uTexture;

void main()
{
    vec4 texColor = texture(uTexture, TexCoord);
    vec4 finalColor = texColor * Color;

    if (finalColor.a < 0.01)
        discard;

    FragColor = finalColor;
}
