using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKFIleTreeExporter.Models
{
    public class ExceptionListWithName
    {
        public string controlName;
        public ObservableCollection<ExceptionListModel> exceptionList;
    }
}
