using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ModernTerminal {
  class PageDefinition {
    public string Name { get; set; }
    public DataTemplate Template { get; set; }
  }

  class PagesView: Control {
    public List<PageDefinition> Pages { get; set; }
    private Stack<string> PageStack;

    public PagesView() {
      DefaultStyleKey = GetType();
      Pages = new List<PageDefinition>();
      PageStack = new Stack<string>();
    }

    private Grid hostGrid;
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();

      hostGrid = Template.FindName("HostGrid", this) as Grid;

      if (hostGrid != null) {
        var pageHost = new ContentControl();
        hostGrid.Children.Add(pageHost);
      }

      NavigateTo(Pages[0].Name);
    }

    public void NavigateTo(string dstPageName) {
      foreach(var page in Pages) {
        if (page.Name.Equals(dstPageName, StringComparison.OrdinalIgnoreCase)) {
          var pageHost = hostGrid.Children[0] as ContentControl;
          pageHost.ContentTemplate = page.Template;
          PageStack.Push(page.Name);
        }
      }
    }

    public void NavigateBack() {
      if (PageStack.Count > 1) {
        PageStack.Pop();
        NavigateTo(PageStack.Pop());
      }
    }
  }

  [DefaultTrigger(typeof(ButtonBase), typeof(Microsoft.Xaml.Behaviors.EventTrigger), "Click")]
  public abstract class PrototypingActionBase : TriggerAction<DependencyObject> {

  }

  public sealed class NavigateToPageAction : PrototypingActionBase {
    public static readonly DependencyProperty TargetPageProperty = DependencyProperty.Register(
        "TargetPage", typeof(string), typeof(NavigateToPageAction), new PropertyMetadata(null));

    public string TargetPage {
      get { return GetValue(TargetPageProperty) as string; }
      set { SetValue(TargetPageProperty, value as string); }
    }

    protected override void Invoke(object parameter) {
      var pagesView = Helpers.GetParentOfType<PagesView>(this.AssociatedObject);
      if (pagesView != null) {
        pagesView.NavigateTo(TargetPage);
      }
    }

    protected override Freezable CreateInstanceCore() {
      return new NavigateToPageAction();
    }
  }

  public sealed class NavigateBackAction : PrototypingActionBase {
    protected override void Invoke(object parameter) {
      var pagesView = Helpers.GetParentOfType<PagesView>(this.AssociatedObject);
      if (pagesView != null) {
        pagesView.NavigateBack();
      }
    }

    protected override Freezable CreateInstanceCore() {
      return new NavigateBackAction();
    }
  }
}
