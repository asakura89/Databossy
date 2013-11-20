using System;
using System.Collections.Generic;
using System.Text;

namespace Databossy.Sql
{
    public class QueryBuilderEx
    {
        private readonly StringBuilder queryString = new StringBuilder();

        public QueryBuilderEx SelectAll()
        {
            queryString.Append("SELECT * ");
            return this;
        }

        public QueryBuilderEx Select(List<String> fieldList)
        {
            String joinedFieldList = String.Join(", ", fieldList.ToArray());
            queryString.Append(String.Format("SELECT {0} ", joinedFieldList));

            return this;
        }

        public QueryBuilderEx SelectDistinct(List<String> fieldList)
        {
            String joinedFieldList = String.Join(", ", fieldList.ToArray());
            queryString.Append(String.Format("SELECT DISTINCT {0} ", joinedFieldList));

            return this;
        }

        public QueryBuilderEx From(String tableName)
        {
            queryString.Append(String.Format("FROM {0} ", tableName));
            return this;
        }

        public QueryBuilderEx Where(List<Criteria> criteriaList)
        {
            String criteriaString = String.Empty;
            foreach (Criteria criteria in criteriaList)
                criteriaString += String.Format("{0} {1} {2} {3} ", criteria.ValueJunction, criteria.FieldName, criteria.ValueOperator, criteria.Value);
            queryString.Append(String.Format("WHERE {0} ", criteriaString.Trim()));

            return this;
        }

        public QueryBuilderEx GroupBy(List<String> fieldList)
        {
            String joinedFieldList = String.Join(", ", fieldList.ToArray());
            queryString.Append(String.Format("GROUP BY {0} ", joinedFieldList));

            return this;
        }

        public QueryBuilderEx OrderBy(List<String> fieldList)
        {
            return OrderBy(fieldList, Sort.Type.ASC);
        }

        public QueryBuilderEx OrderBy(List<String> fieldList, Sort.Type sortType)
        {
            String joinedFieldList = String.Join(", ", fieldList.ToArray());
            queryString.Append(String.Format("ORDER BY {0} {1} ", joinedFieldList, Sort.GetType(sortType)));

            return this;
        }

        public QueryBuilderEx Join(String tableName, List<Criteria> joinCriteriaList)
        {
            return Join(tableName, joinCriteriaList, global::Databossy.Sql.Join.Type.NONE);
        }

        public QueryBuilderEx Join(String tableName, List<Criteria> joinCriteriaList, Join.Type joinType)
        {
            String criteriaString = String.Empty;
            foreach (Criteria criteria in joinCriteriaList)
                criteriaString += String.Format("{0} {1} {2} {3} ", criteria.ValueJunction, criteria.FieldName, criteria.ValueOperator, criteria.Value);
            queryString.Append(String.Format("{0} JOIN {1} ON {2} ", global::Databossy.Sql.Join.GetType(joinType), tableName, criteriaString.Trim()).TrimStart());

            return this;
        }

        public QueryBuilderEx Insert(String tableName, List<InsertUpdateValue> insertValueList)
        {
            String joinedFieldList = String.Empty;
            String joinedValueList = String.Empty;
            foreach (InsertUpdateValue insertValue in insertValueList)
            {
                joinedFieldList += String.Format("{0}, ", insertValue.FieldName);
                joinedValueList += String.Format("{0}, ", CommonDataType.GetValue(insertValue.Value, insertValue.ValueType));
            }
            joinedFieldList.TrimEnd(new[] {',', ' '});
            joinedValueList.TrimEnd(new[] {',', ' '});
            queryString.Append(String.Format("INSERT INTO {0} ({1}) VALUES ({2}) ", tableName, joinedFieldList, joinedValueList));
            
            return this;
        }

        public QueryBuilderEx Update(String tableName, List<InsertUpdateValue> updateValueList)
        {
            String updateString = String.Empty;
            foreach (InsertUpdateValue updateValue in updateValueList)
                updateString += String.Format("{0} = {1}, ", CommonDataType.GetValue(updateValue.Value, updateValue.ValueType));

            updateString.TrimEnd(new[] { ',', ' ' });
            queryString.Append(String.Format("UPDATE {0} SET {1} ", tableName, updateString));

            return this;
        }

        public String Build()
        {
            return queryString.ToString().Trim();
        }
    }
}