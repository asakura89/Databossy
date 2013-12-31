using System;
using System.Collections.Generic;

namespace Databossy
{
    public class ObjectBase
    {
        public IEnumerable<TResult> GetAll<TResult>()
        {
            Type tType = typeof (TResult);
            String query = String.Format("SELECT * FROM {0}", tType.Name);
            using (var db = new Database())
                return db.Query<TResult>(query);
        }

        public IEnumerable<TResult> GetList<TResult>()
        {
            return GetAll<TResult>();
        }
    }
}