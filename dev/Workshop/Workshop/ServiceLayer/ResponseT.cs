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
        internal Response(string msg, int userId) : base(msg, userId) { }
        internal Response(T value, int userId) : base(userId)
        {
            this.Value = value;
        }
        internal Response(T value, string msg, int userId) : base(msg, userId)
        {
            this.Value = value;
        }
    }
}
