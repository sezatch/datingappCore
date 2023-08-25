using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Connection
    {
        public Connection()
        {
        }

        public Connection(string connectionId, string username)
        {
            Username = username;
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}