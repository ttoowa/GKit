using System;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
#if OnUnity
using UnityEngine;
using ColorB = UnityEngine.Color32;
using ColorF = UnityEngine.Color;
#else
using System.Windows;
using System.Windows.Media;
using ColorB = System.Windows.Media.Color;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Graphics {
	public static class ColorUtility {
		public static string ToHex(this ColorB color) {
#if OnUnity
			string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
#else
			string hex = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
#endif
			return hex;
		}

#if OnUnity
		public static ColorF Add(this ColorF color, float value) {
			return new ColorF(
				Mathf.Clamp01(color.r + value),
				Mathf.Clamp01(color.g + value),
				Mathf.Clamp01(color.b + value),
				color.a);
		}
		public static ColorB Add(this ColorB color, float value) {
			byte byteValue = (byte)(value * 255f);
#if OnUnity
			return new ColorB(
				(byte)(Mathf.Clamp((int)color.r + byteValue, 0, 255)),
				(byte)(Mathf.Clamp((int)color.g + byteValue, 0, 255)),
				(byte)(Mathf.Clamp((int)color.b + byteValue, 0, 255)),
				color.a);
#else
			return ColorB.FromArgb(
				color.A,
				(byte)(Mathf.Clamp((int)color.R + byteValue, 0, 255)),
				(byte)(Mathf.Clamp((int)color.G + byteValue, 0, 255)),
				(byte)(Mathf.Clamp((int)color.B + byteValue, 0, 255)));
#endif
		}
		public static ColorB Add(this ColorB color, byte value) {
#if OnUnity
			return new ColorB(
				(byte)(Mathf.Clamp((int)color.r + value, 0, 255)),
				(byte)(Mathf.Clamp((int)color.g + value, 0, 255)),
				(byte)(Mathf.Clamp((int)color.b + value, 0, 255)),
				color.a);
#else
			return ColorB.FromArgb(
				color.A,
				(byte)(Mathf.Clamp((int)color.R + value, 0, 255)),
				(byte)(Mathf.Clamp((int)color.G + value, 0, 255)),
				(byte)(Mathf.Clamp((int)color.B + value, 0, 255)));
#endif
		}
#endif
		public static ColorB ToColor(this string hex) {
			hex = hex.Replace("0x", "");
			hex = hex.Replace("#", "");

			byte a = 255;
			byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

			if (hex.Length == 8) {
				a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
			}
#if OnUnity
			return new ColorB(r, g, b, a);
#else
			return Color.FromArgb(a, r, g, b);
#endif
		}
		public static ColorB ToColor(this HSV hsv) {
			hsv.hue = GMath.Clamp(hsv.hue, 0f, 360f);
			hsv.saturation = GMath.Clamp(hsv.saturation, 0f, 1f);
			hsv.value = GMath.Clamp(hsv.value, 0f, 1f);

			float hD60 = hsv.hue / 60;
			double f = hD60 - Math.Floor(hD60);

			hsv.value = hsv.value * 255;
			byte v = (byte)(hsv.value);
			byte p = (byte)(hsv.value * (1 - hsv.saturation));
			byte q = (byte)(hsv.value * (1 - f * hsv.saturation));
			byte t = (byte)(hsv.value * (1 - (1 - f) * hsv.saturation));

			int hi = (int)(Math.Floor(hD60)) % 6;
#if OnUnity
			if (hi == 0)
				return new ColorB(v, t, p, 255);
			else if (hi == 1)
				return new ColorB(q, v, p, 255);
			else if (hi == 2)
				return new ColorB(p, v, t, 255);
			else if (hi == 3)
				return new ColorB(p, q, v, 255);
			else if (hi == 4)
				return new ColorB(t, p, v, 255);
			else
				return new ColorB(v, p, q, 255);
#else
			if (hi == 0)
				return Color.FromArgb(255, v, t, p);
			else if (hi == 1)
				return Color.FromArgb(255, q, v, p);
			else if (hi == 2)
				return Color.FromArgb(255, p, v, t);
			else if (hi == 3)
				return Color.FromArgb(255, p, q, v);
			else if (hi == 4)
				return Color.FromArgb(255, t, p, v);
			else
				return Color.FromArgb(255, v, p, q);
#endif
		}
		public static HSV ToHSV(this ColorB color) {
			float hue, saturation, value;

#if OnUnity
			int max = Math.Max(color.r, Math.Max(color.g, color.b));
			int min = Math.Min(color.r, Math.Min(color.g, color.b));
			float delta = max - min;
			if (delta == 0) {
				hue = 0;
				saturation = 0;
			} else {
				saturation = 1f - (1f * min / max);
				if (Mathf.Abs(delta) < float.Epsilon) {
					delta = float.Epsilon;
				}
				if (color.r == max) {
					hue = (color.g - color.b) / delta;
				} else if (color.g == max) {
					hue = 2 + (color.b - color.r) / delta;
				} else {
					hue = 4 + (color.r - color.g) / delta;
				}
				hue *= 60;
				if (hue < 0) {
					hue += 360;
				}
			}
#else
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));
			if (max == 0) {
				hue = 0;
				saturation = 0;
			} else {
				saturation = 1f - (1f * min / max);
				float delta = max - min;
				if (Mathf.Abs(delta) < float.Epsilon) {
					delta = float.Epsilon;
				}
				if (color.R == max) {
					hue = (color.G - color.B) / delta;
				} else if (color.G == max) {
					hue = 2 + (color.B - color.R) / delta;
				} else {
					hue = 4 + (color.R - color.G) / delta;
				}
				hue *= 60;
				if (hue < 0) {
					hue += 360;
				}
			}
#endif
			value = max / 255f;

			return new HSV(hue, saturation, value);
		}
#if OnWPF
		public static SolidColorBrush ToBrush(this string hex) {
			return hex.ToColor().ToBrush();
		}
		public static SolidColorBrush ToBrush(this Color color) {
			return new SolidColorBrush(color);
		}
#endif
		public static float GetHashFloat(this string text) {
			const float _765To1 = 0.001307f;
			MD5 md5 = MD5.Create();
			byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
			if (hash.Length < 3) {
				if (hash.Length == 0) {
					hash = new byte[] { 0, 0, 0 };
				} else if (hash.Length == 1) {
					hash = new byte[] { hash[0], 0, 0 };
				} else if (hash.Length == 2) {
					hash = new byte[] { hash[0], hash[1], 0 };
				}
			}
			return ((float)hash[0] + hash[1] + hash[2]) * _765To1;
		}
		public static ColorB GetHashColor(this string Strtextng, float saturation = 0.5f, float value = 0.9f) {
			const float _765To360 = 0.470588f;
			MD5 md5 = MD5.Create();
			byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Strtextng));
			if (hash.Length < 3) {
				if (hash.Length == 0) {
					hash = new byte[] { 0, 0, 0 };
				} else if (hash.Length == 1) {
					hash = new byte[] { hash[0], 0, 0 };
				} else if (hash.Length == 2) {
					hash = new byte[] { hash[0], hash[1], 0 };
				}
			}
			float hue = ((float)hash[0] + hash[1] + hash[2]) * _765To360;
			return ToColor(new HSV(hue, saturation, value));
		}
	}
}
