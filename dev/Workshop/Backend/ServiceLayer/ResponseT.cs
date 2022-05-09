using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.ServiceLayer
{
    public class Response<T>: Response
    {
        public readonly T Value;
        internal Response(string msg) : base(msg) { }
        internal Response(T value) : base()
        {
            this.Value = value;
        }
        internal Response(T value, string msg) : base(msg)
        {
            this.Value = value;
        }
    }
}
