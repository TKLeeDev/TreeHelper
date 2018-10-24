using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKFIleTreeExporter.Models;

namespace TKFIleTreeExporter.Messages
{
    public class ExceptionListMsg : PubSubEvent<ExceptionListWithName>
    {

    }
}
