using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkTest.Domain
{
    public class Users
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int Score { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public int Age { get; set; }
        [StringLength(100)]
        public string Address { get; set; }
        [StringLength(100)]
        public string NickName { get; set; }
    }

}
