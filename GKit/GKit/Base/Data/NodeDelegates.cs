#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
    .Data {
    public delegate void NodeItemDelegate<ItemBase, ParentItem>(ItemBase item, ParentItem parentItem);

    public delegate void NodeItemInsertedDelegate<ItemBase>(int index, ItemBase childItem);
}