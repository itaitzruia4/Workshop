using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.ServiceLayer
{
    public class Response
    {
        public readonly string ErrorMessage;
        public bool ErrorOccured { get => ErrorMessage != null; }
        public int UserId;
        internal Response(int userId) { UserId = userId; }
        internal Response(int userId, string msg)
        {
            ErrorMessage = msg;
            UserId = userId;
        }
    }
}
