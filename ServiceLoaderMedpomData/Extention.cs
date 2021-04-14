using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceLoaderMedpomData
{
    public static class Extention
    {
        public static string FullError(this Exception ex)
        {
            string rzlt = ex.Message;
            if(ex.InnerException!=null)
                rzlt += "||"+ex.InnerException.FullError();           
            return rzlt;
        }
    }
}
