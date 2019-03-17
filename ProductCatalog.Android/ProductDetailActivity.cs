using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using ProductCatalogSolution.Core.Helpers;
using ProductCatalogSolution.Core.Models;
using ProductCatalogSolution.Core.ViewModels;

namespace ProductCatalog.Android
{
    [Activity(
        Label = "@string/product_detail_title",
        ParentActivity = typeof(MainActivity)
    )]
    public class ProductDetailActivity : AppCompatActivity
    {
        public const string PRODUCT_ID_PARAMETER = "PRODUCT_ID_PARAMETER";

        private int _productId;
        private Product _product;
        private ProductDetailViewModel _productDetailViewModel;
        private ImageViewAsync _imgPhoto;
        private TextView _txtName;
        private TextView _txtDescription;
        private TextView _txtCurrentPrice;
        private TextView _txtQuantity;
        private TextView _txtDiscount;
        private LinearLayout _viewGroupDiscount;
        private ToggleButton _togFavorite;
        private ImageButton _btnDecrease;
        private ImageButton _btnIncrease;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.product_detail);

            _imgPhoto = FindViewById<ImageViewAsync>(Resource.Id.imgPhoto);
            _txtName = FindViewById<TextView>(Resource.Id.txtName);
            _txtDescription = FindViewById<TextView>(Resource.Id.txtDescription);
            _txtCurrentPrice = FindViewById<TextView>(Resource.Id.txtCurrentPrice);
            _txtQuantity = FindViewById<TextView>(Resource.Id.txtQuantity);
            _txtDiscount = FindViewById<TextView>(Resource.Id.txtDiscount);
            _viewGroupDiscount = FindViewById<LinearLayout>(Resource.Id.viewGroupDiscount);
            _togFavorite = FindViewById<ToggleButton>(Resource.Id.togFavorite);
            _btnDecrease = FindViewById<ImageButton>(Resource.Id.btnDecrease);
            _btnIncrease = FindViewById<ImageButton>(Resource.Id.btnIncrease);

            _togFavorite.Click += OnToggleFavoriteClick;
            _btnDecrease.Click += OnBtnDecreaseClick;
            _btnIncrease.Click += OnBtnIncreaseClick;

            _productId = Intent.GetIntExtra(PRODUCT_ID_PARAMETER, 0);

            _productDetailViewModel = ServiceLocator.Instance.ResolveProductDetailViewModel();
            _productDetailViewModel.OnProductLoad += OnProductLoad;

            LoadData();
        }

        private void OnToggleFavoriteClick(object sender, EventArgs e)
        {
            if (_productDetailViewModel.ToggleFavoriteCommand.CanExecute(_product))
            {
                _productDetailViewModel.ToggleFavoriteCommand.Execute(_product);
            }
        }

        private void OnBtnDecreaseClick(object sender, EventArgs e)
        {
            if (_productDetailViewModel.DecreaseProductQuantityCommand.CanExecute(_product))
            {
                _productDetailViewModel.DecreaseProductQuantityCommand.Execute(_product);
            }
        }

        private void OnBtnIncreaseClick(object sender, EventArgs e)
        {
            if (_productDetailViewModel.IncreaseProductQuantityCommand.CanExecute(_product))
            {
                _productDetailViewModel.IncreaseProductQuantityCommand.Execute(_product);
            }
        }

        private void OnProductLoad(Product product)
        {
            _product = product;

            ImageService
                .Instance
                .LoadUrl(_product.Photo)
                .DownSample(250, 250)
                .Into(_imgPhoto);

            var culture = CultureInfo.CreateSpecificCulture("pt-BR");
            _txtCurrentPrice.Text = _product.CurrentPrice.ToString("C", culture);

            _txtName.Text = _product.Name;
            _txtDescription.Text = _product.Description;
            
            _txtQuantity.Text = _product.Quantity.ToString();
            _txtDiscount.Text = $"{_product.Discount.ToString()} %";
            _viewGroupDiscount.Visibility = GetViewStateForViewGroupDiscount();
            _togFavorite.Checked = _product.IsFavorite;
        }

        private ViewStates GetViewStateForViewGroupDiscount()
        {
            if (_product.HasDiscount())
            {
                return ViewStates.Visible;
            }

            return ViewStates.Invisible;
        }

        private void LoadData()
        {
            LoadProductData();
        }

        private void LoadProductData()
        {
            if (_productDetailViewModel.GetProductByIdCommand.CanExecute(_productId))
            {
                _productDetailViewModel.GetProductByIdCommand.Execute(_productId);
            }
        }

        protected override void OnDestroy()
        {
            _productDetailViewModel.OnProductLoad -= OnProductLoad;
            _togFavorite.Click -= OnToggleFavoriteClick;
            _btnDecrease.Click -= OnBtnDecreaseClick;
            _btnIncrease.Click -= OnBtnIncreaseClick;

            base.OnDestroy();
        }
    }
}