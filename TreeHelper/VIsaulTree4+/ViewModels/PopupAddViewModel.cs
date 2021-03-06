﻿using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKFIleTreeExporter.Models;
using TKFIleTreeExporter.Views;

namespace TKFIleTreeExporter.ViewModels
{
    class PopupAddViewModel : BindableBase
    {
        public ExceptionListViewModel CallerViewModel { get; set; }

        private string _errMsg;
        public string ErrMsg
        {
            get { return _errMsg; }
            set { SetProperty(ref _errMsg, value); }
        }

        private string _addItem;
        public string AddItem
        {
            get { return _addItem; }
            set { SetProperty(ref _addItem, value); }
        }
        public DelegateCommand AddItemCommand { get; private set; }

        public PopupAddViewModel()
        {
            AddItemCommand = new DelegateCommand(AcceptAddItem, CanExcuteAcceptAddItem)
                .ObservesProperty(() => AddItem);
        }

        private void AcceptAddItem()
        {
            CallerViewModel.AddItemFromPopup = AddItem;

        }

        private bool CanExcuteAcceptAddItem()
        {
            //형태 체크


            //글자가 없을 경우.
            if (string.IsNullOrEmpty(AddItem))
            {
                ErrMsg = "";
                return false;
            }


            ////DirectoryName
            if (CallerViewModel.thisControlName == "Directory")
            {
                string str = @"[~!@\#$%^&*\()\=+|\\/:;?""<>']";
                System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);
                if (rex.IsMatch(AddItem))
                {
                    ErrMsg = "Does it contain special characters.\n" + @"(.[~!@\#$%^&*\()\=+|\\/:;?""<>'])";

                    return false;
                }
                ErrMsg = "";
                return true;
            }

            //FileName
            if (CallerViewModel.thisControlName == "File")
            {
                if (!AddItem.Contains("."))
                {
                    ErrMsg = "Does not contain Extension.\ne.g.\"abc.txt\"";
                    return false;
                }

                string str = @"[~!@\#$%^&*\()\=+|\\/:;?""<>']";
                System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);
                if (rex.IsMatch(AddItem))
                {
                    ErrMsg = "Does it contain special characters.\n" + @"([~!@\#$%^&*\()\=+|\\/:;?""<>'])";

                    return false;
                }
                ErrMsg = "";
                return true;
            }

            //Extention
            if (CallerViewModel.thisControlName == "Extension")
            {  //특수문자확인.
                string str = @"[~!@\#$%^&\()\=+|\\/:;?""<>']";
                System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);
                if (rex.IsMatch(AddItem))
                {
                    ErrMsg = "Does it contain special characters.\n" + @"([~!@\*#$%^&\()\=+|\\/:;?""<>'])";

                    return false;
                }

                string[] testArr = AddItem.ToCharArray().Select(c => c.ToString()).ToArray();
                return true;
            }
            return true;
        }
    }
}
