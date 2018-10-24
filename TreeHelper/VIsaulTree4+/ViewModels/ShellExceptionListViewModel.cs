using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKFIleTreeExporter.Infra;
using TKFIleTreeExporter.Messages;
using TKFIleTreeExporter.Models;
using Winforms = System.Windows.Forms;

namespace TKFIleTreeExporter.ViewModels
{
    //ExceptionList 저장 및 로드 기능.
    class ShellExceptionListViewModel : BindableBase
    {

        private List<ExceptionListModel> exceptionDirectoryList = new List<ExceptionListModel>();
        private List<ExceptionListModel> exceptionFileList = new List<ExceptionListModel>();
        private List<ExceptionListModel> exceptionExtensionList = new List<ExceptionListModel>();

        IEventAggregator _ea;
        public ShellExceptionListViewModel(IEventAggregator ea)
        {
            _ea = ea;
            ea.GetEvent<ExceptionListMsg>().Subscribe(SubscribeFunction, ThreadOption.PublisherThread);
        }

        private void SubscribeFunction(ExceptionListWithName eLN)
        {
            if (eLN.controlName.Contains("Directory"))
                exceptionDirectoryList = new List<ExceptionListModel>(eLN.exceptionList);
            if (eLN.controlName.Contains("File"))
                exceptionFileList = new List<ExceptionListModel>(eLN.exceptionList);
            if (eLN.controlName.Contains("Extension"))
                exceptionExtensionList = new List<ExceptionListModel>(eLN.exceptionList);
        }



        private bool canexcuteFlag { get; set; }

        private DelegateCommand _LoadCommand;
        public DelegateCommand LoadCommand =>
            _LoadCommand ?? (_LoadCommand = new DelegateCommand(ExecuteLoadCommand));

        void ExecuteLoadCommand()
        {
            //Regionmanager로 이름 기준 찾아서 실행해도되지만, 여기서는 EventAggregator방식사용.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "tktree file (*.tktree) | *.tktree";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == Winforms.DialogResult.OK)
            {
                ExceptionListWithName eLN = new ExceptionListWithName();

                 ObservableCollection<ExceptionListModel> exceptionListDirectory = new ObservableCollection<ExceptionListModel>();
                ObservableCollection<ExceptionListModel> exceptionListFile = new ObservableCollection<ExceptionListModel>();
                ObservableCollection<ExceptionListModel> exceptionListExtension = new ObservableCollection<ExceptionListModel>();

                ExceptionListModel exceptionListModel;

                string[] strArr = File.ReadAllLines(openFileDialog.FileName);



                string[] tmp = new string[2];
                for (int i = 0; i < strArr.Count(); i++)
                {
                    //제목
                    tmp = strArr[i].Split('^');
                    if (tmp[0] == "d")
                    {
                        exceptionListModel = new ExceptionListModel();
                        exceptionListModel.ExceptionString = tmp[1];
                        exceptionListDirectory.Add(exceptionListModel);

                    }
                    else if (tmp[0] == "f")
                    {
                        exceptionListModel = new ExceptionListModel();
                        exceptionListModel.ExceptionString = tmp[1];
                        exceptionListFile.Add(exceptionListModel);
                    }

                    else if (tmp[0] == "e")
                    {
                        exceptionListModel = new ExceptionListModel();
                        exceptionListModel.ExceptionString = tmp[1];
                        exceptionListExtension.Add(exceptionListModel);
                    }
                }


         
                eLN.controlName = ItemControlNames.ItemName_Directory;
                eLN.exceptionList = exceptionListDirectory;
                _ea.GetEvent<LoadExceptionListMsg>().Publish(eLN);

                eLN.controlName = ItemControlNames.ItemName_File;
                eLN.exceptionList = exceptionListFile;
                _ea.GetEvent<LoadExceptionListMsg>().Publish(eLN);

                eLN.controlName = ItemControlNames.ItemName_Extension;
                eLN.exceptionList = exceptionListExtension;          
                _ea.GetEvent<LoadExceptionListMsg>().Publish(eLN);
            }
        }












        private DelegateCommand _SaveComnmand;
        public DelegateCommand SaveCommand =>
            _SaveComnmand ?? (_SaveComnmand = new DelegateCommand(ExecuteSaveCommand));

        void ExecuteSaveCommand()
        {
            //다이얼로그 열기
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "tktree file (*.tktree) | *.tktree";
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == Winforms.DialogResult.OK)
            {
                //Build String
                //타이틀3개 + ..
                List<string> buildString = new List<string>();


                foreach (var item in exceptionDirectoryList)
                {
                    buildString.Add("d^" + item.ExceptionString);
                }

                foreach (var item in exceptionFileList)
                {
                    buildString.Add("f^" + item.ExceptionString);
                }

                foreach (var item in exceptionExtensionList)
                {
                    buildString.Add("e^" + item.ExceptionString);
                }

                string[] strArr = new string[buildString.Count];
                for (int i = 0; i < buildString.Count; i++)
                {
                    strArr[i] = buildString[i].ToString();
                }

                // 저장
                File.WriteAllLines(saveFileDialog.FileName, strArr);
            }
            
        }
    }
}
