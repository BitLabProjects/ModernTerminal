using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernTerminal {
  class PageDefinition {
    public string Name { get; set; }
    public DataTemplate Template { get; set; }
  }

  class PagesView : Control {
    public List<PageDefinition> Pages { get; set; }

    private class PageStackEntry {
      public readonly string Name;
      public readonly string Argument;
      public PageStackEntry(string name, string argument) {
        Name = name;
        Argument = argument;
      }
    }
    private Stack<PageStackEntry> PageStack;

    public PagesView() {
      DefaultStyleKey = GetType();
      Pages = new List<PageDefinition>();
      PageStack = new Stack<PageStackEntry>();
    }

    public string GetArgument() {
      if (PageStack.Count == 0) return null;
      return PageStack.Peek().Argument;
    }

    private Grid hostGrid;
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();

      hostGrid = Template.FindName("HostGrid", this) as Grid;

      if (hostGrid != null) {
        //CreatePageHost();
      }

      NavigateTo(Pages[0].Name);
    }

    private ContentControl CreatePageHost() {
      var pageHost = new ContentControl();
      var x = new TranslateTransform();
      pageHost.RenderTransform = x;
      var b = new Binding("DataContext");
      b.Source = this;
      pageHost.SetBinding(ContentControl.ContentProperty, b);
      hostGrid.Children.Add(pageHost);
      return pageHost;
    }

    public void NavigateTo(string dstPageName, string argument = null) {
      foreach (var page in Pages) {
        if (page.Name.Equals(dstPageName, StringComparison.OrdinalIgnoreCase)) {
          if (hostGrid.Children.Count > 0) {
            var pageHostCurr = hostGrid.Children[0] as ContentControl;
            AnimateFadeOut(pageHostCurr);
          }

          var pageHostNew = CreatePageHost();
          pageHostNew.ContentTemplate = page.Template;
          //AnimateFadeIn(pageHostNew);

          PageStack.Push(new PageStackEntry(page.Name, argument));
        }
      }
    }

    private void AnimateFadeOut(UIElement element) {
      var s = new Storyboard();
      var animOpacity = AnimationUtils.CreateAnimation(0);
      Storyboard.SetTarget(animOpacity, element);
      Storyboard.SetTargetProperty(animOpacity, new PropertyPath("Opacity"));
      s.Children.Add(animOpacity);

      s.Completed += (s1, e1)=> {
        hostGrid.Children.Remove(element);
      };

      BeginStoryboard(s);
    }
    private void AnimateFadeIn(UIElement element) {
      var s = new Storyboard();
      var animOpacity = AnimationUtils.CreateAnimation(0, 1, 2000);
      Storyboard.SetTarget(animOpacity, element);
      Storyboard.SetTargetProperty(animOpacity, new PropertyPath("Opacity"));
      s.Children.Add(animOpacity);

      var animSlide = AnimationUtils.CreateAnimation(0, 100, 2000);
      Storyboard.SetTarget(animSlide, element.RenderTransform);
      Storyboard.SetTargetProperty(animSlide, new PropertyPath("X"));
      s.Children.Add(animSlide);

      BeginStoryboard(s);
    }

    public void NavigateBack() {
      if (PageStack.Count > 1) {
        PageStack.Pop();
        NavigateTo(PageStack.Pop().Name);
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
    public static readonly DependencyProperty ArgumentProperty = DependencyProperty.Register(
        "Argument", typeof(string), typeof(NavigateToPageAction), new PropertyMetadata(null));
    public string Argument {
      get { return GetValue(ArgumentProperty) as string; }
      set { SetValue(ArgumentProperty, value as string); }
    }

    protected override void Invoke(object parameter) {
      var pagesView = Helpers.GetParentOfType<PagesView>(this.AssociatedObject);
      if (pagesView != null) {
        pagesView.NavigateTo(TargetPage, Argument);
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
