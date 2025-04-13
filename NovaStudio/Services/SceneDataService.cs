using System.Collections.ObjectModel;
using NovaStudio.Models;

namespace NovaStudio.Services;

/// <summary>
/// Provides scene-level data access and operations.
/// </summary>
public class SceneDataService
{
    public ObservableCollection<EditableEntity> Entities { get; } = new();

    public void Clear()
    {
        Entities.Clear();
    }

    public void AddEntity(EditableEntity entity)
    {
        Entities.Add(entity);
    }
}