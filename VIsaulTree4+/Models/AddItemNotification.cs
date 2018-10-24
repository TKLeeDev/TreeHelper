using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKFIleTreeExporter.Models
{
    class AddItemNotification : Confirmation, IAddItemNotification
    {
        public string Description { get; set; }

        public string AddItem { get; set; }
      

        public AddItemNotification()
        {
            AddItem = null;
        }
    }
}
