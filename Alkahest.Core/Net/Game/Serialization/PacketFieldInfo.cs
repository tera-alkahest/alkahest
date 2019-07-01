using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

namespace Alkahest.Core.Net.Game.Serialization
{
    public abstract class PacketFieldInfo
    {
        public PropertyInfo Property { get; }

        public PacketFieldOptionsAttribute Attribute { get; }

        public bool IsArray { get; }

        public bool IsByteArray { get; }

        public bool IsString { get; }

        public bool IsPrimitive { get; }

        protected PacketFieldInfo(PropertyInfo property, PacketFieldOptionsAttribute attribute)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Attribute = attribute;

            var type = property.PropertyType;

            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(NoNullList<>))
                IsArray = true;
            else if (type == typeof(List<byte>))
                IsByteArray = true;
            else if (type == typeof(string))
                IsString = true;
            else
            {
                if (type.IsEnum)
                    type = type.GetEnumUnderlyingType();

                if (type.IsPrimitive ||
                    type == typeof(Vector3) ||
                    type == typeof(GameId) ||
                    type == typeof(SkillId) ||
                    type == typeof(Angle) ||
                    type == typeof(TemplateId) ||
                    type == typeof(Appearance))
                    IsPrimitive = true;
                else
                    throw Assert.Unreachable();
            }
        }
    }
}
