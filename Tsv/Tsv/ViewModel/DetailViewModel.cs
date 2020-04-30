using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tsv.ViewModel
{
    public class DetailViewModel : ViewModel
    {
        private TsvDataViewModel tsvData;
        private bool isDetailWindowOpen;
        private ICommand closeCmd;

        public TsvDataViewModel TsvData => this.tsvData;

        public ICommand CloseCmd => closeCmd ?? (closeCmd = new RelayCommand(() =>
        {
            this.IsDetailWindowOpen = false;
        }));

        public bool IsDetailWindowOpen
        {
            get => isDetailWindowOpen;

            set => Set(() => IsDetailWindowOpen, ref isDetailWindowOpen, value);
        }


        public DetailViewModel(TsvDataViewModel vm)
        {
            this.tsvData = vm;
        }

    }
}
