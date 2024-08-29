using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadowboxer.Models
{
    internal sealed class User
    {
        [Key]
        public long Id { get; set; }

        public User(long id)
        {
            Id = id;
        }
    }
}
