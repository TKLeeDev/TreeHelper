using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKFIleTreeExporter.Models
{
    public class Files
    {
        private int column;
        public string name;

        public string extension { get; set; }

        public Files(string name, int column,string extension)
        {
            this.column = column;
            this.name = name;
            this.extension = extension;
        }

        public override string ToString()
        {
            return new String(',', column) + name;
        }
    }
}
