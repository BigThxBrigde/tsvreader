using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tsv.Attributes;
using Tsv.Service;

namespace Tsv.ViewModel
{
    [View(typeof(MainWindow))]
    public class MainViewModel : WindowViewModelBase
    {

        private string path;
        private bool expandAll = true;
        private ICommand clickCmd;
        private ICommand parseCmd;
        private ICommand openDetailCmd;
        private ICommand expandAllCmd;
        private DetailViewModel detail;
        private string header = "Collapse All";
        private ObservableCollection<TsvDataViewModel> data;

        /// <summary>
        /// List Data bind to view
        /// </summary>
        public ObservableCollection<TsvDataViewModel> Data
        {
            get => data;
            set => Set(() => Data, ref data, value);
        }

        public IDialogCoordinator DialogService
        {
            get => DialogCoordinator.Instance;
        }

        public string Header
        {
            get => header;
            set => Set(() => Header, ref header, value);
        }

        public bool ExpandAll
        {
            get => expandAll;
            set => Set(() => ExpandAll, ref expandAll, value);
        }

        public DetailViewModel Detail
        {
            get => detail;
            set => Set(() => Detail, ref detail, value);
        }


        /// <summary>
        /// File path
        /// </summary>
        public string Path
        {
            get => path;
            set => Set(() => Path, ref path, value);
        }

        public ICommand OpenDetailCmd => openDetailCmd ?? (openDetailCmd = new RelayCommand<object>((item) =>
        {
            var vm = item as TsvDataViewModel;
            if (vm == null)
            {
                return;
            }
            this.Detail = new DetailViewModel(vm);
            Detail.IsDetailWindowOpen = true;
        }));

        public ICommand ExpandAllCmd => expandAllCmd ?? (expandAllCmd = new RelayCommand(() =>
        {
            this.ExpandAll = !this.ExpandAll;
            this.Header = this.ExpandAll ? "Collapse All" : "Expand All";
        }));



        /// <summary>
        /// Click command
        /// </summary>
        public ICommand ClickCmd => clickCmd ?? (clickCmd = new RelayCommand(() =>
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Tab-separated values files|*.tsv"
            };
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                this.Path = openFileDialog.FileName;
            }
        }));


        /// <summary>
        /// Parse command
        /// </summary>
        public ICommand ParseCmd => parseCmd ?? (parseCmd = new RelayCommand(async () =>
        {
            await DoParseAsync();
        }, () => !string.IsNullOrEmpty(Path)));



        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

        }

        private async Task DoParseAsync()
        {
            // dialog
            var controller = await DialogService.ShowProgressAsync(this,
                "Please wait...",
                "Converting data, please waiting...",
                false,
                new MetroDialogSettings
                {
                    AnimateShow = true,
                    AnimateHide = true,
                    ColorScheme = MetroDialogColorScheme.Accented
                });

            controller.SetIndeterminate();

            var allData = new List<TsvDataViewModel>();

            var r = await Task.Factory.StartNew(() =>
            {
                var result = true;

                var reader = new TsvReader(path);

                Action<string, string> callback = (title, msg) =>
                 {
                     controller.SetCancelable(true);
                     controller.SetTitle(title);
                     controller.SetMessage(msg);

                     controller.Canceled += (o, e) =>
                     {
                         controller.CloseAsync();
                     };
                     result = false;
                 };

                var data = reader.Parse(callback);

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var vm = new TsvDataViewModel();
                        vm.Load(item);

                        allData.Add(vm);

                        SetData(vm);

                        vm.CalcPrice();

                        Logger.Debug($"Calculate price:{Environment.NewLine}{vm}");
                    }

                    allData = Ioc.Default.GetInstance<GroupEngine>().Parse(allData, callback);
                }
                return result;
            });

            if (r)
            {

                this.Data = new ObservableCollection<TsvDataViewModel>(allData);

                await controller.CloseAsync();
            }
        }

        private void SetData(TsvDataViewModel vm)
        {
            if (vm.Data.HasChildren)
            {
                foreach (var item in vm.Data.Children)
                {
                    var sub = new TsvDataViewModel();
                    sub.Load(item);

                    sub.Parent = vm;

                    vm.Children.Add(sub);

                    SetData(sub);

                    sub.CalcPrice();
                    Logger.Debug($"Calculate price while SetData:{Environment.NewLine}{sub}");
                }
            }
        }
    }
}