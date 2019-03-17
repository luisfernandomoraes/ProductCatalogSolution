using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ProductCatalogSolution.Core.Interfaces;

namespace ProductCatalog.Android.Services
{
    public class NavigationService : INavigationService
    {
        public void NavigateToCart()
        {
            var context = GetContext();
            var intent = GetNewIntent(context, typeof(CartActivity));

            NavigateToActivity(context, intent);
        }

        public void NavigateToDetailByProductId(int productId)
        {
            var context = GetContext();
            var intent = GetNewIntent(context, typeof(ProductDetailActivity));

            AddParameterToIntent(intent, ProductDetailActivity.PRODUCT_ID_PARAMETER, productId);

            NavigateToActivity(context, intent);
        }

        private Context GetContext()
        {
            return Application.Context;
        }

        private Intent GetNewIntent(Context context, Type activityType)
        {
            var intent = new Intent(context, activityType);
            intent.AddFlags(ActivityFlags.NewTask);

            return intent;
        }

        private void AddParameterToIntent(Intent intent, string parameterName, int parameterValue)
        {
            intent.PutExtra(parameterName, parameterValue);
        }

        private void NavigateToActivity(Context context, Intent intent)
        {
            context.StartActivity(intent);
        }
    }
}