using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ServiceLoaderMedpomData.Interface
{
    internal interface IValidatorXML
    {
        event ErrorActionEvent Error;
        void Close();
        void Check(XmlReader reader);
    }
}
