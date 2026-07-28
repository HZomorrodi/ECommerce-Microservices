using Dapper;
using eCommerce.Core.Entity;
using eCommerce.Core.RepositoryContracts;
using eCommerce.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Infrastructure.Repositories;

internal class UserRepository(DapperDbContext context) : IUserRepository
{
    private readonly DapperDbContext context = context;

    public async Task<ApplicationUser?> AddUser(ApplicationUser user)
    {
        user.UserId = Guid.NewGuid();
        var query = @"
        INSERT INTO ""Users"" (
            ""UserId"", 
            ""Email"", 
            ""Password"", 
            ""PersonName"", 
            ""Gender""
        ) VALUES (
            @UserId, 
            @Email, 
            @Password, 
            @PersonName, 
            @Gender
        )";
        using IDbConnection connection = context.Connection;
        int rowsAffected = await connection.ExecuteAsync(query, user);

        return rowsAffected > 0 ? user : null;
    }

    public async Task<ApplicationUser?> GetUserByEmailAndPassword(string? email, string? password)
    {
        var query = @"
        SELECT * 
        FROM ""Users""
        WHERE ""Email"" = @Email AND ""Password"" = @Password";

        using IDbConnection connection = context.Connection;
        ApplicationUser? user = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query,
             new { Email = email, Password = password });
        return user;
    }

    public async Task<ApplicationUser?> GetUserByUserID(Guid? userID)
    {
        var query = "SELECT * FROM public.\"Users\" WHERE \"UserId\" = @UserID";
        var parameters = new { UserID = userID };

        using IDbConnection connection = context.Connection;
        return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters);
    }
}
