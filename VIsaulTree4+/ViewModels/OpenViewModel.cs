using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKFIleTreeExporter.Messages;
using Winforms = System.Windows.Forms;

namespace TKFIleTreeExporter.ViewModels
{
    class OpenViewModel : BindableBase
    {
        private string _path;
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }


        string localFilepath { get; set; }
        IEventAggregator _ea;

        public OpenViewModel(IEventAggregator ea)
        {
            Path = "Path : ";
            _ea = ea;
        }

        private DelegateCommand _openCommand;
        public DelegateCommand OpenCommand =>
            _openCommand ?? (_openCommand = new DelegateCommand(ExecuteOpenCommand));

        void ExecuteOpenCommand()
        {
            Winforms.FolderBrowserDialog folderDialog = new Winforms.FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = false;
            folderDialog.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            Winforms.DialogResult result = folderDialog.ShowDialog();

            if (result == Winforms.DialogResult.OK)
            {
                Path = folderDialog.SelectedPath;
                _ea.GetEvent<PathMsg>().Publish(Path);
            }
        }
    }
}
