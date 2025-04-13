using CommunityToolkit.Mvvm.ComponentModel;
using NovaStudio.Models;

namespace NovaStudio.Core;

/// <summary>
/// Holds the current editor selection state.
/// </summary>
public partial class EditorSelection : ObservableObject
{
    [ObservableProperty]
    private EditableEntity? selectedEntity;
}