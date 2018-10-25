using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TKFIleTreeExporter.Messages;
using TKFIleTreeExporter.Models;
using TKFIleTreeExporter.Views;

namespace TKFIleTreeExporter.ViewModels
{
    class ExceptionListViewModel : BindableBase
    {

        #region SetUI


        #endregion

        PopupAddView popupAdd;
        /*Publish는 총 3개, 추가시, 제거시, 파일로드시*/
        ExceptionListWithName exceptionListWithName = new ExceptionListWithName();

        public string thisControlName { get; set; }
        //public InteractionRequest<IAddItemNotification> AddItemInteractionRequest { get; set; } = new InteractionRequest<IAddItemNotification>();
        IEventAggregator _ea;

        public ExceptionListViewModel(IEventAggregator ea)
        {
            
            _ea = ea;
            // ??? + save에서 MVVM모드일때 오는 aggregator 하나는??
            _ea.GetEvent<LoadExceptionListMsg>().Subscribe(LoadSubscribeFunction, ThreadOption.PublisherThread);

        }

        //SaveViewModel에 현재 컨트롤의 이름(Derectory, File, Extension)과 함께 List보낼 객체.(로드시)
        private void LoadSubscribeFunction(ExceptionListWithName eLn)
        {
            if (eLn.controlName == this.thisControlName)
            {
                exceptionList = eLn.exceptionList;

                _ea.GetEvent<ExceptionListMsg>().Publish(eLn);
            }

        }

        private ObservableCollection<ExceptionListModel> _exceptionList = new ObservableCollection<ExceptionListModel>();
        public ObservableCollection<ExceptionListModel> exceptionList
        {
            get { return _exceptionList; }
            set { SetProperty(ref _exceptionList, value); }
        }


        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private DelegateCommand _addCommand;
        public DelegateCommand AddCommand =>
            _addCommand ?? (_addCommand = new DelegateCommand(ExecuteAddCommand));
        void ExecuteAddCommand()
        {

            popupAdd = new PopupAddView();
            var vm = popupAdd.DataContext as PopupAddViewModel;
            vm.CallerViewModel = this;
            popupAdd.Show();
            
        }

        public string AddItemFromPopup
        {
            set
            {
                if (exceptionList.Any(p => p.ExceptionString == value))
                {
                    MessageBox.Show("\"" + value + "\"Already exist string.");
                }
                else
                {
                    exceptionList.Add(new ExceptionListModel() { ExceptionString = value });

                    //현재 List의 이름 (디렉토리, 파일, 확장자) 이름과 함께 Save에 넘겨준다.
                    //Save에서 구분하기 위해서.
                    exceptionListWithName.exceptionList = exceptionList;
                    exceptionListWithName.controlName = thisControlName;
                    _ea.GetEvent<ExceptionListMsg>().Publish(exceptionListWithName);

                    //값이 들어올경우 팝업을 닫는다.
                    popupAdd.Close();
                }
            }
        }


        private DelegateCommand<string> _removeCommand;
        public DelegateCommand<string> RemoveCommand =>
            _removeCommand ?? (_removeCommand = new DelegateCommand<string>(ExecuteRemoveCommand));
        void ExecuteRemoveCommand(string param)
        {
            //Single()로 Exception처리. 
            exceptionList.Remove(exceptionList.Where(i => i.ExceptionString == param).Single());
            exceptionListWithName.exceptionList = exceptionList;
            exceptionListWithName.controlName = thisControlName;
            _ea.GetEvent<ExceptionListMsg>().Publish(exceptionListWithName);
        }
    }
}
