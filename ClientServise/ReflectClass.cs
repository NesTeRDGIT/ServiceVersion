using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ClientServise
{

    class ReflectClass
    {  // Данный метод выводит информацию о содержащихся в классе методах
        public static MethodInfo[] MethodReflectInfo<T>() where T : class
        {
            Type t = typeof(T);
            // Получаем коллекцию методов
            MethodInfo[] MArr = t.GetMethods();
            return MArr;
        }
    }
}
