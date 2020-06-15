#if OnUnity
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKitForUnity {
	public class AnimatorBoolSetter : StateMachineBehaviour {
		public string paramName;
		public bool value;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool(paramName, value);
		}
	}
}
#endif