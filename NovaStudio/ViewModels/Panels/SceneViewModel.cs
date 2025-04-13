using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using Nova2D.Engine.Graphics;

namespace NovaStudio.ViewModels.Panels;

/// <summary>
/// View model for the scene view panel.
/// </summary>
public partial class SceneViewModel : ObservableObject
{
    [ObservableProperty]
    private Camera2D camera = new(1280, 720);

    public void Zoom(float factor)
    {
        camera.Zoom *= new Vector2(factor, factor);
    }

    public void Pan(Vector2 offset)
    {
        camera.Position += offset;
    }

    public void Resize(int width, int height)
    {
        camera.Resize(width, height);
    }
}