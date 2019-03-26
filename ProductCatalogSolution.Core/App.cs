using MvvmCross.Platform.IoC;
using ProductCatalogSolution.Core.ViewModels;

namespace ProductCatalogSolution.Core
{
    public class App: MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterNavigationServiceAppStart<CartViewModel>();
            RegisterNavigationServiceAppStart<ProductDetailViewModel>();
            RegisterNavigationServiceAppStart<StoreViewModel>();
        }
    }
}
