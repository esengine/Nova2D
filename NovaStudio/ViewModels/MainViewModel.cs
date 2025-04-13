using System.Collections.ObjectModel;
using NovaStudio.Core;
using NovaStudio.ViewModels.Panels;

namespace NovaStudio.ViewModels;

/// <summary>
/// View model for the main editor window.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    public EditorContext Context { get; }

    public HierarchyViewModel Hierarchy { get; }
    public InspectorViewModel Inspector { get; }
    public Panels.SceneViewModel Scene { get; }
    
    public MainViewModel()
    {
        Context = new EditorContext();

        Hierarchy = new HierarchyViewModel(Context);
        Scene = new Panels.SceneViewModel();
        Inspector = new InspectorViewModel(Context);
    }
}
