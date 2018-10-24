using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKFIleTreeExporter.Models
{
    public interface IAddItemNotification : IConfirmation
    {
        string Description { get; set; }
        string AddItem { get; set; }
    }
}
