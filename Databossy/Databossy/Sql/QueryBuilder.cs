using System;
using System.Collections.Generic;

namespace Databossy.Sql
{
    public class QueryBuilder
    {
        public String defaultQuery = String.Empty;
        public String tableName = String.Empty;
        public List<String> fieldList = new List<String>();
        public List<Criteria> criteriaList = new List<Criteria>();
        public List<String> groupCriteriaList = new List<String>();
        public List<String> sortCriteriaList = new List<String>();
        public Sort.Type SortType = Sort.Type.ASC;

        private String BuildSelectClause()
        {
            String builtQueryString = String.Empty;

            if (defaultQuery != String.Empty)
            {
                builtQueryString = defaultQuery;
            }
            else
            {
                if (fieldList.Count != 0)
                {
                    String selectString = String.Join(", ", fieldList.ToArray());

                    builtQueryString = String.Format("SELECT {0} FROM {1}", selectString, tableName);
                }
                else
                {
                    builtQueryString = String.Format("SELECT * FROM {0}", tableName);
                }
            }

            return builtQueryString;
        }

        private String BuildWhereClause(String selectClause)
        {
            String criteriaString = String.Empty;
            foreach (Criteria criteria in criteriaList)
                criteriaString += String.Format("{0} {1} {2} {3} ", criteria.ValueJunction, criteria.FieldName, criteria.ValueOperator, criteria.Value);

            String builtQueryString = String.Format("{0} WHERE {1}", selectClause, criteriaString.Trim());

            return builtQueryString;
        }

        private String BuildGroupByClause(String selectClause)
        {
            String groupCriteriaString = String.Join(", ", groupCriteriaList.ToArray());
            String builtQueryString = String.Format("{0} GROUP BY {1}", selectClause, groupCriteriaString);

            return builtQueryString;
        }

        private String BuildOrderByClause(String selectClause)
        {
            String sortCriteriaString = String.Join(", ", sortCriteriaList.ToArray());
            String builtQueryString = String.Format("{0} ORDER BY {1}", selectClause, sortCriteriaString);

            return builtQueryString;
        }

        public String BuildQueryString()
        {
            if (tableName == String.Empty && defaultQuery == String.Empty)
                throw new ArgumentException("tableName");

            String finalBuiltQueryString = BuildSelectClause();

            if (criteriaList.Count != 0)
                finalBuiltQueryString = BuildWhereClause(finalBuiltQueryString);
            if (groupCriteriaList.Count != 0)
                finalBuiltQueryString = BuildGroupByClause(finalBuiltQueryString);
            if (sortCriteriaList.Count != 0)
                finalBuiltQueryString = BuildOrderByClause(finalBuiltQueryString);

            return finalBuiltQueryString;
        } 
    }
}