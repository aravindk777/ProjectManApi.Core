using System;
using System.Collections.Generic;
using System.Text;

namespace PM.BL.Extensions
{
    public static class ViewModelToData<T, Y> where T: class, Y where Y : class
    {
        public static Y Convert(T input)
        {
            return null;
        }
    }
}
