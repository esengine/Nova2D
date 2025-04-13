using System;
using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using Nova2D.Engine.Core;
using Nova2D.Engine.ECS;
using Nova2D.Engine.Graphics;

namespace NovaStudio.Models;

/// <summary>
/// Editable version of SpriteComponent for use in the editor.
/// </summary>
public partial class EditableSpriteComponent : EditableComponent
{
    [ObservableProperty]
    private string? texturePath;

    [ObservableProperty]
    private Vector2 size = new(64, 64);

    [ObservableProperty]
    private Vector4 color = Vector4.One;

    [ObservableProperty]
    private Vector2 origin = Vector2.Zero;

    public override object ToRuntimeComponent()
    {
        if (string.IsNullOrWhiteSpace(TexturePath))
            throw new InvalidOperationException("SpriteComponent requires a TexturePath.");

        if (!NovaContext.Textures.TryGetValue(TexturePath, out var texture))
        {
            texture = new Texture(NovaContext.GL!, TexturePath);
            NovaContext.Textures[TexturePath] = texture;
        }

        return new SpriteComponent(texture)
        {
            Size = Size,
            Color = Color,
            Origin = Origin
        };
    }
}