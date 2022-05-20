namespace GKitForWPF.UI.Controls {
    public enum NodeDirection {
        /// <summary>
        /// 아이템 위에 배치
        /// </summary>
        Top,
        /// <summary>
        /// 아이템 아래에 배치
        /// </summary>
        Bottom,
        /// <summary>
        /// 폴더 내 아이템 아래에 배치
        /// </summary>
        InnerBottom,
        /// <summary>
        /// 폴더가 비어있는 경우 내부 index 0에 배치
        /// </summary>
        InnerTop,
    }
}
