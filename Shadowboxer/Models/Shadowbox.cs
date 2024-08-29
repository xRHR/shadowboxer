using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Shadowboxer.Models
{
    internal sealed class Shadowbox
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string ApiUrl { get; set; }

        [Required]
        public long OwnerId { get; set; }

        public User Owner { get; set; }
    }
}
