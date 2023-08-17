using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string KnownAs { get; set; }
        public DateTime AccountCreated { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public List<Photo>  Photos { get; set; } = new List<Photo>();
        // or
        // public List<Photo>  Photos { get; set; } = new();


        // Commenting the func to optimize automapper quering.. To reduce the load from database query and exclude passwordhash and password salt from query
        // public int GetAge()
        // {
        //     return DateOfBirth.CalculateAge();
        // }


        public List<UserLike> LikedByUsers { get; set; }
        public List<UserLike> LikedUsers { get; set; }

        public List<Message> MessagesSent { get; set; }
        public List<Message> MessagesReceived { get; set; }

    }
}