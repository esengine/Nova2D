using CommunityToolkit.Mvvm.ComponentModel;
using NovaStudio.ViewModels;

namespace NovaStudio.Models;

/// <summary>
/// Base class for all editable components in the editor.
/// Inherits ObservableObject to support data binding.
/// </summary>
public abstract class EditableComponent : ObservableObject
{
    /// <summary>
    /// Converts this editable component into a runtime ECS component.
    /// </summary>
    /// <returns>Component instance compatible with Nova2D.Engine ECS.</returns>
    public abstract object ToRuntimeComponent();
    
    public virtual string DisplayName => GetType().Name;
}