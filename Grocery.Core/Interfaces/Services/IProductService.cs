using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.Core.Interfaces.Services
{
    public interface IProductService
    {
        public ObservableCollection<Product> GetAll();

        public Product Add(Product item);

        public Product? Delete(Product item);

        public Product? Get(int id);

        public Product? Update(Product item);
    }
}
