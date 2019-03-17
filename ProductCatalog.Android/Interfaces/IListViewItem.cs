using Android.Views;
using ProductCatalog.Android.Enums;

namespace ProductCatalog.Android.Interfaces
{
    public interface IListViewItem
    {
        int GetId();
        void UpdateItem(object item);
        ListViewRowType GetViewType();
        View GetView(LayoutInflater inflater, View convertView);
    }
}
