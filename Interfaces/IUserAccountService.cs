using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.Interfaces;

public interface IUserAccountService
{
    User GetUser(int userId);

    void UpdateProfile(int userId, string firstName, string lastName, string email, string phone);

    void ChangePassword(int userId, string currentPassword, string newPassword);
}
