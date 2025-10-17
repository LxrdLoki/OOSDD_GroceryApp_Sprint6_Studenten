using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly  IProductService _productService;

        [ObservableProperty]
        private string name = "exampleProduct";

        [ObservableProperty]
        private int stock = 3;

        [ObservableProperty]
        private decimal price = 0.0m;

        [ObservableProperty]
        private DateOnly shelfLife = DateOnly.FromDateTime(DateTime.Now.AddDays(30));

        [ObservableProperty]
        private string error = "";

        [ObservableProperty]
        Client client;

        public NewProductViewModel(IProductService productService, GlobalViewModel global)
        {
            _productService = productService;
            client = global.Client;
        }

        [RelayCommand]
        public void AddProduct()
        {
            if (string.IsNullOrWhiteSpace(Name) || Stock < 0)
            {
                Error = "Fout, naam bestaad al of je hebt ene verkeerde voorraad doorgegeven";
                return;
            }
            Debug.WriteLine(Price);
            Price = ConvertPrice(Price);
            Product newProduct = new Product(0, Name, Stock, ShelfLife, (decimal)Price);
            _productService.Add(newProduct);

            Error = $"Product {Name} is toegevoegd!";
        }

        private decimal ConvertPrice(decimal price)
        {
            return Math.Round(price, 2);
        }
    }
}
