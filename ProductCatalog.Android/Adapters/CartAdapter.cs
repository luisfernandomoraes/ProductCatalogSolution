using System.Collections.Generic;
using System.Globalization;
using Android.Content;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using ProductCatalogSolution.Core.Models;

namespace ProductCatalog.Android.Adapters
{
    public class CartAdapter : BaseAdapter<Product>
    {
        private readonly LayoutInflater _inflater;
        private readonly IList<Product> _products;

        public CartAdapter(Context context, IList<Product> products)
        {
            _inflater = LayoutInflater.FromContext(context);
            _products = products;
        }

        public override Product this[int position] => _products[position];

        public override int Count => _products.Count;

        public override long GetItemId(int position)
        {
            return _products[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = _inflater.Inflate(Resource.Layout.cart_product_item, null);

            var imgPhoto = view.FindViewById<ImageViewAsync>(Resource.Id.imgPhoto);
            var txtName = view.FindViewById<TextView>(Resource.Id.txtName);
            var txtTotalPrice = view.FindViewById<TextView>(Resource.Id.txtTotalPrice);
            var txtQuantity = view.FindViewById<TextView>(Resource.Id.txtQuantity);
            var txtDiscount = view.FindViewById<TextView>(Resource.Id.txtDiscount);
            var viewGroupDiscount = view.FindViewById<LinearLayout>(Resource.Id.viewGroupDiscount);

            var product = _products[position];

            ImageService
                .Instance
                .LoadUrl(product.Photo)
                .DownSample(55, 55)
                .Into(imgPhoto);

            txtName.Text = product.Name;

            var culture = CultureInfo.CreateSpecificCulture("pt-BR");
            

            txtTotalPrice.Text = product.GetTotalPrice().ToString("C", culture);
            txtQuantity.Text = $"{product.Quantity.ToString()} UN";
            txtDiscount.Text = $"{product.Discount.ToString()} %";
            viewGroupDiscount.Visibility = GetViewStateForViewGroupDiscount(product);

            return view;
        }

        private ViewStates GetViewStateForViewGroupDiscount(Product product)
        {
            if (product.HasDiscount())
            {
                return ViewStates.Visible;
            }

            return ViewStates.Invisible;
        }
    }
}
