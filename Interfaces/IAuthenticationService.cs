using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AltunPital.Data;

namespace AltunPital.Interfaces;

public interface IAuthenticationService
{
    Patient RegisterPatient(
        string firstName,
        string lastName,
        string email,
        string phone,
        string password);

    Patient RegisterPatient(
        string firstName,
        string lastName,
        string email,
        string phone,
        string city,
        string password);

    User Login(
        string email,
        string password);

    string GetRole(User user);
}