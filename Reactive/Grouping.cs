using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal {
  public class Group<TKey, TItem> {
    public TKey Key { get; }
    public ObservableCollection<TItem> Items { get; }

    public Group(TKey key, TItem firstItem) {
      Key = key;
      Items = new ObservableCollection<TItem>();
      Items.Add(firstItem);
    }
  }

  public class Grouping<TKey, TItem> : ObservableCollection<Group<TKey, TItem>> {
    private readonly ObservableCollection<TItem> source;
    private readonly Func<TItem, TKey> keySelector;

    public Grouping(ObservableCollection<TItem> source, Func<TItem, TKey> keySelector) {
      this.source = source;
      this.keySelector = keySelector;

      this.source.CollectionChanged += Source_CollectionChanged;
    }

    private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      switch (e.Action) {
        case NotifyCollectionChangedAction.Add:
          for(var i=0; i<e.NewItems.Count; i++) {
            AddOne((TItem)e.NewItems[i]);
          }
          break;

        case NotifyCollectionChangedAction.Move:
          throw new NotImplementedException();
          // break;

        case NotifyCollectionChangedAction.Remove:
          throw new NotImplementedException();
          // break;

        case NotifyCollectionChangedAction.Replace:
          throw new NotImplementedException();
          // break;

        case NotifyCollectionChangedAction.Reset:
          Clear();
          break;
      }
    }

    private void Resync() {
      this.Clear();

      foreach(var item in source) {
        AddOne(item);
      }
    }

    private void AddOne(TItem item) {
      var key = keySelector(item);
      for(var i=0; i<Count; i++) {
        if (object.Equals(Items[i].Key, key)) {
          // Group found, add to this one
          Items[i].Items.Add(item);
          return;
        }
      }
      // No group found, add a new one
      Add(new Group<TKey, TItem>(key, item));
    }
  }
}
