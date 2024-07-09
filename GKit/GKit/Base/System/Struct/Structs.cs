using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using Point = System.Drawing.Point;
#if OnUnity
using UnityEngine;
#endif

#if OnUnity
namespace GKitForUnity;
#elif OnWPF
namespace GKitForWPF;
#else
namespace GKit;
#endif


public enum Direction {
    Top,
    Bottom,
    Left,
    Right
}

public enum Speed {
    Fast,
    Normal,
    Slow
}

public enum DepthOrder {
    Bottom,
    Current,
    Top
}

public struct GRangeInt {
    public int min;
    public int max;
    public int Length => max - min;
    public float LengthInvert => 1f / Length;
    
    public GRangeInt(int min, int max) {
        this.min = min;
        this.max = max;
    }
    
    public override string ToString() {
        return min.ToString("(0.000") + max.ToString(", 0.000)");
    }
}

public struct GRange {
    public float min;
    public float max;
    public float Length => max - min;
    public float LengthInvert => 1f / Length;
    
    public GRange(float min, float max) {
        this.min = min;
        this.max = max;
    }
    
    public override string ToString() {
        return min.ToString("(0.000") + max.ToString(", 0.000)");
    }
}

public struct GRect {
    public float xMin;
    public float yMin;
    public float xMax;
    public float yMax;
    public float SumX => xMin + xMax;
    public float SumY => yMax + yMin;
    
    public float Width => xMax - xMin;
    public float Height => yMax - yMin;
    public Vector2 Size => new(Width, Height);
    
    public float Area => Width * Height;
    
    public GRect(float xyMinMax) : this(xyMinMax, xyMinMax) {
    }
    
    public GRect(float xMinMax, float yMinMax) : this(xMinMax, yMinMax, xMinMax, yMinMax) {
    }
    
    public GRect(float xMin, float yMin, float xMax, float yMax) {
        this.xMin = xMin;
        this.yMin = yMin;
        this.xMax = xMax;
        this.yMax = yMax;
    }
    
    public static explicit operator GRectInt(GRect rect) {
        return new GRectInt((int)rect.xMin, (int)rect.yMin, (int)rect.xMax, (int)rect.yMax);
    }
    
    public GRect Extend(GRect source) {
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
        
        return this;
    }
    
    public GRect Reduce(GRect source) {
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
        
        return this;
    }
    
    public override string ToString() {
        const string Format = "0.0";
        return "(" + xMin.ToString(Format) + ", " + xMax.ToString(Format) + "), (" + yMin.ToString(Format) + ", " + yMax.ToString(Format) + ")";
    }
}

public struct GRectInt {
    public int xMin;
    public int yMin;
    public int xMax;
    public int yMax;
    public int SumX => xMin + xMax;
    
    public int SumY => yMax + yMin;
    public int Width => xMax - xMin;
    
    public int Height => yMax - yMin;
    public Vector2Int Size => new(Width, Height);
    
    public int Area => Width * Height;
    
    public GRectInt(int xyMinMax) : this(xyMinMax, xyMinMax) {
    }
    
    public GRectInt(int xMinMax, int yMinMax) : this(xMinMax, yMinMax, xMinMax, yMinMax) {
    }
    
    public GRectInt(int xMin, int yMin, int xMax, int yMax) {
        this.xMin = xMin;
        this.yMin = yMin;
        this.xMax = xMax;
        this.yMax = yMax;
    }
    
    public static implicit operator GRect(GRectInt rect) {
        return new GRect(rect.xMin, rect.yMin, rect.xMax, rect.yMax);
    }
    
    public GRectInt Extend(GRectInt source) {
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
        
        return this;
    }
    
    public GRectInt Reduce(GRectInt source) {
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
        
        return this;
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
    
    public float magnitude => (float)Math.Sqrt(x * x + y * y);
    
    public float sqrMagnitude => x * x + y * y;
    
    /// <summary>
    ///     우측 하단이 양수인 좌표계를 기준으로 범위 내에 있는지 검사합니다.
    /// </summary>
    public bool CheckOverlap(float left, float right, float top, float bottom) {
        return x < right && x > left && y > top && y < bottom;
    }
    
    public override int GetHashCode() {
        return x.GetHashCode() ^ (y.GetHashCode() << 2);
    }
    
    public override bool Equals(object obj) {
        if (obj is Vector2) {
            return (Vector2)obj == this;
        } else {
            return obj.GetHashCode() == GetHashCode();
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
    
    public static explicit operator Point(Vector2 vector2) {
        return new Point((int)vector2.x, (int)vector2.y);
    }
    
    public static explicit operator System.Windows.Point(Vector2 vector2) {
        return new System.Windows.Point(vector2.x, vector2.y);
    }
    
    public static explicit operator Vector2(Point point) {
        return new Vector2(point.X, point.Y);
    }
    
    public static explicit operator Vector2(System.Windows.Point point) {
        return new Vector2((float)point.X, (float)point.Y);
    }
}

public struct Vector2Int {
    public int x, y;
    
    public Vector2Int(int x, int y) {
        this.x = x;
        this.y = y;
    }
    
    public double magnitude => Math.Sqrt(x * x + y * y);
    public float sqrMagnitude => x * x + y * y;
    
    public override int GetHashCode() {
        return x.GetHashCode() ^ (y.GetHashCode() << 2);
    }
    
    public override bool Equals(object obj) {
        if (obj is Vector2Int) {
            return (Vector2Int)obj == this;
        } else {
            return obj.GetHashCode() == GetHashCode();
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
    
    public static explicit operator Point(Vector2Int vector2) {
        return new Point((int)vector2.x, (int)vector2.y);
    }
    
    public static explicit operator System.Windows.Point(Vector2Int vector2) {
        return new System.Windows.Point(vector2.x, vector2.y);
    }
    
    public static explicit operator Vector2Int(Point point) {
        return new Vector2Int(point.X, point.Y);
    }
    
    public static explicit operator Vector2Int(System.Windows.Point point) {
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
        return other is Vector3 && Equals((Vector3)other);
    }
    
    public override int GetHashCode() {
        return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
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
        return other is Vector3Int && Equals((Vector3Int)other);
    }
    
    public bool Equals(Vector3Int other) {
        return this == other;
    }
    
    public override int GetHashCode() {
        return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
    }
    
    public override string ToString() {
        return x.ToString() + ", " + y.ToString() + ", " + z.ToString();
    }
}

public struct Vector4 {
    public const float kEpsilon = 1E-05f;
    private static readonly Vector4 zeroVector = new(0f, 0f, 0f, 0f);
    private static readonly Vector4 oneVector = new(1f, 1f, 1f, 1f);
    
    public float x;
    public float y;
    public float z;
    public float w;
    public Vector4 normalized => Normalize(this);
    
    public float magnitude => Mathf.Sqrt(Dot(this, this));
    public float sqrMagnitude => Dot(this, this);
    
    public static Vector4 zero => zeroVector;
    
    public static Vector4 one => oneVector;
    
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
        w = 0f;
    }
    
    public Vector4(float x, float y) {
        this.x = x;
        this.y = y;
        z = 0f;
        w = 0f;
    }
    
    public void Set(float newX, float newY, float newZ, float newW) {
        x = newX;
        y = newY;
        z = newZ;
        w = newW;
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
        x *= scale.x;
        y *= scale.y;
        z *= scale.z;
        w *= scale.w;
    }
    
    public override int GetHashCode() {
        return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
    }
    
    public override bool Equals(object other) {
        return other is Vector4 && Equals((Vector4)other);
    }
    
    public bool Equals(Vector4 other) {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
    }
    
    public static Vector4 Normalize(Vector4 a) {
        float num = Magnitude(a);
        Vector4 result;
        if (num > 1E-05f) {
            result = a / num;
        } else {
            result = zero;
        }
        
        return result;
    }
    
    public void Normalize() {
        float num = Magnitude(this);
        if (num > 1E-05f) {
            this /= num;
        } else {
            this = zero;
        }
    }
    
    public static float Dot(Vector4 a, Vector4 b) {
        return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
    }
    
    public static Vector4 Project(Vector4 a, Vector4 b) {
        return b * Dot(a, b) / Dot(b, b);
    }
    
    public static float Distance(Vector4 a, Vector4 b) {
        return Magnitude(a - b);
    }
    
    public static float Magnitude(Vector4 a) {
        return Mathf.Sqrt(Dot(a, a));
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
        return SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
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
        return Dot(a, a);
    }
    
    public float SqrMagnitude() {
        return Dot(this, this);
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
    
    public double Length => Math.Sqrt(x * x + y * y);
    
    public static explicit operator IntPoint(System.Windows.Point point) {
        return new IntPoint((int)point.X, (int)point.Y);
    }
    
    public static explicit operator IntPoint(IntSize intSize) {
        return new IntPoint(intSize.width, intSize.height);
    }
    
    public static explicit operator IntPoint(Size size) {
        return new IntPoint((int)size.Width, (int)size.Height);
    }
    
    public static explicit operator System.Windows.Point(IntPoint intPoint) {
        return new System.Windows.Point(intPoint.x, intPoint.y);
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