using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Databossy.Test
{
    [TestClass]
    public class DatabossyUnitTest
    {
        private const String Provider = "System.Data.SQLite";
        private readonly String ConnectionString = "DataSource=" + AppDomain.CurrentDomain.BaseDirectory + "\\databossytest.db;Password=databossytest;Version=3;Compress=True;UTF8Encoding=True;Page Size=1024;FailIfMissing=False;Read Only=False;Pooling=True;Max Pool Size=100;";

        public DatabossyUnitTest()
        {
            InitializeSqliteDbProvider();
        }

        private void InitializeSqliteDbProvider()
        {
            try
            {
                var configDs = ConfigurationManager.GetSection("system.data") as DataSet;
                if (configDs != null)
                    configDs.Tables[0]
                        .Rows.Add("SQLite Data Provider", ".Net Framework Data Provider for SQLite",
                            Provider, "System.Data.SQLite.SQLiteFactory, System.Data.SQLite");
            }
            catch { }
        }

        [TestMethod]
        public void QueryTest()
        {
            const String categoryId = "CAT28789";
            IList<Product> pList = null;
            using (var db = new Database(ConnectionString, Database.ConnectionStringType.ConnectionString, Provider))
                pList = db
                    .Query<Product>("SELECT * FROM [main].Product WHERE CategoryId = @0", categoryId)
                    .ToList();

            Assert.IsNotNull(pList);
            Assert.IsTrue(pList.Any());
            Assert.IsTrue(pList.Count > 1);
            Assert.AreEqual(pList[0].CategoryId, categoryId);
        }

        [TestMethod]
        public void QuerySingleTest()
        {
            const String productId = "PROD07341";
            Product p = null;
            using (var db = new Database(ConnectionString, Database.ConnectionStringType.ConnectionString, Provider))
                p = db.QuerySingle<Product>("SELECT * FROM [main].Product WHERE [Id] = @0", productId);

            Assert.IsNotNull(p);
            Assert.AreEqual(p.Id, productId);
        }

        
    }
}
