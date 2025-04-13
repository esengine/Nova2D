using CommunityToolkit.Mvvm.ComponentModel;
using NovaStudio.Models;
using NovaStudio.Core;
using System.Collections.ObjectModel;

namespace NovaStudio.ViewModels.Panels;

/// <summary>
/// View model for the hierarchy panel.
/// </summary>
public partial class HierarchyViewModel : ObservableObject
{
    public ObservableCollection<EditableEntity> Entities => _context.SceneService.Entities;

    [ObservableProperty]
    private EditableEntity? selected;

    private readonly EditorContext _context;

    public HierarchyViewModel(EditorContext context)
    {
        _context = context;
        Selected = _context.Selection.SelectedEntity;

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Selected))
            {
                _context.Selection.SelectedEntity = Selected;
            }
        };
    }
}