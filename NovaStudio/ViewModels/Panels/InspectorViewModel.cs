using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NovaStudio.Core;
using NovaStudio.Models;

namespace NovaStudio.ViewModels.Panels;

/// <summary>
/// View model for the inspector panel.
/// </summary>
public partial class InspectorViewModel : ObservableObject
{
    public ObservableCollection<EditableComponent> Components { get; } = new();

    private readonly EditorContext _context;

    public InspectorViewModel(EditorContext context)
    {
        _context = context;

        _context.Selection.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_context.Selection.SelectedEntity))
            {
                UpdateComponents();
            }
        };

        UpdateComponents();
    }

    private void UpdateComponents()
    {
        Components.Clear();

        var entity = _context.Selection.SelectedEntity;
        if (entity == null) return;

        foreach (var component in entity.Components)
        {
            Components.Add(component);
        }
    }
}