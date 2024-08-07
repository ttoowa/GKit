using System;

#if OnUnity
namespace GKitForUnity;
#elif OnWPF
namespace GKitForWPF;
#else
namespace GKit;
#endif

public class DirtyFlagAction {
    public bool IsDirty { get; private set; }
    
    private readonly Action action;
    
    public DirtyFlagAction(Action action) {
        this.action = action;
    }
    
    public void SetDirty() {
        IsDirty = true;
    }
    
    public bool Invoke(bool force = false) {
        if (IsDirty || force) {
            IsDirty = false;
            
            action?.Invoke();
            return true;
        }
        
        return false;
    }
}