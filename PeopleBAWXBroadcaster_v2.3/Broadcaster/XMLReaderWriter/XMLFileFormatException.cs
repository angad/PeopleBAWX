using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLReaderWriter
{
    class XMLFileFormatException : Exception
    {
        public XMLFileFormatException(string msg)
            : base(msg)
        {
            
        }

    }
}
