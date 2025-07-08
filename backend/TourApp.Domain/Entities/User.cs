using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Domain.Entities
{
    public abstract class User
    {
        public Guid Id { get; protected set; }
        public string Username { get; protected set; }
        public string Email { get; protected set; }
        public string PasswordHash { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public bool IsBlocked { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected User() { }

        protected User(string username, string email, string passwordHash,
                      string firstName, string lastName)
        {
            Id = Guid.NewGuid();
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            IsBlocked = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void Block()
        {
            IsBlocked = true;
        }

        public void Unblock()
        {
            IsBlocked = false;
        }
    }
}
