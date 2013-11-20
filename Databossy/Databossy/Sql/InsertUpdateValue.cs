using System;

namespace Databossy.Sql
{
    public class InsertUpdateValue
    {
        public String FieldName { get; private set; }
        public String Value { get; private set; }
        public CommonDataType.Type ValueType { get; private set; }

        public InsertUpdateValue(String fieldName, Object value, CommonDataType.Type valueType)
        {
            FieldName = fieldName;
            ValueType = valueType;
            Value = CommonDataType.GetValue(value, valueType);
        }
    }
}