using Microsoft.Office.Interop.Excel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
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
    class SaveViewModel : BindableBase
    {
        private List<Files> filesList = new List<Files>();

        private List<ExceptionListModel> exceptionDirectoryList = new List<ExceptionListModel>();
        private List<ExceptionListModel> exceptionFileList = new List<ExceptionListModel>();
        private List<ExceptionListModel> exceptionExtensionList = new List<ExceptionListModel>();

        private string _localPath;
        public string localPath
        {
            get { return _localPath; }
            set { SetProperty(ref _localPath, value); }
        }

        IEventAggregator _ea;
        public SaveViewModel(IEventAggregator ea)
        {
            _ea = ea;
            IsExportXls = true;
            ea.GetEvent<PathMsg>().Subscribe(SubscribePathFunc, ThreadOption.PublisherThread);
            ea.GetEvent<ExceptionListMsg>().Subscribe(SubscribeExceptionListFunc, ThreadOption.PublisherThread);

        }
        private void SubscribePathFunc(string msg)
        {
            //파일경로.
            localPath = msg;
        }

        private void SubscribeExceptionListFunc(ExceptionListWithName eLN)
        {
            //제외리스트 : 이름은 Bootstrapper에서 지정됨.
            // 하나의 뷰를 여러개로 인스터화해서 사용하기때문에 이름으로 구분함.
            if (eLN != null)
            {
                if (eLN.controlName.Contains(ItemControlNames.ItemName_Directory))
                    exceptionDirectoryList = new List<ExceptionListModel>(eLN.exceptionList);
                if (eLN.controlName.Contains(ItemControlNames.ItemName_File))
                    exceptionFileList = new List<ExceptionListModel>(eLN.exceptionList);
                if (eLN.controlName.Contains(ItemControlNames.ItemName_Extension))
                    exceptionExtensionList = new List<ExceptionListModel>(eLN.exceptionList);
            }
        }

        #region Binding Properties
        private bool _txt;
        public bool IsExportText
        {
            get { return _txt; }
            set
            {

                SetProperty(ref _txt, value);
              

            }
        }
        private bool _Csv;
        public bool IsExportCsv
        {
            get { return _Csv; }
            set
            {

                SetProperty(ref _Csv, value);

            }
        }

        private bool _xls;
        public bool IsExportXls
        {
            get { return _xls; }
            set
            {
                SetProperty(ref _xls, value);
                IsEnableMVVM = IsExportXls;
            }
        }



        private bool _mvvmenalbe;
        public bool IsEnableMVVM
        {
            get { return _mvvmenalbe; }
            set { SetProperty(ref _mvvmenalbe, value); }
        }
        #endregion

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand, CanexcuteSaveCommand)).ObservesProperty(() => localPath);

        private bool CanexcuteSaveCommand()
        {
            if (!string.IsNullOrEmpty(localPath))
                return true;
            return false;
        }

        void ExecuteSaveCommand()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (IsExportCsv)
                saveFileDialog.Filter = "CSV Excel (*.csv) | *.csv";
            else if (IsExportText)
                saveFileDialog.Filter = "Text file (*.txt) | *.txt";
            else if (IsExportXls)
                saveFileDialog.Filter = "Excel file (*.xlsx) | *.xlsx";

            //파일리스트 가져오기. 
            filesList.Clear();
            filesList.Add(new Files(System.IO.Path.GetFileName(localPath), 0, "Directory"));
            WriteFilesRec(localPath, 1, ref filesList);

            //다이얼로그 OK
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == Winforms.DialogResult.OK)
            {
                //WriteAllLines 위해 string[] 으로 형변환.
                string[] strArr = new string[filesList.Count];
                for (int i = 0; i < filesList.Count; i++)
                {
                    strArr[i] = filesList[i].ToString();
                }

                //파일일 경우 ,를 탭으로 변경.
                if (IsExportCsv || IsExportText)
                {
                    if (IsExportText)
                        for (int i = 0; i < strArr.Count(); i++)
                        {
                            strArr[i] = strArr[i].Replace(',', '\t');
                        }

                    try
                    {
                        File.WriteAllLines(saveFileDialog.FileName, strArr);
                    }
                    catch
                    {
                        MessageBox.Show("Fail : Close the csv file.");
                    }
                }

                if (IsExportXls)
                {

                    Microsoft.Office.Interop.Excel.Application app =
                            new Microsoft.Office.Interop.Excel.Application();

                    app.WindowState = XlWindowState.xlMaximized;

                    Workbook wb = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                    Worksheet ws = wb.Worksheets[1];
                    DateTime dt = DateTime.Now;


                    //일반 모드일경우

                    //셀 위치 결정.
                    for (int i = 0; i < strArr.Count(); i++)
                    {
                        //column = unicode활용, row = i + 1
                        int columnCount = 65;

                        columnCount += strArr[i].Split(',').Count() - 1;
                        char column = (char)columnCount;
                        string position = column.ToString() + (i + 1);

                        string[] tmpSplite = strArr[i].Split(',');

                        for (int j = 0; j < tmpSplite.Count(); j++)
                        {
                            if (tmpSplite[j] != ",")
                            {
                                //엑셀쓰기.
                                ws.Range[position].Value = tmpSplite[j];
                            }
                        }
                    }
                    //xlsx
                    try
                    {
                        ws.SaveAs(saveFileDialog.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Fail : Close the xls file.");
                    }

                }
            }
        }

        //파을일 읽고 리스트에 담는다.
        public void WriteFilesRec(string path, int i, ref List<Files> fileList)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            //디렉토리 경우 (제외리스트 : exceptionDirectoryList)
            foreach (var d in directory.GetDirectories())
            {
                if (!exceptionDirectoryList.Exists(e => e.ExceptionString == d.ToString()))
                {

                    fileList.Add(new Files(d.Name, i, ItemControlNames.ItemName_Directory));

                    WriteFilesRec(System.IO.Path.Combine(path, d.Name), i + 1, ref fileList);
                }
            }

            //파일 + 확장자 경우 (제외리스트 : exceptionFileList, exceptionExtensionList)
            foreach (var f in directory.GetFiles())
            {
                //확장자검사
                if (!exceptionExtensionList.Exists(e => e.ExceptionString == f.Extension))
                {
                    //파일명검사
                    if (!exceptionFileList.Exists(e => e.ExceptionString == f.ToString()))
                    {
                        //특수경우 : *.xaml.cs가 Exception리스트에 있는경우
                        if (exceptionFileList.Exists(e => e.ExceptionString.Contains(".xaml.cs")))
                        {
                            //*.xaml.cs를 '포함'한 경우 제외 하고 add
                            if (!f.ToString().Contains(".xaml.cs"))
                            {

                                fileList.Add(new Files(f.Name, i, f.Extension));
                            }
                            else if (f.ToString().Contains(".xaml.cs"))
                            {
                                //포함한파일이 있는 경우
                            }
                        }
                        else
                        {

                            fileList.Add(new Files(f.Name, i, f.Extension));
                        }
                    }
                }
            }
        }
    }
}

