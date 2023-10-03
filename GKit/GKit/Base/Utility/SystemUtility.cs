using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
#if OnUnity
using UnityEngine;
using Debug = UnityEngine.Debug;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public static class SystemExtension {
        public static float GetElapsedMilliseconds(this Stopwatch stopwatch) {
            return stopwatch.ElapsedTicks / (float)Stopwatch.Frequency * 1000f;
        }

        /// <summary>
        ///     함수를 호출하며 예외를 검사합니다.
        /// </summary>
        public static bool TryInvoke(this Action action) {
            try {
                action?.Invoke();
                return true;
            } catch (Exception ex) {
                GDebug.Log(ex.ToString(), GLogLevel.Warnning);
                return false;
            }
        }

        public static int Parse2Int(this string value, int exceptionValue = 0) {
            int result;
            if (int.TryParse(value, out result))
                return result;
            else
                return 0;
        }

        public static float Parse2Float(this string value, float exceptionValue = 0f) {
            float result;
            if (float.TryParse(value, out result))
                return result;
            else
                return 0;
        }

        public static bool Contained(this float value, float min, float max) {
            return value <= max && value >= min;
        }

        public static bool Contained(this int value, int min, int max) {
            return value <= max && value >= min;
        }

        public static T[] ToArray<T>(this T value) where T : struct {
            return new T[] { value };
        }

        public static List<T> ToList<T>(this T value) where T : struct {
            return new List<T>() { value };
        }

        public static T Cast<T>(this object obj) {
            return (T)obj;
        }

        public static bool HasTrueValue(this bool? value) {
            return value.HasValue && value.Value;
        }

        public static bool IsStruct(this Type type) {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }

        public static bool IsZeroSizeStruct(this Type type) {
            return type.IsStruct() && type.GetFields((BindingFlags)0x34).All(fi => IsZeroSizeStruct(fi.FieldType));
        }

        public static bool IsNumeric(object value) {
            return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float ||
                   value is double || value is decimal;
        }

        public static bool IsNumericType(this Type type) {
            switch (Type.GetTypeCode(type)) {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}