using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Nova2D.Engine.ECS;

namespace NovaStudio.Models;

/// <summary>
/// A view model representation of an ECS entity used in the editor.
/// Contains editable components and can be converted into a runtime entity.
/// </summary>
public partial class EditableEntity : ObservableObject
{
    [ObservableProperty]
    private string name = "New Entity";
    
    public ObservableCollection<EditableComponent> Components { get; } = new();
    
    /// <summary>
    /// Converts this editable entity into a runtime Nova2D entity.
    /// </summary>
    public Entity ToRuntimeEntity()
    {
        var entity = new Entity();

        foreach (var comp in Components)
        {
            entity.Add(comp.ToRuntimeComponent());
        }

        return entity;
    }
}