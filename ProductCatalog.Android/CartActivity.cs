using System.Collections.Generic;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProductCatalog.Android.Adapters;
using ProductCatalogSolution.Core.Helpers;
using ProductCatalogSolution.Core.Models;
using ProductCatalogSolution.Core.ViewModels;

namespace ProductCatalog.Android
{
    [Activity(
        Label = "@string/cart_activity_title",
        ParentActivity = typeof(MainActivity)
    )]
    public class CartActivity : Activity
    {
        private CartViewModel _cartViewModel;
        private ListView _lvlCart;
        private TextView _txtTotalOfUnits;
        private TextView _txtTotalPrice;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.cart);

            _cartViewModel = ServiceLocator.Instance.ResolveCartViewModel();
            _cartViewModel.OnProductsCartLoad += OnProductsCartLoad;
            _cartViewModel.OnTotalOfUnitsLoad += OnTotalOfUnitsLoad;
            _cartViewModel.OnTotalPriceLoad += OnTotalPriceLoad;

            _lvlCart = FindViewById<ListView>(Resource.Id.lvlCart);
            _lvlCart.AddFooterView(GetListViewFooter());
            _txtTotalOfUnits = FindViewById<TextView>(Resource.Id.txtTotalOfUnits);
            _txtTotalPrice = FindViewById<TextView>(Resource.Id.txtTotalPrice);

            LoadData();
        }

        private View GetListViewFooter()
        {
            var inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
            return inflater.Inflate(Resource.Layout.cart_footer, null);
        }

        private void OnProductsCartLoad(IList<Product> products)
        {
            var adapter = new CartAdapter(this, products);
            _lvlCart.Adapter = adapter;
        }

        private void OnTotalOfUnitsLoad(int totalOfUnits)
        {
            _txtTotalOfUnits.Text = $"{totalOfUnits} UN";
        }

        private void OnTotalPriceLoad(double totalPrice)
        {
            var culture = CultureInfo.CreateSpecificCulture("pt-BR");

            _txtTotalPrice.Text = totalPrice.ToString("C", culture); ;
        }

        private void LoadData()
        {
            LoadProductsInCart();
            LoadTotalOfUnits();
            LoadTotalPrice();
        }

        private void LoadProductsInCart()
        {
            if (_cartViewModel.GetProductsCommand.CanExecute(null))
            {
                _cartViewModel.GetProductsCommand.Execute(null);
            }
        }

        private void LoadTotalOfUnits()
        {
            if (_cartViewModel.GetTotalOfUnitsCommand.CanExecute(null))
            {
                _cartViewModel.GetTotalOfUnitsCommand.Execute(null);
            }
        }

        private void LoadTotalPrice()
        {
            if (_cartViewModel.GetTotalPriceCommand.CanExecute(null))
            {
                _cartViewModel.GetTotalPriceCommand.Execute(null);
            }
        }

        protected override void OnDestroy()
        {
            _cartViewModel.OnProductsCartLoad -= OnProductsCartLoad;
            _cartViewModel.OnTotalOfUnitsLoad -= OnTotalOfUnitsLoad;

            base.OnDestroy();
        }
    }
}
