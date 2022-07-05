using System;

namespace GKit.Json;

public class GSerializeSettings {
    public FieldToJTokenDelegate memberHandler = null;
    public FieldHandlerDelegate preHandler = null;
    public Type requiredAttributeType;
}