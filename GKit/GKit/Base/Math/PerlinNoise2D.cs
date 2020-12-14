using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if OnUnity
using UnityEngine;
using Random = System.Random;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public class PerlinNoise2d {
		private Random random;
		private int[] permutation;

		private Vector2[] gradients;

		public PerlinNoise2d(int seed = 0) {
			SetSeed(seed);
		}

		public void SetSeed(int seed) {
			random = new Random(seed);

			CalculatePermutation(out permutation);
			CalculateGradients(out gradients);
		}
		public float Noise(float x, float y) {
			var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

			var total = 0f;

			var corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

			foreach (var n in corners) {
				var ij = cell + n;
				var uv = new Vector2(x - ij.x, y - ij.y);

				var index = permutation[(int)ij.x % permutation.Length];
				index = permutation[(index + (int)ij.y) % permutation.Length];

				var grad = gradients[index % gradients.Length];

				total += Q(uv.x, uv.y) * GMath.Dot(grad, uv);
			}

			return Math.Max(Math.Min(total, 1f), -1f);
		}

		private void CalculatePermutation(out int[] p) {
			p = Enumerable.Range(0, 256).ToArray();

			/// shuffle the array
			for (var i = 0; i < p.Length; i++) {
				var source = random.Next(p.Length);

				var t = p[i];
				p[i] = p[source];
				p[source] = t;
			}
		}
		private void CalculateGradients(out Vector2[] grad) {
			grad = new Vector2[256];

			for (var i = 0; i < grad.Length; i++) {
				Vector2 gradient;

				do {
					gradient = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
				}
				while (gradient.sqrMagnitude >= 1);
#if OnUnity
                gradient = gradient.normalized;
#else
                gradient = gradient.Normalized;
#endif

				grad[i] = gradient;
			}

		}

		private float Drop(float t) {
			t = Math.Abs(t);
			return 1f - t * t * t * (t * (t * 6 - 15) + 10);
		}
		private float Q(float u, float v) {
			return Drop(u) * Drop(v);
		}
	}
}
