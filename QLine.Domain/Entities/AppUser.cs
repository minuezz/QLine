using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain;
using QLine.Domain.Enums;

namespace QLine.Domain.Entities
{
    public class AppUser
    {
        public Guid Id { get; private set; }

        public string Email { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;

        public string PasswordHash { get; private set; } = null!;

        public UserRole Role { get; private set; }

        private AppUser() { }

        private AppUser(
            Guid id,
            string email,
            string firstName,
            string lastName,
            string passwordHash,
            UserRole role)
        {
            if (id == Guid.Empty) throw new DomainException("AppUser Id cannot be empty.");
            if (string.IsNullOrWhiteSpace(email)) throw new DomainException("AppUser Email is required.");
            if (string.IsNullOrWhiteSpace(firstName)) throw new DomainException("AppUser FirstName is required.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new DomainException("AppUser LastName is required.");
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("AppUser PasswordHash is required.");

            Id = id;
            Email = email.Trim();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PasswordHash = passwordHash.Trim();
            Role = role;
        }

        public static AppUser Create(
            Guid id,
            string email,
            string firstName,
            string lastName,
            string passwordHash,
            UserRole role)
            => new(id, email, firstName, lastName, passwordHash, role);

        public void UpdateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) 
                throw new DomainException("AppUser Email is required.");
            Email = email.Trim();
        }

        public void UpdateName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("AppUser FirstName is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("AppUser LastName is required.");
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("AppUser PasswordHash is required.");

            PasswordHash = passwordHash.Trim();
        }

        public void ChangeRole(UserRole role) => Role = role;

        public string FullName => $"{FirstName} {LastName}";
    }
}
