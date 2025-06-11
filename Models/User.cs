using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectThread.Models
{
    public class User
    {
        [Key]
        [ScaffoldColumn(false)]
        public int UserID { get; set; }

        [ScaffoldColumn(false)]
        public Guid UserGUID { get; set; } = Guid.NewGuid();

        public string? Name { get; set; }
        
        public string? Gender { get; set; }

        public DateTime? Dob { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address1 { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        [ScaffoldColumn(false)]
        public int IsDeleted { get; set; } = 0;

        [ScaffoldColumn(false)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime LastLogin { get; set; } 
        public List<Friend>? Friends { get; set; } = new List<Friend>();
    }

    public class Friend
    {
        [Key]
        public int FriendID { get; set; }
        public int UserID { get; set; }
        public int FriendUserID { get; set; }
        public string? FriendName { get; set; }
        public int IsBlocked { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
     public class FriendRequest
    {
        [Key]
        public int RequestID { get; set; }
        public int UserID { get; set; }
        public int FriendUserID { get; set; }
        public string? FriendName { get; set; }
        public int IsConfirm { get; set; } = 0;
        public int IsDeclined { get; set; } = 0;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }

    public class Thread
    {
        [Key]
        public int ThreadID { get; set; }
        public Guid ThreadGUID { get; set; }
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string? ReceiverName { get; set; }
        public string? ThreadBody { get; set; }
        public int IsDeleted { get; set; } = 0;
        //User-Direction=0
        public int Direction { get; set; } = 0;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }




}
