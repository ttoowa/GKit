#if UNITY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace GKit {
	[CreateAssetMenu(fileName = "B2Environment")]
	public class B2Environment : ScriptableObject {
		[Header("환경")]
		public Vector2 gravity = new Vector2(0f, -0.018f);
		public Vector2 maxSpeed = new Vector2(1f, 1f);
		public Vector2 airDrag = new Vector2(0.07f, 0.02f);

		[Space]
		[Header("판정")]
		public int groundCallRelaxation = 3;
		public float groundAngle = 0.7f * 90;
	}
}
#endif