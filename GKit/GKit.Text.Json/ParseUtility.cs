using System.Text.Json;

namespace GKit.Text.Json;

public static class ParseUtility {
    public static int GetInt32(this JsonElement jsonElement, string propertyName, int defaultValue) {
        JsonElement element = jsonElement.GetProperty(propertyName);
        if (element.ValueKind == JsonValueKind.Number) {
            return element.GetInt32();
        } else {
            return defaultValue;
        }
    }
    
    public static float GetSingle(this JsonElement jsonElement, string propertyName, float defaultValue) {
        JsonElement element = jsonElement.GetProperty(propertyName);
        if (element.ValueKind == JsonValueKind.Number) {
            return element.GetSingle();
        } else {
            return defaultValue;
        }
    }
    
    public static double GetDouble(this JsonElement jsonElement, string propertyName, double defaultValue) {
        JsonElement element = jsonElement.GetProperty(propertyName);
        if (element.ValueKind == JsonValueKind.Number) {
            return element.GetDouble();
        } else {
            return defaultValue;
        }
    }
    
    public static string GetString(this JsonElement jsonElement, string propertyName, string defaultValue) {
        JsonElement element = jsonElement.GetProperty(propertyName);
        if (element.ValueKind == JsonValueKind.String) {
            return element.GetString();
        } else {
            return defaultValue;
        }
    }
    
    public static bool GetBoolean(this JsonElement jsonElement, string propertyName, bool defaultValue) {
        JsonElement element = jsonElement.GetProperty(propertyName);
        if (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False) {
            return element.GetBoolean();
        } else {
            return defaultValue;
        }
    }
}