using System;


namespace RectUI.JSON
{
    public class EnumSerializationAttribute : Attribute
    {
        public EnumSerializationType EnumSerializationType;

        public EnumSerializationAttribute(EnumSerializationType serializationType)
        {
            EnumSerializationType = serializationType;
        }
    }
}