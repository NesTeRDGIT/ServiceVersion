using System;

namespace ServiceLoaderMedpomData
{
    public static class Extension
    {
        public static string FullError(this Exception ex)
        {
            var rzlt = ex.Message;
            if(ex.InnerException!=null)
                rzlt += "||"+ex.InnerException.FullError();           
            return rzlt;
        }
    }
}
