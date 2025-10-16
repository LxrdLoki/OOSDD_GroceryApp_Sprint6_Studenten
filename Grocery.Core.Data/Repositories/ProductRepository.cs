using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        private readonly List<Product> products = new();
        public ProductRepository()
        {
            CreateTable(@"
                DROP TABLE IF EXISTS Products;
            ");
            CreateTable(@"
                        CREATE TABLE IF NOT EXISTS Products (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] NVARCHAR(80) NOT NULL,
                            [Stock] INTEGER NOT NULL,
                            [ShelfLife] DATE NOT NULL,
                            [Price] INTEGER NOT NULL
                        );
                    ");

            OpenConnection();
            using (var cmd = new SqliteCommand("PRAGMA table_info(Products);", Connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    Debug.WriteLine("---- Products table columns ----");
                    while (reader.Read())
                    {
                        Debug.WriteLine($"{reader.GetString(1)} ({reader.GetString(2)})");
                    }
                }
            }
            CloseConnection();

            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO Products(Name, Stock, ShelfLife, Price) VALUES('Melk', 300, '2025-09-25', 0.95)",
                @"INSERT OR IGNORE INTO Products(Name, Stock, ShelfLife, Price) VALUES('Kaas', 100, '2025-09-30', 7.98)",
                @"INSERT OR IGNORE INTO Products(Name, Stock, ShelfLife, Price) VALUES('Cornflakes', 0, '2025-12-31', 1.48)",
                @"INSERT OR IGNORE INTO Products(Name, Stock, ShelfLife, Price) VALUES('Brood', 400, '2025-09-12', 2.19)"
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
                    DateOnly shelfLife = DateOnly.Parse(reader.GetString(3));
                    decimal price = reader.GetDecimal(4);
                    products.Add(new(id, name, stock, shelfLife, price));
                }
            }
            CloseConnection();
            return products;
        }

        public Product? Get(int id)
        {
            Product? productOnId = null;
            string getProductQuery = $"SELECT * FROM Products WHERE Id = {id}";
            
            OpenConnection();
            using (SqliteCommand command = new(getProductQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int productId = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                    decimal price = reader.GetDecimal(4);
                    productOnId = new(productId, name, stock, shelfLife, price);
                }
            }
            CloseConnection();
            return productOnId;

        }

        public Product Add(Product item)
        {
            string insertQuery = @"
                INSERT INTO Products(Name, Stock, ShelfLife, Price) 
                VALUES(@Name, @Stock, @ShelfLife, @Price)
            ";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Stock", item.Stock);
                command.Parameters.AddWithValue("@ShelfLife", item.ShelfLife.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@Price", item.Price);

                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public Product? Delete(Product item)
        {
            throw new NotImplementedException();
        }

        public Product? Update(Product item)
        {
            string updateQuery = $@"
                  UPDATE Products
                  SET Name = @Name,
                  Stock = @Stock,
                  ShelfLife = @ShelfLife,
                  Price = @Price
                  WHERE Id = @Id;
            ";


            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Stock", item.Stock);
                command.Parameters.AddWithValue("@ShelfLife", item.ShelfLife.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@Price", item.Price);
                command.Parameters.AddWithValue("@Id", item.Id);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }
    }
}
