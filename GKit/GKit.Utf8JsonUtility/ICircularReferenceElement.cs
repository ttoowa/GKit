using System.Collections.Generic;
using System.Linq;

namespace GKit.Utf8JsonUtility;

public interface ICircularReferenceElement<T> where T : ICircularReferenceElement<T>{
    string Id { get; }
    IList<string> FlatChildren { get; }
    IList<T> Children { get; }
    
    public List<T> FlattenChildrens() {
        List<T> flatList = new List<T>();
        if (Children != null) {
            foreach (T item in Children) {
                if (!flatList.Contains(item)) {
                    flatList.Add(item);
                    flatList.AddRange(item.FlattenChildrens());
                }
            }
        }
        
        return flatList;
    }
    
    public void RestoreChildrens() {
        List<T> flatList = FlattenChildrens();
        Dictionary<string, T> lookup = new Dictionary<string, T>(flatList.Count);
        foreach (T element in flatList) {
            lookup[element.Id] = element;
        }
        
        foreach (T element in flatList) {
            if (element.FlatChildren != null) {
                element.Children.Clear();
                foreach (string id in element.FlatChildren) {
                    if(lookup.TryGetValue(id, out T child)) {
                        element.Children.Add(child);
                    }
                }
            }
        }
    }
}