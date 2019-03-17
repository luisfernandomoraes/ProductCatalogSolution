using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content.PM;
using ProductCatalogSolution.Core.ViewModels;
using System.Collections.Generic;
using ProductCatalogSolution.Core.Models;
using ProductCatalogSolution.Core.Helpers;
using ProductCatalog.Android.Adapters;
using ProductCatalog.Android.Services;
using ProductCatalogSolution.Core.Services;
using Android.Views;
using ProductCatalog.Android.Interfaces;
using ProductCatalog.Android.Enums;
using System.Linq;
using System.Globalization;

namespace ProductCatalog.Android
{
    /// <summary>
    /// Refactor
    /// </summary>
    [Activity(Label = "@string/main_activity_title", Theme = "@style/AppTheme", MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : AppCompatActivity
    {

        private StoreViewModel _storeViewModel;
        private ProductAdapter _adapter;
        private LinearLayout _viewGroupFooter;
        private Button _btnBuy;
        private ListView _lvlProducts;
        private IList<Category> _categories;


        public MainActivity()
        {
            ServiceLocator.Instance.RegisterNavigationService(new NavigationService());
            ServiceLocator.Instance.RegisterCacheService(new AcavacheCacheService());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _storeViewModel = ServiceLocator.Instance.ResolveStoreViewModel();
            _storeViewModel.OnCatalogDataLoad += CatalogDataLoad;
            _storeViewModel.OnProductCategoryLoad += ProductCategoryLoad;
            _storeViewModel.OnProductUpdate += ProductUpdate;
            _storeViewModel.OnProductsCartUpdate += ProductsCartUpdate;
            _storeViewModel.OnTotalPriceUpdate += TotalPriceUpdate;

            _adapter = new ProductAdapter(this);
            _adapter.OnProductQuantityDecrease += ProductQuantityDecrease;
            _adapter.OnProductQuantityIncrease += ProductQuantityIncrease;
            _adapter.OnToggleFavorite += ToggleFavorite;  

            _viewGroupFooter = FindViewById<LinearLayout>(Resource.Id.viewGroupFooter);
            _viewGroupFooter.Visibility = ViewStates.Gone;
            _btnBuy = FindViewById<Button>(Resource.Id.btnBuy);
            _lvlProducts = FindViewById<ListView>(Resource.Id.lvlProducts);
            _categories = new List<Category>();

            _btnBuy.Click += BtnBuyClick;
            _lvlProducts.ItemClick += ItemClick;

            LoadCatalogData();
        }

        private void LoadCatalogData()
        {
            if (_storeViewModel.LoadCatalogDataCommand.CanExecute(null))
            {
                _storeViewModel.LoadCatalogDataCommand.Execute(null);
            }
        }

        private void CatalogDataLoad(IList<ProductCollection> products)
        {
            _adapter.SetProducts(products);
            _lvlProducts.Adapter = _adapter;
        }

        private void ProductCategoryLoad(IList<Category> categories)
        {
            _categories = categories;
            InvalidateOptionsMenu();
        }

        private void ProductUpdate(Product product)
        {
            _adapter.UpdateProduct(product);
        }

        private void ProductsCartUpdate(bool hasProductsInCart)
        {
            if (hasProductsInCart)
            {
                _viewGroupFooter.Visibility = ViewStates.Visible;
                return;
            }

            _viewGroupFooter.Visibility = ViewStates.Gone;
        }

        private void TotalPriceUpdate(double totalPrice)
        {
            var culture = CultureInfo.CreateSpecificCulture("pt-BR");
            _btnBuy.Text = $"Comprar ► {totalPrice.ToString("C", culture)}";
        }

        private void ProductQuantityDecrease(Product product)
        {
            if (_storeViewModel.DecreaseProductQuantityCommand.CanExecute(product))
            {
                _storeViewModel.DecreaseProductQuantityCommand.Execute(product);
            }
        }

        private void ProductQuantityIncrease(Product product)
        {
            if (_storeViewModel.IncreaseProductQuantityCommand.CanExecute(product))
            {
                _storeViewModel.IncreaseProductQuantityCommand.Execute(product);
            }
        }

        private void ToggleFavorite(Product product)
        {
            if (_storeViewModel.ToggleFavoriteCommand.CanExecute(product))
            {
                _storeViewModel.ToggleFavoriteCommand.Execute(product);
            }
        }

        private void BtnBuyClick(object sender, System.EventArgs e)
        {
            if (_storeViewModel.NavigateToCartCommand.CanExecute(null))
            {
                _storeViewModel.NavigateToCartCommand.Execute(null);
            }
        }

        private void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = _adapter[e.Position];
            if (IsNotListItem(item)) return;

            var productId = item.GetId();

            if (_storeViewModel.NavigateToDetailByProductIdCommand.CanExecute(productId))
            {
                _storeViewModel.NavigateToDetailByProductIdCommand.Execute(productId);
            }
        }

        private bool IsNotListItem(IListViewItem item)
        {
            return !IsListItem(item);
        }

        private bool IsListItem(IListViewItem item)
        {
            return item.GetViewType().Equals(ListViewRowType.ListItem);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (_categories.Count == 0) return false;

            menu.Clear();
            menu.Add("Todas as categorias");

            foreach (var category in _categories)
            {
                menu.Add(category.Name);
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var name = item.ToString();
            var category = GetCategoryByName(name);
            GetProductsByCategory(category);

            return base.OnOptionsItemSelected(item);
        }

        
        private Category GetCategoryByName(string name)
        {
            return _categories.FirstOrDefault(e => e.Name.Equals(name));
        }

        private void GetProductsByCategory(Category category)
        {
            if (category == null)
            {
                if (_storeViewModel.GetProductsCommand.CanExecute(null))
                {
                    _storeViewModel.GetProductsCommand.Execute(null);
                }
                return;
            }

            if (_storeViewModel.GetProductsByCategoryIdCommand.CanExecute(category.Id))
            {
                _storeViewModel.GetProductsByCategoryIdCommand.Execute(category.Id);
            }
        }


        protected override void OnResume()
        {
            base.OnResume();

            _adapter.UpdateListItems();

            if (_storeViewModel.UpdateCartDataCommand.CanExecute(null))
            {
                _storeViewModel.UpdateCartDataCommand.Execute(null);
            }
        }

        protected override void OnDestroy()
        {
            _storeViewModel.OnCatalogDataLoad -= CatalogDataLoad;
            _storeViewModel.OnProductCategoryLoad -= ProductCategoryLoad;
            _storeViewModel.OnProductUpdate -= ProductUpdate;
            _storeViewModel.OnProductsCartUpdate -= ProductsCartUpdate;
            _storeViewModel.OnTotalPriceUpdate -= TotalPriceUpdate;

            _adapter.OnProductQuantityDecrease -= ProductQuantityDecrease;
            _adapter.OnProductQuantityIncrease -= ProductQuantityIncrease;
            _adapter.OnToggleFavorite -= ToggleFavorite;

            _btnBuy.Click -= BtnBuyClick;
            _lvlProducts.ItemClick -= ItemClick;

            base.OnDestroy();
        }

    }
}