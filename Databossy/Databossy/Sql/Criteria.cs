using System;

namespace Databossy.Sql
{
    public class Criteria
    {
        public enum Junction
        {
            NONE,
            AND,
            OR
        }

        public enum Operator
        {
            EQ,
            NE,
            LT,
            LE,
            GT,
            GE,
            LIKE,
            IN
        }

        private String GetOperator(Operator op)
        {
            String opString = String.Empty;
            switch (op)
            {
                case Operator.EQ:
                    opString = "=";
                    break;
                case Operator.NE:
                    opString = "<>";
                    break;
                case Operator.LT:
                    opString = "<";
                    break;
                case Operator.LE:
                    opString = "<=";
                    break;
                case Operator.GT:
                    opString = ">";
                    break;
                case Operator.GE:
                    opString = ">=";
                    break;
                case Operator.IN:
                    opString = "IN";
                    break;
                case Operator.LIKE:
                    opString = "LIKE";
                    break;
            }

            return opString;
        }

        private String GetJunction(Junction junction)
        {
            String junctionString = String.Empty;
            switch (junction)
            {
                case Junction.AND:
                    junctionString = "AND";
                    break;
                case Junction.OR:
                    junctionString = "OR";
                    break;
            }

            return junctionString;
        }

        public String FieldName { get; private set; }
        public String ValueOperator { get; private set; }
        public String Value { get; private set; }
        public CommonDataType.Type ValueType { get; private set; }
        public String ValueJunction { get; private set; }

        public Criteria(String fieldName, Operator valueOperator, Object value, CommonDataType.Type valueType, Junction valueJunction)
        {
            FieldName = fieldName;
            ValueOperator = GetOperator(valueOperator);
            ValueType = valueType;
            Value = CommonDataType.GetValue(value, valueType);
            ValueJunction = GetJunction(valueJunction);
        }
    }
}