using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace ServiceLoaderMedpomData
{
    public class ReflectClass
    {  // Данный метод выводит информацию о содержащихся в классе методах
        public static MethodInfo[] MethodReflectInfo<T>() where T : class
        {
            var t = typeof(T);
            // Получаем коллекцию методов
            var MArr = t.GetMethods();
            return MArr;
        }
    }
}
