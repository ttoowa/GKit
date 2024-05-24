using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if OnUnity
namespace GKitForUnity;
#elif OnWPF
namespace GKitForWPF;
#else
namespace GKit;
#endif

public static class FlattableNodeUtility {
    public static List<T> FlattenChildrens<T>(this IFlattableNode rootElement) where T : IFlattableNode {
        return rootElement.Flatten<T>();
    }
    
    public static void RestoreChildrens<T>(this IFlattableNode element, Dictionary<string, T> lookup) where T : IFlattableNode {
        element.RestoreChildrens(lookup);
    }
    
    public static Dictionary<string, T> CreateLookup<T>(this IEnumerable<T> elements) where T : IFlattableNode {
        Dictionary<string, T> lookup = new();
        foreach (T element in elements) {
            lookup[element.Id] = element;
        }
        
        return lookup;
    }
    
    public static void SerializeNode<T>(this IRootFlattableNodeContainer rootContainer, T rootElement) where T : IFlattableNode {
        rootContainer.SerializeNode<T>(rootElement);
    }
    
    public static T DeserializeNode<T>(this IRootFlattableNodeContainer rootContainer) where T : IFlattableNode {
        return rootContainer.DeserializeNode<T>();
    }
}

public interface IRootFlattableNodeContainer {
    public string RootElementId { get; set; }
    public IList FlatElements { get; }
    
    public void SerializeNode<T>(IFlattableNode rootElement) where T : IFlattableNode {
        FlatElements.Clear();
        List<T> childFlattenChilds = rootElement.Flatten<T>();
        RootElementId = rootElement.Id;
        foreach (IFlattableNode element in childFlattenChilds) {
            FlatElements.Add(element);
        }
    }
    
    public T DeserializeNode<T>() where T : IFlattableNode {
        Dictionary<string, T> lookup = new();
        foreach (T element in FlatElements) {
            lookup[element.Id] = element;
        }
        
        foreach (T element in FlatElements) {
            element.RestoreChildrens(lookup);
        }
        
        return lookup[RootElementId];
    }
}

public interface IFlattableNode {
    string Id { get; set; }
    IList<string> ChildrenIds { get; }
    IList Children { get; }
    
    public List<T> Flatten<T>() where T : IFlattableNode {
        HashSet<T> resultSet = new();
        Flatten<T>(resultSet);
        
        List<T> resultList = new();
        foreach (T item in resultSet) {
            resultList.Add(item);
        }
        
        return resultList;
    }
    
    private void Flatten<T>(HashSet<T> resultSet) where T : IFlattableNode {
        ChildrenIds.Clear();
        
        T _this = (T)this;
        if (string.IsNullOrWhiteSpace(_this.Id)) {
            _this.Id = Guid.NewGuid().ToString();
        }
        
        resultSet.Add(_this);
        
        if (Children != null) {
            foreach (T item in Children) {
                if (string.IsNullOrWhiteSpace(item.Id)) {
                    item.Id = Guid.NewGuid().ToString();
                }
                
                ChildrenIds.Add(item.Id);
                
                item.Flatten<T>(resultSet);
            }
        }
    }
    
    public void RestoreChildrens<T>(Dictionary<string, T> lookup) {
        Children.Clear();
        foreach (string childId in ChildrenIds) {
            if (lookup.TryGetValue(childId, out T child)) {
                Children.Add(child);
            }
        }
    }
}