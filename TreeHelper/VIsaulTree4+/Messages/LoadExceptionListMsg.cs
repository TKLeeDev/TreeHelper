using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKFIleTreeExporter.Models;

namespace TKFIleTreeExporter.Messages
{
    public class LoadExceptionListMsg : PubSubEvent<ExceptionListWithName>
    {
    }
}
