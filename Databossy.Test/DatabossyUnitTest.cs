using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Databossy.Test {
    [TestClass]
    public class DatabossyUnitTest {
        const String Provider = "System.Data.SQLite";

        readonly String connectionString =
                $"DataSource={AppDomain.CurrentDomain.BaseDirectory}\\databossytest.db;Password=databossytest;Version=3;Compress=True;UTF8Encoding=True;Page Size=1024;FailIfMissing=False;Read Only=False;Pooling=True;Max Pool Size=100;"
            ;

        readonly IDatabaseFactory dbFactory = new DatabaseFactory();

        public DatabossyUnitTest() {
            InitializeSqliteDbProvider();
        }

        void InitializeSqliteDbProvider() {
            try {
                var configDs = ConfigurationManager.GetSection("system.data") as DataSet;
                if (configDs != null)
                    configDs.Tables[0]
                        .Rows.Add("SQLite Data Provider", ".Net Framework Data Provider for SQLite",
                            Provider, "System.Data.SQLite.SQLiteFactory, System.Data.SQLite");
            }
            catch { }
        }

        [TestMethod]
        public void QueryTest() {
            IList<Product> pList = null;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                pList = db.Query<Product>("SELECT * FROM Product").ToList();

            Assert.IsNotNull(pList);
            Assert.IsTrue(pList.Any());
            Assert.IsTrue(pList.Count > 1);
        }

        [TestMethod]
        public void QueryWParamTest() {
            const String categoryId = "CAT28789";
            IList<Product> pList = null;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                pList = db
                    .Query<Product>("SELECT * FROM Product WHERE CategoryId = @0", categoryId)
                    .ToList();

            Assert.IsNotNull(pList);
            Assert.IsTrue(pList.Any());
            Assert.IsTrue(pList.Count > 1);
            Assert.AreEqual(pList[0].CategoryId, categoryId);
        }

        [TestMethod]
        public void QueryWObjectParamTest() {
            const String categoryId = "CAT28789";
            IList<Product> pList = null;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                pList = db
                    .NQuery<Product>("SELECT * FROM Product WHERE CategoryId = @Category", new {Category = categoryId})
                    .ToList();

            Assert.IsNotNull(pList);
            Assert.IsTrue(pList.Any());
            Assert.IsTrue(pList.Count > 1);
            Assert.AreEqual(pList[0].CategoryId, categoryId);
        }

        [TestMethod]
        public void QueryDataSetTest() {
            DataSet ds = null;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                ds = db.QueryDataSet("SELECT * FROM Category; SELECT * FROM Product;");

            Assert.IsNotNull(ds);
            Assert.IsTrue(ds.Tables.Count == 2);
            Assert.IsTrue(ds.Tables[0].Rows.Count > 0);
            Assert.IsTrue(ds.Tables[1].Rows.Count > 0);
        }

        [TestMethod]
        public void QueryDataSetWParamTest() {
            const String categoryId = "CAT28789";
            DataSet ds = null;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                ds = db.QueryDataSet(
                    "SELECT * FROM Category; SELECT * FROM Product; SELECT COUNT(0) FROM Product WHERE CategoryId = @0",
                    categoryId);

            Assert.IsNotNull(ds);
            Assert.IsTrue(ds.Tables.Count == 3);
            Assert.IsTrue(ds.Tables[0].Rows.Count > 0);
            Assert.IsTrue(ds.Tables[1].Rows.Count > 0);
            Assert.IsTrue(ds.Tables[2].Rows.Count == 1);
            Assert.AreEqual(Convert.ToInt32(ds.Tables[2].Rows[0][0]), 7);
        }

        [TestMethod]
        public void QueryDataSetWObjectParamTest() {
            const String categoryId = "CAT28789";
            DataSet ds = null;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                ds = db.NQueryDataSet(
                    "SELECT * FROM Category; SELECT * FROM Product; SELECT COUNT(0) FROM Product WHERE CategoryId = @Category",
                    new {Category = categoryId});

            Assert.IsNotNull(ds);
            Assert.IsTrue(ds.Tables.Count == 3);
            Assert.IsTrue(ds.Tables[0].Rows.Count > 0);
            Assert.IsTrue(ds.Tables[1].Rows.Count > 0);
            Assert.IsTrue(ds.Tables[2].Rows.Count == 1);
            Assert.AreEqual(Convert.ToInt32(ds.Tables[2].Rows[0][0]), 7);
        }

        [TestMethod]
        public void QueryDataTableTest() {
            DataTable dt = null;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                dt = db.QueryDataTable("SELECT * FROM Product");

            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
        }

        [TestMethod]
        public void QueryDataTableWParamTest() {
            const String productId = "PROD07341";
            DataTable dt = null;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                dt = db.QueryDataTable("SELECT * FROM Product WHERE [Id] = @0", productId);

            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual(dt.Rows[0]["Id"].ToString(), productId);
        }

        [TestMethod]
        public void QueryDataTableWObjectParamTest() {
            const String productId = "PROD07341";
            DataTable dt = null;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                dt = db.NQueryDataTable("SELECT * FROM Product WHERE [Id] = @Product", new {Product = productId});

            Assert.IsNotNull(dt);
            Assert.IsTrue(dt.Rows.Count > 0);
            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual(dt.Rows[0]["Id"].ToString(), productId);
        }

        [TestMethod]
        public void QueryScalarTest() {
            const String categoryId = "CAT28789";
            Int64 productCount = 0;
            Boolean isExists = false;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider)) {
                // NOTE: EXISTS and COUNT in sqlite return object {long} type and value is case-sensitive
                productCount = db.QueryScalar<Int64>("SELECT COUNT(0) FROM Product WHERE CategoryId = @0", categoryId);
                isExists = Convert.ToBoolean(
                    db.QueryScalar<Int64>(
                        "SELECT EXISTS (SELECT * FROM sqlite_master WHERE type = 'table' AND name = @0);", "Product"));
            }

            Assert.IsTrue(productCount != 0);
            Assert.IsTrue(productCount > 1);

            Assert.IsTrue(isExists);
        }

        [TestMethod]
        public void QueryScalarNullableTest() {
            const String categoryId = "CAT28789";
            Int64 productCount = 0;
            Int64? nullableCount = 0;
            Boolean isExists = false;
            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider)) {
                // NOTE: EXISTS and COUNT in sqlite return object {long} type and value is case-sensitive
                productCount = db.NQueryScalar<Int64>("SELECT COUNT(0) FROM Product WHERE CategoryId = @Category", new {Category = categoryId});
                isExists = Convert.ToBoolean(
                    db.QueryScalar<Int64>(
                        "SELECT EXISTS (SELECT * FROM sqlite_master WHERE type = 'table' AND name = @0);", "Product"));

                nullableCount = db.QueryScalar<Int64?>("SELECT 1");
            }

            Assert.IsTrue(productCount != 0);
            Assert.IsTrue(productCount > 1);

            Assert.IsTrue(isExists);

            Assert.AreEqual(nullableCount, 1);
        }

        [TestMethod]
        public void QuerySingleTest() {
            const String productId = "PROD07341";
            ProductViewModel pVM = null;
            var query = new StringBuilder()
                .Append("SELECT p.[Id], p.[Name], c.[Desc] CategoryJ ")
                .Append("FROM Product p JOIN Category c ON c.[Id] = p.CategoryId ")
                .Append("WHERE p.[Id] = @0")
                .ToString();

            using (IDatabase db = dbFactory.CreateSession(connectionString, true, Provider))
                pVM = db.QuerySingle<ProductViewModel>(query, productId);

            Assert.IsNotNull(pVM);
            Assert.AreEqual(pVM.Id, productId);
            Assert.IsFalse(String.IsNullOrEmpty(pVM.CategoryJ));
        }

        [TestMethod]
        public void WithTransactionTest() { }
    }
}