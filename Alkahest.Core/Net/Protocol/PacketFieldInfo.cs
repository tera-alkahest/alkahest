using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol
{
    public abstract class PacketFieldInfo
    {
        public PropertyInfo Property { get; }

        public PacketFieldAttribute Attribute { get; }

        public bool IsArray { get; }

        public bool IsByteArray { get; }

        public bool IsString { get; }

        public bool IsPrimitive { get; }

        protected PacketFieldInfo(PropertyInfo property,
            PacketFieldAttribute attribute)
        {
            Property = property;
            Attribute = attribute;

            var type = property.PropertyType;
            var isArray = type.IsConstructedGenericType &&
                type.GetGenericTypeDefinition() == typeof(List<>);
            var elemType = isArray ? type.GetGenericArguments()[0] : null;
            var isByteArray = elemType == typeof(byte);

            IsArray = isArray && !isByteArray;
            IsByteArray = isArray && isByteArray;
            IsString = type == typeof(string);
            IsPrimitive = IsPrimitiveType(type);
        }

        static bool IsPrimitiveType(Type type)
        {
            if (type.IsEnum)
                type = type.GetEnumUnderlyingType();

            return type.IsPrimitive ||
                type == typeof(Vector3) ||
                type == typeof(EntityId) ||
                type == typeof(SkillId) ||
                type == typeof(Angle) ||
                type == typeof(ModelId);
        }
    }
}
