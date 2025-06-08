using System.ComponentModel.DataAnnotations;

namespace ProjectThread.Models
{
    public class Note
    {
        [Key]
        [ScaffoldColumn(false)]
        public int NoteId { get; set; }

        [ScaffoldColumn(false)]
        public Guid NoteGUID { get; set; } = Guid.NewGuid();
        public int UserID { get; set; }
        public string? Heading { get; set; }
        public string? Body { get; set; }

        [ScaffoldColumn(false)]
        public int IsDeleted { get; set; } = 0;

        [ScaffoldColumn(false)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

    }
}
