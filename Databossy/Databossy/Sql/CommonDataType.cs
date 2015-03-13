using System;

namespace Databossy.Sql
{
    public class CommonDataType
    {
        public enum Type
        {
            STRING,
            INTEGER,
            DATETIME,
            FIELD
        }

        public static String GetValue(Object value, Type type)
        {
            String valueString = String.Empty;
            switch (type)
            {
                case Type.STRING:
                    valueString = String.Format("'{0}'", value);
                    break;
                case Type.FIELD:
                case Type.INTEGER:
                    valueString = value.ToString();
                    break;
                case Type.DATETIME:
                    valueString = Convert.ToDateTime(value).ToDateTimeForSql();
                    break;
            }

            return valueString;
        }
    }
}