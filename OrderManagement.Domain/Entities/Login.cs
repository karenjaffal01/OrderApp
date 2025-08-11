using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Entities
{
    public class Login
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        public string Password{ get; set; }

    }
}
