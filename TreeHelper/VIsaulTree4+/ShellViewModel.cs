using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKFIleTreeExporter
{
    public class ShellViewModel : BindableBase
    {
        private DelegateCommand _closeWindow;
        public DelegateCommand CloseWindowCommand =>
            _closeWindow ?? (_closeWindow = new DelegateCommand(ExecuteCloseWindowCommand));

        void ExecuteCloseWindowCommand()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
