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
        internal Response(int userId, string msg) : base(userId, msg) { }
        internal Response(int userId, T value) : base(userId)
        {
            this.Value = value;
        }
        internal Response(int userId, T value, string msg) : base(userId, msg)
        {
            this.Value = value;
        }
    }
}
