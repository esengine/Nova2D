using NovaStudio.Services;

namespace NovaStudio.Core;

/// <summary>
/// Provides global editor state and shared services.
/// </summary>
public class EditorContext
{
    public SceneDataService SceneService { get; } = new();
    public EditorSelection Selection { get; } = new();
}