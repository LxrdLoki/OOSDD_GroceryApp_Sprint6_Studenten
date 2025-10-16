using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        private readonly List<GroceryListItem> groceryListItems = new();

        public GroceryListItemsRepository()
        {
            CreateTable(@"
                DROP TABLE IF EXISTS GroceryListItems;
                        CREATE TABLE IF NOT EXISTS GroceryListItems (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [GroceryListId] INTEGER NOT NULL,
                            [ProductId] INTEGER NOT NULL,
                            [Amount] INTEGER NOT NULL
                        );
                    ");

            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(1, 1, 3)",
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(2, 2, 1)",
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(3, 3, 4)",
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(4, 1, 2)",
                @"INSERT OR IGNORE INTO GroceryListItems(GroceryListId, ProductId, Amount) VALUES(5, 2, 5)"
            ];
            InsertMultipleWithTransaction(insertQueries);
            GetAll();
        }

        public List<GroceryListItem> GetAll()
        {
            groceryListItems.Clear();
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    groceryListItems.Add(new(id, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            List<GroceryListItem> selectedItems = new();
            string selectQuery = $"SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE GroceryListId = {id}";
            OpenConnection();
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int item_id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    selectedItems.Add(new(id, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return selectedItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            string addQuery = $@"
                INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount)
                VALUES ({item.GroceryListId}, {item.ProductId}, {item.Amount});
            ";
            OpenConnection();
            using (SqliteCommand command = new(addQuery, Connection))
            {
                command.ExecuteNonQuery();
            }


            int newId;
            using (SqliteCommand command = new("SELECT last_insert_rowid();", Connection))
            {
                newId = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();

            return Get(newId);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            GroceryListItem? groceryListItemOnID = null;
            string getItemQuery = $"SELECT * FROM GroceryListItems WHERE Id == {id}";

            OpenConnection();
            using (SqliteCommand command = new(getItemQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int itemId = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    groceryListItemOnID = new(itemId, groceryListId, productId, amount);
                }
            }
            CloseConnection();
            return groceryListItemOnID;
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            string updateQuery = $"UPDATE GroceryListItems set GroceryListId = {item.GroceryListId}, ProductId = {item.ProductId}, Amount = {item.Amount} WHERE Id = {item.Id}";
            OpenConnection();
            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }
    }
}
