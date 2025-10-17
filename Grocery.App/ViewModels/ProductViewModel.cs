using CommunityToolkit.Mvvm.ComponentModel;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        [ObservableProperty]
        private ObservableCollection<Product> products;

        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
            Products = _productService.GetAll();
        }
    }
}
