using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModernTerminal {
  class EnumDataTemplateSelector : DataTemplateSelector {
    public EnumDataTemplateSelector() {
    }

    public string Prefix { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container) {
      if (item == null) return null;

      if (!item.GetType().IsEnum) {
        Debug.WriteLine("EnumDataTemplateSelector expects an item of type Enum");
        return null;
      }

      var containerAsUIElement = container as FrameworkElement;

      var valueNameInEnum = item.GetType().GetEnumName(item);
      var template = containerAsUIElement.TryFindResource(Prefix + valueNameInEnum) as DataTemplate;
      if (template == null) {
        Debug.WriteLine($"EnumDataTemplateSelector could not find DataTemplate '{Prefix + valueNameInEnum}'");
      }
      return template;
    }
  }
}
