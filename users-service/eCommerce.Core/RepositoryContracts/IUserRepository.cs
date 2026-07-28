using eCommerce.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Core.RepositoryContracts;
/// <summary>
/// Contract to be implemented by UserRepository that contains data access
/// logic of users data store
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Method to add a user to the data store and return the added user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<ApplicationUser?> AddUser(ApplicationUser user);
    /// <summary>
    /// Method to retrieve existing user by their emial and password
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<ApplicationUser?> GetUserByEmailAndPassword(string? email, string? password);

    /// <summary>
    /// Returns the users data based on the given user ID
    /// </summary>
    /// <param name="userID">User ID to search</param>
    /// <returns>ApplicationUser object that matches with given UserID</returns>
    Task<ApplicationUser?> GetUserByUserID(Guid? userID);
}
