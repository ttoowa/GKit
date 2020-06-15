#if OnUnity
using UnityEngine;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public static class GRandom {
		private static System.Random systemRandom;
		public static int RandomInt {
			get {
				return systemRandom.Next();
			}
		}
		public static float RandomFloat {
			get {
				return RandomInt + Value;
			}
		}
		public static float Value {
			get {
				return (float)systemRandom.NextDouble();
			}
		}

		static GRandom() {
			systemRandom = new System.Random();
		}
		public static int Range(int minInclusive, int maxInclusive) {
			return systemRandom.Next(minInclusive, maxInclusive + 1);
		}
		public static float Range(float minInclusive, float maxInclusive) {
			float range = maxInclusive - minInclusive;
			return (float)(systemRandom.NextDouble() * range) + minInclusive;
		}

		public static float RandomGauss() {
			float u1 = (float)systemRandom.NextDouble();
			float u2 = (float)systemRandom.NextDouble();
			float temp1 = Mathf.Sqrt(-2.0f * Mathf.Log(u1));
			float temp2 = 2.0f * Mathf.PI * u2;

			float _nextGauss = temp1 * Mathf.Sin(temp2);
			return temp1 * Mathf.Cos(temp2);
		}
		public static float RandomGauss01(float mu = 0f, float sigma = 1f) {
			return Mathf.Clamp01(RandomGauss(mu, sigma) / Mathf.PI);
		}
		public static float RandomGauss(float mu = 0f, float sigma = 1f) {
			float x1 = 1 - (float)systemRandom.NextDouble();
			float x2 = 1 - (float)systemRandom.NextDouble();

			float y1 = Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Cos(2.0f * Mathf.PI * x2);
			return y1 * sigma + mu;
		}
		public static float RandomGauss(float sigma = 1f) {
			return sigma * RandomGauss();
		}
	}
}
