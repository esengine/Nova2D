using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using Nova2D.Engine.ECS;

namespace NovaStudio.Models;

/// <summary>
/// Editable version of TransformComponent for use in the editor UI.
/// Supports binding and conversion to runtime component.
/// </summary>
public partial class EditableTransformComponent : EditableComponent
{
    [ObservableProperty]
    private Vector2 position = Vector2.Zero;

    [ObservableProperty]
    private float rotation = 0f;

    [ObservableProperty]
    private Vector2 scale = Vector2.One;

    public override object ToRuntimeComponent()
    {
        return new TransformComponent
        {
            Position = Position,
            Rotation = Rotation,
            Scale = Scale
        };
    }
}