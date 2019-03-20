using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.BL.Common
{
    public interface ICommonLogic<T> where T: class
    {
        int Count();
        IEnumerable<T> Search(string keyword, bool exactMatch = false, string fieldType = "");
    }
}
