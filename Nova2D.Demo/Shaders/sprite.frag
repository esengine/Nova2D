#version 330 core

in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D uTexture;
uniform vec4 uColor;
uniform bool uGrayscale;
uniform bool uDiscardTransparent;

void main()
{
    vec4 texColor = texture(uTexture, TexCoord);

    if (uDiscardTransparent && texColor.a == 0.0)
    discard;

    vec3 color = texColor.rgb;

    if (uGrayscale) {
        float gray = dot(color, vec3(0.299, 0.587, 0.114));
        color = vec3(gray);
    }

    FragColor = vec4(color, texColor.a) * uColor;
}
