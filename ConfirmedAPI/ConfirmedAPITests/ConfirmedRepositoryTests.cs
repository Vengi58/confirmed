using ConfirmedAPI.Data;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace ConfirmedAPITests
{
    public class Tests
    {
        private SqliteConnection connection;
        private ConfirmedRepository repo;

        [SetUp]
        public void Setup()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            repo = TestDb.CreateTestConfirmedRepository(connection);
        }

        [TearDown]
        public void TearDown()
        {
            connection.Close();
        }

        [Test]
        public void GetProduct_valid_productId_should_return_product()
        {
            var product = repo.GetProduct(1);

            Assert.NotNull(product);
            Assert.AreEqual(1, product.ID);
            Assert.AreEqual("Superstar", product.Name);
        }
    }
}