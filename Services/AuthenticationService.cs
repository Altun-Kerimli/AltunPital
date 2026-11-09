using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;
using AltunPital.Interfaces;
using AltunPital.Security;

namespace AltunPital.Services;

public class AuthenticationService
    : IAuthenticationService
{
    private readonly AppDbContext _db;

    public AuthenticationService(
        AppDbContext db)
    {
        _db = db;
    }

    public Patient RegisterPatient(
        string firstName,
        string lastName,
        string email,
        string phone,
        string password)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new Exception(
                "Ad boş bırakılamaz.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new Exception(
                "Soyad boş bırakılamaz.");

        if (string.IsNullOrWhiteSpace(email))
            throw new Exception(
                "E-posta boş bırakılamaz.");

        if (string.IsNullOrWhiteSpace(password))
            throw new Exception(
                "Şifre boş bırakılamaz.");

        if (password.Length < 4)
            throw new Exception(
                "Şifre en az 4 karakter olmalıdır.");

        email = email.Trim().ToLowerInvariant();

        bool exists =
            _db.Users.Any(u =>
                u.Email.ToLower() == email);

        if (exists)
            throw new Exception(
                "Bu e-posta zaten kullanılıyor.");

        var patient = new Patient
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),

            Email = email,
            Phone = phone.Trim(),

            PasswordHash =
                PasswordHasher
                    .HashPassword(password)
        };

        _db.Patients.Add(patient);
        _db.SaveChanges();

        return patient;
    }


    public User Login(
        string email,
        string password)
    {
        email =
            email.Trim().ToLowerInvariant();

        var user =
            _db.Users.FirstOrDefault(u =>
                u.Email.ToLower() == email);

        if (user == null)
            throw new Exception(
                "E-posta veya şifre hatalı.");

        bool valid;

        if (PasswordHasher.IsHashed(
                user.PasswordHash))
        {
            valid =
                PasswordHasher.VerifyPassword(
                    password,
                    user.PasswordHash);
        }
        else
        {
            // Eski seed kullanıcıları için.
            valid =
                user.PasswordHash == password;

            // Başarılı eski girişte
            // şifre otomatik güvenli hale gelir.
            if (valid)
            {
                user.PasswordHash =
                    PasswordHasher
                        .HashPassword(password);

                _db.SaveChanges();
            }
        }

        if (!valid)
            throw new Exception(
                "E-posta veya şifre hatalı.");

        return user;
    }


    public string GetRole(User user)
    {
        if (_db.Managers.Any(m =>
                m.UserId == user.UserId))
            return "Manager";

        if (_db.Doctors.Any(d =>
                d.UserId == user.UserId))
            return "Doctor";

        if (_db.HealthStaff.Any(s =>
                s.UserId == user.UserId))
            return "HealthStaff";

        if (_db.Patients.Any(p =>
                p.UserId == user.UserId))
            return "Patient";

        return "Unknown";
    }


}