using System;
using System.Linq;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using FFImageLoading;
using ProductCatalog.Android.Interfaces;
using ProductCatalogSolution.Core.Models;
using ProductCatalog.Android.Enums;
using System.Globalization;
using ProductCatalog.Android.Helper;

namespace ProductCatalog.Android.Adapters
{
    /// <summary>
    /// TODO: Refatorar
    /// </summary>
    public class ProductAdapter : BaseAdapter<IListViewItem>
    {
        public delegate void ProductQuantityDecreaseDelegate(Product product);
        public delegate void ProductQuantityIncreaseDelegate(Product product);
        public delegate void ToggleFavoriteDelegate(Product product);

        public event ProductQuantityDecreaseDelegate OnProductQuantityDecrease;
        public event ProductQuantityIncreaseDelegate OnProductQuantityIncrease;
        public event ToggleFavoriteDelegate OnToggleFavorite;

        private LayoutInflater _inflater;
        private IList<IListViewItem> _products;

        public ProductAdapter(Context context)
        {
            _inflater = LayoutInflater.FromContext(context);
        }

        public void SetProducts(IList<ProductCollection> productGroup)
        {
            _products = new List<IListViewItem>();

            foreach (var group in productGroup)
            {
                _products.Add(new ProductHeader(group.Name));

                foreach (var product in group.ToList())
                {
                    _products.Add(new ProductItem(this, product));
                }
            }
        }

        public void UpdateProduct(Product product)
        {
            var existingProduct = _products.FirstOrDefault(e => e.GetId() == product.Id);
            if (existingProduct == null)
            {
                return;
            }

            existingProduct.UpdateItem(product);
            UpdateListItems();
        }

        public void UpdateListItems()
        {
            NotifyDataSetChanged();
        }

        public override IListViewItem this[int position] => _products[position];

        public override int Count => _products.Count;

        public override long GetItemId(int position)
        {
            return (long)_products[position].GetView(_inflater, null).Id;
        }

        public new ListViewRowType GetItemViewType(int position)
        {
            return GetItem(position).Cast<IListViewItem>().GetViewType();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = GetItem(position).Cast<IListViewItem>();
            return item.GetView(_inflater, convertView);
        }

        private class ProductItem : IListViewItem
        {
            private ProductAdapter _adapter;
            private Product _product;
            private ImageButton _btnDecrease;
            private ImageButton _btnIncrease;
            private ToggleButton _togFavorite;

            public ProductItem(ProductAdapter adapter, Product product)
            {
                _adapter = adapter;
                _product = product;
            }

            ~ProductItem()
            {
                if (_btnDecrease != null)
                {
                    _btnDecrease.Click -= OnBtnDecreaseClick;
                }

                if (_btnIncrease != null)
                {
                    _btnIncrease.Click -= OnBtnIncreaseClick;
                }

                if (_togFavorite != null)
                {
                    _togFavorite.Click -= OnToggleFavoriteClick;
                }
            }

            public View GetView(LayoutInflater inflater, View convertView)
            {
                var view = inflater.Inflate(Resource.Layout.product_item, null);

                var imgPhoto = view.FindViewById<ImageViewAsync>(Resource.Id.imgPhoto);
                var txtName = view.FindViewById<TextView>(Resource.Id.txtName);
                var txtPrice = view.FindViewById<TextView>(Resource.Id.txtPrice);
                var txtQuantity = view.FindViewById<TextView>(Resource.Id.txtQuantity);
                var txtDiscount = view.FindViewById<TextView>(Resource.Id.txtDiscount);
                var viewGroupDiscount = view.FindViewById<LinearLayout>(Resource.Id.viewGroupDiscount);

                _btnDecrease = view.FindViewById<ImageButton>(Resource.Id.btnDecrease);
                _btnIncrease = view.FindViewById<ImageButton>(Resource.Id.btnIncrease);
                _togFavorite = view.FindViewById<ToggleButton>(Resource.Id.togFavorite);

                ImageService
                    .Instance
                    .LoadUrl(_product.Photo)
                    .DownSample(100, 100)
                    .Into(imgPhoto);

                txtName.Text = _product.Name;

                var culture = CultureInfo.CreateSpecificCulture("pt-BR");

                txtPrice.Text = _product.CurrentPrice.ToString("C", culture);
                txtQuantity.Text = _product.Quantity.ToString();
                txtDiscount.Text = $"{_product.Discount.ToString()} %";
                viewGroupDiscount.Visibility = GetViewStateForViewGroupDiscount();
                _togFavorite.Checked = _product.IsFavorite;

                _btnDecrease.Click += OnBtnDecreaseClick;
                _btnIncrease.Click += OnBtnIncreaseClick;
                _togFavorite.Click += OnToggleFavoriteClick;

                return view;
            }

            private ViewStates GetViewStateForViewGroupDiscount()
            {
                if (_product.HasDiscount())
                {
                    return ViewStates.Visible;
                }

                return ViewStates.Invisible;
            }

            private void OnBtnDecreaseClick(object sender, EventArgs e)
            {
                _adapter.OnProductQuantityDecrease?.Invoke(_product);
            }

            private void OnBtnIncreaseClick(object sender, EventArgs e)
            {
                _adapter.OnProductQuantityIncrease?.Invoke(_product);
            }

            private void OnToggleFavoriteClick(object sender, EventArgs e)
            {
                _adapter.OnToggleFavorite?.Invoke(_product);
            }

            public ListViewRowType GetViewType()
            {
                return ListViewRowType.ListItem;
            }

            public int GetId()
            {
                return _product.Id;
            }

            public void UpdateItem(object item)
            {
                if (item is Product product)
                {
                    _product = product;
                }
            }
        }

        private class ProductHeader : IListViewItem
        {
            private string _name;

            public ProductHeader(string name)
            {
                _name = name;
            }

            public View GetView(LayoutInflater inflater, View convertView)
            {
                var view = inflater.Inflate(Resource.Layout.product_header, null);

                view.FindViewById<TextView>(Resource.Id.txtName).Text = _name;

                return view;
            }

            public ListViewRowType GetViewType()
            {
                return ListViewRowType.HeaderItem;
            }

            public int GetId()
            {
                return 0;
            }

            public void UpdateItem(object item)
            {
                return;
            }
        }
    }

}