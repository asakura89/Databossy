using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Databossy
{
    public static class Extensions
    {
        public static IEnumerable<TResult> ToIEnumerable<TResult>(this DataTable dt)
        {
            foreach (DataRow dataRow in dt.Rows)
            {
                var t = Activator.CreateInstance<TResult>();
                Type tType = typeof (TResult);
                PropertyInfo[] tProperties = tType.GetProperties();
                FieldInfo[] tFields = tType.GetFields();

                if (tProperties.Length != 0)
                {
                    foreach (PropertyInfo property in tProperties)
                    {
                        Type propertyType = property.PropertyType;

                        if (dataRow[property.Name] != DBNull.Value)
                            property.SetValue(t, Convert.ChangeType(dataRow[property.Name], propertyType), null);
                    }
                }
                else
                {
                    foreach (FieldInfo field in tFields)
                    {
                        Type fieldType = field.FieldType;

                        if (dataRow[field.Name] != DBNull.Value)
                            field.SetValue(t, Convert.ChangeType(dataRow[field.Name], fieldType));
                    }
                }

                yield return t;
            }
        }

        public static String ToDateForSql(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd");
        }

        public static String ToDateTimeForSql(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}