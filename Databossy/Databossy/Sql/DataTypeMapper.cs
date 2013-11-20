using System;
using System.Collections.Generic;
using System.Data;

namespace Databossy.Sql
{
    public class DataTypeMapper
    {
        private static readonly Dictionary<Type, DbType> typeMapList = new Dictionary<Type, DbType>();

        public DataTypeMapper()
        {
            typeMapList[typeof(byte)] = DbType.Byte;
            typeMapList[typeof(sbyte)] = DbType.SByte;
            typeMapList[typeof(short)] = DbType.Int16;
            typeMapList[typeof(ushort)] = DbType.UInt16;
            typeMapList[typeof(int)] = DbType.Int32;
            typeMapList[typeof(uint)] = DbType.UInt32;
            typeMapList[typeof(long)] = DbType.Int64;
            typeMapList[typeof(ulong)] = DbType.UInt64;
            typeMapList[typeof(float)] = DbType.Single;
            typeMapList[typeof(double)] = DbType.Double;
            typeMapList[typeof(decimal)] = DbType.Decimal;
            typeMapList[typeof(bool)] = DbType.Boolean;
            typeMapList[typeof(string)] = DbType.String;
            typeMapList[typeof(char)] = DbType.StringFixedLength;
            typeMapList[typeof(Guid)] = DbType.Guid;
            typeMapList[typeof(DateTime)] = DbType.DateTime;
            typeMapList[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMapList[typeof(byte[])] = DbType.Binary;
            typeMapList[typeof(byte?)] = DbType.Byte;
            typeMapList[typeof(sbyte?)] = DbType.SByte;
            typeMapList[typeof(short?)] = DbType.Int16;
            typeMapList[typeof(ushort?)] = DbType.UInt16;
            typeMapList[typeof(int?)] = DbType.Int32;
            typeMapList[typeof(uint?)] = DbType.UInt32;
            typeMapList[typeof(long?)] = DbType.Int64;
            typeMapList[typeof(ulong?)] = DbType.UInt64;
            typeMapList[typeof(float?)] = DbType.Single;
            typeMapList[typeof(double?)] = DbType.Double;
            typeMapList[typeof(decimal?)] = DbType.Decimal;
            typeMapList[typeof(bool?)] = DbType.Boolean;
            typeMapList[typeof(char?)] = DbType.StringFixedLength;
            typeMapList[typeof(Guid?)] = DbType.Guid;
            typeMapList[typeof(DateTime?)] = DbType.DateTime;
            typeMapList[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            //typeMapList[typeof(System.Data.Linq.Binary)] = DbType.Binary; 
        }

        public static DbType GetDbType(Type type)
        {
            return typeMapList[type];
        }

        public static void AddDbType(Type type, DbType dbType)
        {
            typeMapList[type] = dbType;
        }
    }
}
