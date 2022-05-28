using System.ComponentModel;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public enum TimeFormat {
		yyyy,
		yyyyMM,
		yyyyMMdd,
		yyyyMMddtthh,
		yyyyMMddtthhmm,
		yyyyMMddtthhmmss,

		yyyyMMdd_tthh,
		yyyyMMdd_tthhmm,
		yyyyMMdd_tthhmmss,

		yyyy_MM_dd,
		yyyy_MM_dd_tthh,
		yyyy_MM_dd_tthhmm,
		yyyy_MM_dd_tthhmmss,

		yyyy년MM월dd일,
		yyyy년MM월dd일tthh시,
		yyyy년MM월dd일tthh시mm분,
		yyyy년MM월dd일tthh시mm분ss초,

		[Description("yyyy년MM월dd일_tthh:mm")]
		yyyy년MM월dd일tthh_mm___WithColon,
		[Description("yyyy년MM월dd일_tthh:mm:ss")]
		yyyy년MM월dd일tthh_mm_ss___WithColon,

		yyyy년_MM월_dd일,
		yyyy년_MM월_dd일_tthh시,
		yyyy년_MM월_dd일_tthh시mm분,
		yyyy년_MM월_dd일_tthh시mm분ss초,

		[Description("yyyy년_MM월_dd일_tthh:mm")]
		yyyy년_MM월_dd일_tthh_mm___WithColon,
		[Description("yyyy년_MM월_dd일_tthh:mm:ss")]
		yyyy년_MM월_dd일_tthh_mm_ss___WithColon,

		[Description("yyyy-MM-dd")]
		yyyy_MM_dd___WithHyphen,
		[Description("yyyy-MM-dd-tthh")]
		yyyy_MM_dd_tthh___WithHyphen,
		[Description("yyyy-MM-dd-tthh-mm")]
		yyyy_MM_dd_tthh_mm___WithHyphen,
		[Description("yyyy-MM-dd-tthh-mm-ss")]
		yyyy_MM_dd_tthh_mm___ss_WithHyphen,

		[Description("yyyy-MM-dd-tthh:mm")]
		yyyy_MM_dd_tthh_mm___WithHyphenAndColon,
		[Description("yyyy-MM-dd-tthh:mm:ss")]
		yyyy_MM_dd_tthh_mm_ss___WithHyphenAndColon,

		[Description("yyyyMMdd_tthh:mm")]
		yyyyMMdd_tthh_mm___WithColon,
		[Description("yyyyMMdd_tthh:mm:ss")]
		yyyyMMdd_tthh_mm_ss___WithColon,
	}
}
