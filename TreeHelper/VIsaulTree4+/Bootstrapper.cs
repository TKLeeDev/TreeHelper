using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;
using System.Windows;
using TKFIleTreeExporter.Infra;
using TKFIleTreeExporter.ViewModels;
using TKFIleTreeExporter.Views;

namespace TKFIleTreeExporter
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            var regionManager = this.Container.Resolve<IRegionManager>();


            regionManager.RegisterViewWithRegion(RegionNames.Region_Top, typeof(OpenView));

            regionManager.RegisterViewWithRegion(RegionNames.Region_Middle, typeof(ShellExceptionListView));

            var view = this.Container.Resolve<ExceptionListView>();
            var vm = view.DataContext as ExceptionListViewModel;
            vm.Description = "Filter Directories";
            vm.thisControlName = ItemControlNames.ItemName_Directory;
            regionManager.Regions[RegionNames.Region_ListRegion].Add(view, ItemControlNames.ItemName_Directory);

            view = Container.Resolve<ExceptionListView>();
            vm = view.DataContext as ExceptionListViewModel;
            vm.Description = "Filter Files";
            vm.thisControlName = ItemControlNames.ItemName_File;
            regionManager.Regions[RegionNames.Region_ListRegion].Add(view, ItemControlNames.ItemName_File);

            view = Container.Resolve<ExceptionListView>();
            vm = view.DataContext as ExceptionListViewModel;
            vm.Description = "Filter Extensions";
            vm.thisControlName = ItemControlNames.ItemName_Extension;
            regionManager.Regions[RegionNames.Region_ListRegion].Add(view, ItemControlNames.ItemName_Extension);


            regionManager.RegisterViewWithRegion(RegionNames.Region_Bottom, typeof(SaveView));

            Application.Current.MainWindow.Show();


        }
    }


}
