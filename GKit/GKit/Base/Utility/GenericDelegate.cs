using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public delegate void Arg1Delegate<ArgT>(ArgT value);
	public delegate void Arg2Delegate<ArgT1, ArgT2>(ArgT1 value1, ArgT2 value2);
	public delegate void Arg3Delegate<ArgT1, ArgT2, ArgT3>(ArgT1 value1, ArgT2 value2, ArgT3 value3);
	public delegate void Arg3Delegate<ArgT1, ArgT2, ArgT3, ArgT4>(ArgT1 value1, ArgT2 value2, ArgT3 value3, ArgT4 value4);
	public delegate ReturnT ReturnDelegate<ReturnT>();
	public delegate ReturnT Arg1ReturnDelegate<ReturnT, ArgT>(ArgT value);
	public delegate ReturnT Arg2ReturnDelegate<ReturnT, ArgT1, ArgT2>(ArgT1 value1, ArgT2 value2);
	public delegate ReturnT Arg2ReturnDelegate<ReturnT, ArgT1, ArgT2, ArgT3>(ArgT1 value1, ArgT2 value2, ArgT3 value3);
	public delegate ReturnT Arg2ReturnDelegate<ReturnT, ArgT1, ArgT2, ArgT3, ArgT4>(ArgT1 value1, ArgT2 value2, ArgT3 value3, ArgT4 value4);
	public delegate void ValueChangedDelegate<T>(T newValue, T oldValue);
}
