using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {

    }
}

namespace Databossy.NET20
{
    public static class Extensions
    {
        public static IEnumerable<TResult> ToIEnumerable<TResult>(this DataTable dt)
        {
            foreach (DataRow dataRow in dt.Rows)
            {
                TResult t = Activator.CreateInstance<TResult>();
                Type tType = typeof(TResult);
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

        public static String ToDateForSQL(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd");
        }

        public static String ToDateTimeForSQL(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static TResult FirstOrDefault<TResult>(this IEnumerable<TResult> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            IList<TResult> list = source as IList<TResult>;
            if (list != null)
            {
                if (list.Count > 0)
                    return list[0];
            }
            else
            {
                using (IEnumerator<TResult> enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                        return enumerator.Current;
                }
            }

            return default(TResult);
        }
    }
}