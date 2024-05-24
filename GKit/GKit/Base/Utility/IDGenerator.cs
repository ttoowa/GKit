using System;
using System.Collections.Generic;

#if OnUnity
namespace GKitForUnity;
#elif OnWPF
namespace GKitForWPF;
#else
namespace GKit;
#endif
/// <summary>
///     중복되지 않는 ID를 생성하는 클래스입니다.
/// </summary>
public class IDGenerator {
    public int StartIndex { get; private set; }
    public int Capacity { get; private set; }
    private List<int> IDList;
    
    public IDGenerator(int startIndex = 0) {
        StartIndex = startIndex;
        IDList = new List<int>();
        Expand(16);
    }
    
    public int GetID() {
        if (IDList.Count == 0) {
            Expand(1);
        }
        
        int index = IDList.Count - 1;
        int ID = IDList[index];
        IDList.RemoveAt(index);
        return ID;
    }
    
    public void MarkIDUsed(int ID) {
        if (StartIndex + Capacity <= ID) {
            Expand(ID - (StartIndex + Capacity) + 1);
        }
        
        IDList.Remove(ID);
    }
    
    public void ReturnID(int ID) {
        IDList.Add(ID);
    }
    
    public void Clear() {
        Capacity = 0;
        IDList.Clear();
    }
    
    private void Expand(int count) {
        int i = StartIndex + Capacity;
        Capacity += count;
        for (; i < Capacity; ++i) {
            IDList.Add(i);
        }
    }
}