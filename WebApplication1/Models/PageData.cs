using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Models
{
    public class PageData
    {
        public string NewSearchExpression { get; set; }
        public string NewSearchHost { get; set; }

        public string PreviousSearchExpression { get; set; }
        public string PreviousSearchHost { get; set; }

        public string Expression { get; set; }

        public int PageNumber { get; set; }

        public List<History> Histories { get; set; }

        public int PageCount { get; set; }
        public int NotesPerPage = 10;
        public int TotalPages { get; set; }
        public int Count { get; set; }

        public StringBuilder AnswerFilter { get; set; }
    }
}
