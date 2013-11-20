using System;

namespace Databossy.Sql
{
    public static class Sort
    {
        public enum Type
        {
            ASC,
            DESC
        }

        public static String GetType(Type type)
        {
            String sortString = String.Empty;
            switch (type)
            {
                case Type.ASC:
                    sortString = "ASC";
                    break;
                case Type.DESC:
                    sortString = "DESC";
                    break;
            }

            return sortString;
        }
    }
}