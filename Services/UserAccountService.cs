using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;
using AltunPital.Security;

namespace AltunPital.Services;

public class UserAccountService : IUserAccountService
{
    private readonly AppDbContext _db;

    public UserAccountService(
        AppDbContext db)
    {
        _db = db;
    }


    public User GetUser(
        int userId)
    {
        return _db.Users
            .FirstOrDefault(u =>
                u.UserId == userId)
            ?? throw new Exception(
                "Kullanıcı bulunamadı.");
    }


    public void UpdateProfile(
        int userId,
        string firstName,
        string lastName,
        string email,
        string phone)
    {
        var user =
            GetUser(userId);

        firstName =
            firstName.Trim();

        lastName =
            lastName.Trim();

        email =
            email.Trim()
                .ToLowerInvariant();

        phone =
            phone.Trim();


        if (string.IsNullOrWhiteSpace(
                firstName))
        {
            throw new Exception(
                "Ad boş bırakılamaz.");
        }

        if (string.IsNullOrWhiteSpace(
                lastName))
        {
            throw new Exception(
                "Soyad boş bırakılamaz.");
        }

        if (string.IsNullOrWhiteSpace(
                email))
        {
            throw new Exception(
                "E-posta boş bırakılamaz.");
        }


        bool emailExists =
            _db.Users.Any(u =>
                u.UserId != userId &&
                u.Email == email);

        if (emailExists)
        {
            throw new Exception(
                "Bu e-posta başka bir kullanıcı tarafından kullanılıyor.");
        }


        user.FirstName =
            firstName;

        user.LastName =
            lastName;

        user.Email =
            email;

        user.Phone =
            phone;


        _db.SaveChanges();
    }


    public void ChangePassword(
        int userId,
        string currentPassword,
        string newPassword)
    {
        var user =
            GetUser(userId);


        if (string.IsNullOrWhiteSpace(
                newPassword) ||
            newPassword.Length < 6)
        {
            throw new Exception(
                "Yeni şifre en az 6 karakter olmalıdır.");
        }


        bool currentPasswordCorrect;

        if (PasswordHasher.IsHashed(
                user.PasswordHash))
        {
            currentPasswordCorrect =
                PasswordHasher.VerifyPassword(
                    currentPassword,
                    user.PasswordHash);
        }
        else
        {
            currentPasswordCorrect =
                user.PasswordHash ==
                currentPassword;
        }


        if (!currentPasswordCorrect)
        {
            throw new Exception(
                "Mevcut şifre yanlış.");
        }


        user.PasswordHash =
            PasswordHasher.HashPassword(
                newPassword);

        _db.SaveChanges();
    }
}