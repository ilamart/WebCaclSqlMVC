using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("History")]
    public class History
    {
        public int ID { get; set; }
        public string Expression { get; set; }
        public string Result { get; set; }
        public string Host { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
