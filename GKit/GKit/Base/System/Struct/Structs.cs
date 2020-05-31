using System;
using System.Globalization;
using System.Windows;
using System.Runtime.InteropServices;
#if OnUnity
using UnityEngine;
#endif

namespace GKit {
	public enum Direction {
		Top,
		Bottom,
		Left,
		Right,
	}
	public enum Speed {
		Fast,
		Normal,
		Slow,
	}
	public enum DepthOrder {
		Bottom,
		Current,
		Top,
	}

	public struct RangeInt {
		public int min;
		public int max;
		public int Length {
			get {
				return max - min;
			}
		}
		public float LengthInvert {
			get {
				return 1f / Length;
			}
		}

		public RangeInt(int min, int max) {
			this.min = min;
			this.max = max;
		}
		public override string ToString() {
			return min.ToString("(0.000") + max.ToString(", 0.000)");
		}
	}
	public struct Range {
		public float min;
		public float max;
		public float Length {
			get {
				return max - min;
			}
		}
		public float LengthInvert {
			get {
				return 1f / Length;
			}
		}

		public Range(float min, float max) {
			this.min = min;
			this.max = max;
		}

		public override string ToString() {
			return min.ToString("(0.000") + max.ToString(", 0.000)");
		}
	}
	public struct BRect {
		public float xMin;
		public float yMin;
		public float xMax;
		public float yMax;
		public float SumX {
			get {
				return xMin + xMax;
			}
		}
		public float SumY {
			get {
				return yMax + yMin;
			}
		}
		public float Width {
			get {
				return xMax - xMin;
			}
		}
		public float Height {
			get {
				return yMax - yMin;
			}
		}
		public Vector2 Size {
			get {
				return new Vector2(Width, Height);
			}
		}
		public float Area {
			get {
				return Width * Height;
			}
		}

		public BRect(float xyMinMax) : this(xyMinMax, xyMinMax) {

		}
		public BRect(float xMinMax, float yMinMax) : this(xMinMax, yMinMax, xMinMax, yMinMax) {
		}
		public BRect(float xMin, float yMin, float xMax, float yMax) {
			this.xMin = xMin;
			this.yMin = yMin;
			this.xMax = xMax;
			this.yMax = yMax;
		}
		public static explicit operator BRectInt(BRect rect) {
			return new BRectInt((int)rect.xMin, (int)rect.yMin, (int)rect.xMax, (int)rect.yMax);
		}

		public void Extend(BRect source) {
			if (xMin > source.xMin) {
				xMin = source.xMin;
			}
			if (xMax < source.xMax) {
				xMax = source.xMax;
			}
			if (yMax < source.yMax) {
				yMax = source.yMax;
			}
			if (yMin > source.yMin) {
				yMin = source.yMin;
			}
		}
		public void Reduce(BRect source) {
			if (xMin < source.xMin) {
				xMin = source.xMin;
			}
			if (xMax > source.xMax) {
				xMax = source.xMax;
			}
			if (yMax > source.yMax) {
				yMax = source.yMax;
			}
			if (yMin < source.yMin) {
				yMin = source.yMin;
			}
		}

		public override string ToString() {
			const string Format = "0.0";
			return "(" + xMin.ToString(Format) + ", " + xMax.ToString(Format) + "), (" + yMin.ToString(Format) + ", " + yMax.ToString(Format) + ")";
		}
	}
	public struct BRectInt {
		public int xMin;
		public int yMin;
		public int xMax;
		public int yMax;
		public int SumX {
			get {
				return xMin + xMax;
			}
		}
		public int SumY {
			get {
				return yMax + yMin;
			}
		}
		public int Width {
			get {
				return xMax - xMin;
			}
		}
		public int Height {
			get {
				return yMax - yMin;
			}
		}
		public Vector2Int Size {
			get {
				return new Vector2Int(Width, Height);
			}
		}
		public int Area {
			get {
				return Width * Height;
			}
		}

		public BRectInt(int xyMinMax) : this(xyMinMax, xyMinMax) {

		}
		public BRectInt(int xMinMax, int yMinMax) : this(xMinMax, yMinMax, xMinMax, yMinMax) {
		}
		public BRectInt(int xMin, int yMin, int xMax, int yMax) {
			this.xMin = xMin;
			this.yMin = yMin;
			this.xMax = xMax;
			this.yMax = yMax;
		}
		public static implicit operator BRect(BRectInt rect) {
			return new BRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax);
		}

		public void Extend(BRectInt source) {
			if (xMin > source.xMin) {
				xMin = source.xMin;
			}
			if (xMax < source.xMax) {
				xMax = source.xMax;
			}
			if (yMax < source.yMax) {
				yMax = source.yMax;
			}
			if (yMin > source.yMin) {
				yMin = source.yMin;
			}
		}
		public void Reduce(BRectInt source) {
			if (xMin < source.xMin) {
				xMin = source.xMin;
			}
			if (xMax > source.xMax) {
				xMax = source.xMax;
			}
			if (yMax > source.yMax) {
				yMax = source.yMax;
			}
			if (yMin < source.yMin) {
				yMin = source.yMin;
			}
		}

		public override string ToString() {
			return "(" + xMin.ToString() + ", " + xMax.ToString() + "), (" + yMin.ToString() + ", " + yMax.ToString() + ")";
		}
	}
#if !OnUnity
	public struct Vector2 {
		public float x, y;

		public Vector2(float x, float y) {
			this.x = x;
			this.y = y;
		}
		public float magnitude {
			get {
				return (float)Math.Sqrt(x * x + y * y);
			}
		}
		public float sqrMagnitude {
			get {
				return x * x + y * y;
			}
		}

		/// <summary>
		///우측 하단이 양수인 좌표계를 기준으로 범위 내에 있는지 검사합니다.
		/// </summary>
		public bool CheckOverlap(float left, float right, float top, float bottom) {
			return x < right && x > left && y > top && y < bottom;
		}
		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode() << 2;
		}
		public override bool Equals(object obj) {
			if (obj is Vector2) {
				return (Vector2)obj == this;
			} else {
				return obj.GetHashCode() == this.GetHashCode();
			}
		}
		public override string ToString() {
			return x.ToString("0.000") + ", " + y.ToString("0.000");
		}
		public static Vector2 Parse(string text) {
			text = text.Trim();
			string[] nums = text.Split(',');

			return new Vector2(float.Parse(nums[0], CultureInfo.InvariantCulture), float.Parse(nums[1], CultureInfo.InvariantCulture));
		}

		public Vector2 Normalized {
			get {
				float lengthInv = 1f / (float)Math.Sqrt(x * x + y * y);
				return new Vector2(x * lengthInv, y * lengthInv);
			}
		}


		public static bool operator ==(Vector2 left, Vector2 right) {
			return left.x == right.x && left.y == right.y;
		}
		public static bool operator !=(Vector2 left, Vector2 right) {
			return left.x != right.x || left.y != right.y;
		}

		public static Vector2 operator +(Vector2 left, Vector2 right) {
			return new Vector2(left.x + right.x, left.y + right.y);
		}
		public static Vector2 operator -(Vector2 left, Vector2 right) {
			return new Vector2(left.x - right.x, left.y - right.y);
		}
		public static Vector2 operator -(Vector2 vector2) {
			return new Vector2(-vector2.x, -vector2.y);
		}

		public static Vector2 operator *(Vector2 left, Vector2 right) {
			return new Vector2(left.x * right.x, left.y * right.y);
		}
		public static Vector2 operator *(Vector2 left, int right) {
			return new Vector2(left.x * right, left.y * right);
		}
		public static Vector2 operator *(int left, Vector2 right) {
			return right * left;
		}
		public static Vector2 operator *(Vector2 left, float right) {
			return new Vector2(left.x * right, left.y * right);
		}
		public static Vector2 operator *(float left, Vector2 right) {
			return right * left;
		}
		public static Vector2 operator *(Vector2 left, double right) {
			float rightF = (float)right;
			return new Vector2(left.x * rightF, left.y * rightF);
		}
		public static Vector2 operator *(double left, Vector2 right) {
			return right * left;
		}

		public static Vector2 operator /(Vector2 left, Vector2 right) {
			return new Vector2(left.x / right.x, left.y / right.y);
		}
		public static Vector2 operator /(Vector2 left, int right) {
			return new Vector2(left.x / right, left.y / right);
		}
		public static Vector2 operator /(Vector2 left, float right) {
			return new Vector2(left.x / right, left.y / right);
		}
		public static Vector2 operator /(Vector2 left, double right) {
			float rightF = (float)right;
			return new Vector2(left.x / rightF, left.y / rightF);
		}
		public static explicit operator Vector2Int(Vector2 vector2) {
			return new Vector2Int((int)vector2.x, (int)vector2.y);
		}
		public static explicit operator System.Drawing.Point(Vector2 vector2) {
			return new System.Drawing.Point((int)vector2.x, (int)vector2.y);
		}
		public static explicit operator Point(Vector2 vector2) {
			return new Point(vector2.x, vector2.y);
		}
		public static explicit operator Vector2(System.Drawing.Point point) {
			return new Vector2(point.X, point.Y);
		}
		public static explicit operator Vector2(Point point) {
			return new Vector2((float)point.X, (float)point.Y);
		}
	}
	public struct Vector2Int {
		public int x, y;

		public Vector2Int(int x, int y) {
			this.x = x;
			this.y = y;
		}
		public double magnitude {
			get {
				return Math.Sqrt(x * x + y * y);
			}
		}
		public float sqrMagnitude {
			get {
				return x * x + y * y;
			}
		}
		public override int GetHashCode() {
			return x.GetHashCode() ^ y.GetHashCode() << 2;
		}
		public override bool Equals(object obj) {
			if (obj is Vector2Int) {
				return (Vector2Int)obj == this;
			} else {
				return obj.GetHashCode() == this.GetHashCode();
			}
		}
		public override string ToString() {
			return x.ToString("0.000") + ", " + y.ToString("0.000");
		}
		public Vector2 normalized {
			get {
				float lengthInv = 1f / (float)Math.Sqrt(x * x + y * y);
				return new Vector2(x * lengthInv, y * lengthInv);
			}
		}


		public static bool operator ==(Vector2Int left, Vector2Int right) {
			return left.x == right.x && left.y == right.y;
		}
		public static bool operator !=(Vector2Int left, Vector2Int right) {
			return !(left.x == right.x && left.y == right.y);
		}

		public static Vector2Int operator +(Vector2Int left, Vector2Int right) {
			return new Vector2Int(left.x + right.x, left.y + right.y);
		}
		public static Vector2Int operator -(Vector2Int left, Vector2Int right) {
			return new Vector2Int(left.x - right.x, left.y - right.y);
		}
		public static Vector2Int operator -(Vector2Int vector2) {
			return new Vector2Int(-vector2.x, -vector2.y);
		}

		public static Vector2Int operator *(Vector2Int left, Vector2Int right) {
			return new Vector2Int(left.x * right.x, left.y * right.y);
		}
		public static Vector2Int operator *(Vector2Int left, int right) {
			return new Vector2Int(left.x * right, left.y * right);
		}
		public static Vector2Int operator *(int left, Vector2Int right) {
			return right * left;
		}
		public static Vector2 operator *(Vector2Int left, float right) {
			return new Vector2(left.x * right, left.y * right);
		}
		public static Vector2 operator *(float left, Vector2Int right) {
			return right * left;
		}
		public static Vector2 operator *(Vector2Int left, double right) {
			float rightF = (float)right;
			return new Vector2(left.x * rightF, left.y * rightF);
		}
		public static Vector2 operator *(double left, Vector2Int right) {
			return right * left;
		}

		public static Vector2Int operator /(Vector2Int left, Vector2Int right) {
			return new Vector2Int(left.x / right.x, left.y / right.y);
		}
		public static Vector2Int operator /(Vector2Int left, int right) {
			return new Vector2Int(left.x / right, left.y / right);
		}
		public static Vector2 operator /(Vector2Int left, float right) {
			return new Vector2(left.x / right, left.y / right);
		}
		public static Vector2 operator /(Vector2Int left, double right) {
			float rightF = (float)right;
			return new Vector2(left.x / rightF, left.y / rightF);
		}
		public static implicit operator Vector2(Vector2Int vector2Int) {
			return new Vector2(vector2Int.x, vector2Int.y);
		}
		public static explicit operator System.Drawing.Point(Vector2Int vector2) {
			return new System.Drawing.Point((int)vector2.x, (int)vector2.y);
		}
		public static explicit operator Point(Vector2Int vector2) {
			return new Point(vector2.x, vector2.y);
		}
		public static explicit operator Vector2Int(System.Drawing.Point point) {
			return new Vector2Int(point.X, point.Y);
		}
		public static explicit operator Vector2Int(Point point) {
			return new Vector2Int((int)point.X, (int)point.Y);
		}
	}
	public struct Vector3 {
		public float x;
		public float y;
		public float z;

		public Vector3(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString() {
			return x.ToString("0.000") + ", " + y.ToString("0.000") + ", " + z.ToString("0.000");
		}
		public override bool Equals(object other) {
			return other is Vector3 && this.Equals((Vector3)other);
		}
		public override int GetHashCode() {
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
		}

		public static bool operator ==(Vector3 left, Vector3 right) {
			return left.x == right.x && left.y == right.y && left.z == right.z;
		}
		public static bool operator !=(Vector3 left, Vector3 right) {
			return left.x != right.x || left.y != right.y || left.z != right.z;
		}
		public static Vector3 operator +(Vector3 left, Vector3 right) {
			return new Vector3(left.x + right.x, left.y + right.y, left.z + right.z);
		}
		public static Vector3 operator -(Vector3 left, Vector3 right) {
			return new Vector3(left.x - right.x, left.y - right.y, left.z - right.z);
		}
		public static Vector3 operator -(Vector3 vector) {
			return new Vector3(-vector.x, -vector.y, -vector.z);
		}

		public static Vector3 operator *(Vector3 left, Vector3 right) {
			return new Vector3(left.x * right.x, left.y * right.y, left.z * right.z);
		}
		public static Vector3 operator *(Vector3 left, int right) {
			return new Vector3(left.x * right, left.y * right, left.z * right);
		}
		public static Vector3 operator *(int left, Vector3 right) {
			return right * left;
		}
		public static Vector3 operator *(Vector3 left, float right) {
			return new Vector3(left.x * right, left.y * right, left.z * right);
		}
		public static Vector3 operator *(float left, Vector3 right) {
			return right * left;
		}
		public static Vector3 operator *(Vector3 left, double right) {
			float rightF = (float)right;
			return new Vector3(left.x * rightF, left.y * rightF, left.z * rightF);
		}
		public static Vector3 operator *(double left, Vector3 right) {
			return right * left;
		}

		public static Vector3 operator /(Vector3 left, Vector3 right) {
			return new Vector3(left.x / right.x, left.y / right.y, left.z / right.z);
		}
		public static Vector3 operator /(Vector3 left, int right) {
			return new Vector3(left.x / right, left.y / right, left.z / right);
		}
		public static Vector3 operator /(Vector3 left, float right) {
			return new Vector3(left.x / right, left.y / right, left.z / right);
		}
		public static Vector3 operator /(Vector3 left, double right) {
			float rightF = (float)right;
			return new Vector3(left.x / rightF, left.y / rightF, left.z / rightF);
		}
	}
	public struct Vector3Int {
		public int x;
		public int y;
		public int z;

		public Vector3Int(int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static Vector3Int operator +(Vector3Int a, Vector3Int b) {
			return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Vector3Int operator -(Vector3Int a, Vector3Int b) {
			return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static Vector3Int operator *(Vector3Int a, Vector3Int b) {
			return new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector3Int operator *(Vector3Int a, int b) {
			return new Vector3Int(a.x * b, a.y * b, a.z * b);
		}

		public static bool operator ==(Vector3Int lhs, Vector3Int rhs) {
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}

		public static bool operator !=(Vector3Int lhs, Vector3Int rhs) {
			return !(lhs == rhs);
		}
		public override bool Equals(object other) {
			return other is Vector3Int && this.Equals((Vector3Int)other);
		}

		public bool Equals(Vector3Int other) {
			return this == other;
		}

		public override int GetHashCode() {
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
		}

		public override string ToString() {
			return x.ToString() + ", " + y.ToString() + ", " + z.ToString();
		}
	}
	public struct Vector4 {
		public const float kEpsilon = 1E-05f;
		private static readonly Vector4 zeroVector = new Vector4(0f, 0f, 0f, 0f);
		private static readonly Vector4 oneVector = new Vector4(1f, 1f, 1f, 1f);

		public float x;
		public float y;
		public float z;
		public float w;
		public Vector4 normalized {
			get {
				return Vector4.Normalize(this);
			}
		}
		public float magnitude {
			get {
				return Mathf.Sqrt(Vector4.Dot(this, this));
			}
		}
		public float sqrMagnitude {
			get {
				return Vector4.Dot(this, this);
			}
		}

		public static Vector4 zero {
			get {
				return Vector4.zeroVector;
			}
		}
		public static Vector4 one {
			get {
				return Vector4.oneVector;
			}
		}

		public Vector4(float x, float y, float z, float w) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public Vector4(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = 0f;
		}
		public Vector4(float x, float y) {
			this.x = x;
			this.y = y;
			this.z = 0f;
			this.w = 0f;
		}
		public void Set(float newX, float newY, float newZ, float newW) {
			this.x = newX;
			this.y = newY;
			this.z = newZ;
			this.w = newW;
		}

		public static Vector4 Lerp(Vector4 a, Vector4 b, float t) {
			t = Mathf.Clamp01(t);
			return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
		}
		public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, float t) {
			return new Vector4(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t, a.w + (b.w - a.w) * t);
		}
		public static Vector4 MoveTowards(Vector4 current, Vector4 target, float maxDistanceDelta) {
			Vector4 a = target - current;
			float magnitude = a.magnitude;
			Vector4 result;
			if (magnitude <= maxDistanceDelta || magnitude == 0f) {
				result = target;
			} else {
				result = current + a / magnitude * maxDistanceDelta;
			}
			return result;
		}
		public static Vector4 Scale(Vector4 a, Vector4 b) {
			return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}
		public void Scale(Vector4 scale) {
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
			this.w *= scale.w;
		}

		public override int GetHashCode() {
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
		}
		public override bool Equals(object other) {
			return other is Vector4 && this.Equals((Vector4)other);
		}
		public bool Equals(Vector4 other) {
			return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
		}
		public static Vector4 Normalize(Vector4 a) {
			float num = Vector4.Magnitude(a);
			Vector4 result;
			if (num > 1E-05f) {
				result = a / num;
			} else {
				result = Vector4.zero;
			}
			return result;
		}

		public void Normalize() {
			float num = Vector4.Magnitude(this);
			if (num > 1E-05f) {
				this /= num;
			} else {
				this = Vector4.zero;
			}
		}

		public static float Dot(Vector4 a, Vector4 b) {
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		public static Vector4 Project(Vector4 a, Vector4 b) {
			return b * Vector4.Dot(a, b) / Vector4.Dot(b, b);
		}

		public static float Distance(Vector4 a, Vector4 b) {
			return Vector4.Magnitude(a - b);
		}

		public static float Magnitude(Vector4 a) {
			return Mathf.Sqrt(Vector4.Dot(a, a));
		}

		public static Vector4 Min(Vector4 lhs, Vector4 rhs) {
			return new Vector4(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z), Mathf.Min(lhs.w, rhs.w));
		}

		public static Vector4 Max(Vector4 lhs, Vector4 rhs) {
			return new Vector4(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z), Mathf.Max(lhs.w, rhs.w));
		}

		public static Vector4 operator +(Vector4 a, Vector4 b) {
			return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vector4 operator -(Vector4 a, Vector4 b) {
			return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Vector4 operator -(Vector4 a) {
			return new Vector4(-a.x, -a.y, -a.z, -a.w);
		}

		public static Vector4 operator *(Vector4 a, float d) {
			return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		public static Vector4 operator *(float d, Vector4 a) {
			return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		public static Vector4 operator /(Vector4 a, float d) {
			return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
		}

		public static bool operator ==(Vector4 lhs, Vector4 rhs) {
			return Vector4.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
		}

		public static bool operator !=(Vector4 lhs, Vector4 rhs) {
			return !(lhs == rhs);
		}

		public static implicit operator Vector4(Vector3 v) {
			return new Vector4(v.x, v.y, v.z, 0f);
		}

		public static implicit operator Vector3(Vector4 v) {
			return new Vector3(v.x, v.y, v.z);
		}

		public static implicit operator Vector4(Vector2 v) {
			return new Vector4(v.x, v.y, 0f, 0f);
		}

		public static implicit operator Vector2(Vector4 v) {
			return new Vector2(v.x, v.y);
		}

		public override string ToString() {
			return x.ToString("0.000") + ", " + y.ToString("0.000") + ", " + z.ToString("0.000") + ", " + w.ToString("0.000");
		}

		public static float SqrMagnitude(Vector4 a) {
			return Vector4.Dot(a, a);
		}

		public float SqrMagnitude() {
			return Vector4.Dot(this, this);
		}
	}
	public struct IntSize {
		public int width, height;

		public IntSize(int width, int height) {
			this.width = width;
			this.height = height;
		}
		public static explicit operator IntSize(Size size) {
			return new IntSize((int)size.Width, (int)size.Height);
		}
		public static explicit operator Size(IntSize intSize) {
			return new Size(intSize.width, intSize.height);
		}
		public static explicit operator IntSize(IntPoint intPoint) {
			return new IntSize(intPoint.x, intPoint.y);
		}
		public static IntSize operator -(IntSize left, IntSize right) {
			return new IntSize(left.width - right.width, left.height - right.height);
		}
		public static IntSize operator +(IntSize left, IntSize right) {
			return new IntSize(left.width + right.width, left.height + right.height);
		}
		public static IntSize operator -(IntSize intSize) {
			return new IntSize(-intSize.width, -intSize.height);
		}
	}
	public struct IntPoint {
		public int x, y;

		public IntPoint(int x, int y) {
			this.x = x;
			this.y = y;
		}
		public double Length {
			get {
				return Math.Sqrt(x * x + y * y);
			}
		}
		public static explicit operator IntPoint(Point point) {
			return new IntPoint((int)point.X, (int)point.Y);
		}
		public static explicit operator IntPoint(IntSize intSize) {
			return new IntPoint(intSize.width, intSize.height);
		}
		public static explicit operator IntPoint(Size size) {
			return new IntPoint((int)size.Width, (int)size.Height);
		}
		public static explicit operator Point(IntPoint intPoint) {
			return new Point(intPoint.x, intPoint.y);
		}
		public static explicit operator Size(IntPoint intPoint) {
			return new Size(intPoint.x, intPoint.y);
		}
		public static IntPoint operator -(IntPoint left, IntPoint right) {
			return new IntPoint(left.x - right.x, left.y - right.y);
		}
		public static IntPoint operator +(IntPoint left, IntPoint right) {
			return new IntPoint(left.x + right.x, left.y + right.y);
		}
		public static IntPoint operator -(IntPoint intPoint) {
			return new IntPoint(-intPoint.x, -intPoint.y);
		}
	}
#endif
}
