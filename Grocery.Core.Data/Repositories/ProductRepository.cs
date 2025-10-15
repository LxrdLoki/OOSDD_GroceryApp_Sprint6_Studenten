using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        private readonly List<Product> products = new();
        public ProductRepository()
        {
            CreateTable(@"
                        DROP TABLE IF EXISTS Products;
                        CREATE TABLE IF NOT EXISTS Products (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] NVARCHAR(80) NOT NULL,
                            [Stock] INTEGER NOT NULL,
                            [ShelfLife] DATE NOT NULL,
                            [Price] INTEGER NOT NULL
                        );
                    ");
            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO Products(Id, Name, Stock, ShelfLife, Price) VALUES(1, 'Melk', 300, '2025-09-25', 0.95)",
                @"INSERT OR IGNORE INTO Products(Id, Name, Stock, ShelfLife, Price) VALUES(2, 'Kaas', 100, '2025-09-30', 7.98)",
                @"INSERT OR IGNORE INTO Products(Id, Name, Stock, ShelfLife, Price) VALUES(4, 'Cornflakes', 0, '2025-12-31', 1.48)",
                @"INSERT OR IGNORE INTO Products(Id, Name, Stock, ShelfLife, Price) VALUES(3, 'Brood', 400, '2025-09-12', 2.19)"
            ];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }
        public List<Product> GetAll()
        {
            products.Clear();
            string selectQuery = "SELECT * FROM Products";

            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                    decimal price = reader.GetDecimal(4);
                    products.Add(new(id, name, stock, shelfLife, price));
                }
            }
            CloseConnection();
            return products;
        }

        public Product? Get(int id)
        {
            return products.FirstOrDefault(p => p.Id == id);
        }

        public Product Add(Product item)
        {
            throw new NotImplementedException();
        }

        public Product? Delete(Product item)
        {
            throw new NotImplementedException();
        }

        public Product? Update(Product item)
        {
            Product? product = products.FirstOrDefault(p => p.Id == item.Id);
            if (product == null) return null;
            product.Id = item.Id;
            return product;
        }
    }
}
