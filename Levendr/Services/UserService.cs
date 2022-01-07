using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Levendr.Databases;
using Levendr.Models;
using Levendr.Helpers;
using Levendr.Constants;
using Levendr.Enums;
using Levendr.Mappings;

using Microsoft.Extensions.Configuration;

namespace Levendr.Services
{
    public class UserService : BaseService
    {

        public UserService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> Signup(SignupRequest user)
        {
            try
            {
                if (user.Username == null || user.Username.Length < 6)
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "Username should be at least 6 characters!",
                        Data = null
                    };
                }

                if (user.Username == null || user.Username.Length < 6 || !Validations.IsValidEmail(user.Email))
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "Email address is not valid!",
                        Data = null
                    };
                }

                List<Dictionary<string, object>> usernameCheckResult = await QueryDesigner
                    .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                    .WhereEquals("Username", user.Username)
                    .RunSelectQuery();

                if ((usernameCheckResult?.Count ?? 0) > 0 && (usernameCheckResult[0]?["Username"]?.ToString().Length ?? 0) > 0)
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "User already exists!",
                        Data = null
                    };
                }

                List<Dictionary<string, object>> emailCheckResult = await QueryDesigner
                    .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                    .WhereEquals("Email", user.Email)
                    .RunSelectQuery();

                if ((emailCheckResult?.Count ?? 0) > 0 && (emailCheckResult[0]?["Email"]?.ToString().Length ?? 0) > 0)
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "User already exists!",
                        Data = null
                    };
                }

                APIResult userDefaultRoleUser = await ServiceManager
                    .Instance
                    .GetService<SettingsService>()
                    .GetSetting(Constants.Settings.DefaultRoleOnSignup);

                Dictionary<string, object> userDefaultRole = (Dictionary<string, object>)(userDefaultRoleUser.Data);

                // Create User
                Dictionary<string, object> userData = new Dictionary<string, object>
                {
                    { "Username", user.Username },
                    { "Password", Hash.Create(user.Password) },
                    { "Email", user.Email ?? "" },
                    { "Fullname", user.Fullname ?? "" },
                    { "CreatedOn", DateTime.UtcNow }
                };

                List<int> ids = await QueryDesigner
                    .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                    .AddRow(userData)
                    .RunInsertQuery();

                if((ids?.Count ?? 0) < 1)
                {
                    return APIResult.GetSimpleFailureResult("An error occured while signing up!");
                }

                Dictionary<string, object> userRoleData = new Dictionary<string, object>{
                    { "User", ids[0] },
                    { "Role", Int32.Parse(userDefaultRole["Value"].ToString()) }
                };

                Columns.AppendCreatedInfo(userRoleData, ids[0]);

                await QueryDesigner
                    .CreateDesigner(schema: Schemas.Levendr, table: TableNames.UserRoles.ToString())
                    .AddRow(userRoleData)
                    .RunInsertQuery();

                return await Login(user.Username, null, user.Password);
                
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> Login(string username, string email, string password)
        {
            try
            {
                List<Dictionary<string, object>> result = await QueryDesigner
                    .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                    .WhereEquals(
                        (username?.Length ?? 0) > 0 ? "Username" : "Email",
                        (username?.Length ?? 0) > 0 ? username : email
                    )
                    .RunSelectQuery();

                if ((result?.Count ?? 0) > 0)
                {
                    if ((result[0]["Password"]?.ToString().Length ?? 0) > 0)
                    {
                        if (Hash.Validate(password, result[0]["Password"].ToString()) == true)
                        {
                            return await GetUserInfo(result[0]);
                        }
                        else
                        {
                            return new APIResult()
                            {
                                Success = false,
                                Message = "Password is invalid!",
                                Data = null
                            };
                        }
                    }
                    else
                    {
                        return new APIResult()
                        {
                            Success = false,
                            Message = "Password not set up!",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "User not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> AuthLogin(int Id)
        {
            try
            {
                List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Levendr, table: TableNames.Users.ToString())
                .WhereEquals("Id", Id)
                .RunSelectQuery();

                if ((result?.Count ?? 0) > 0)
                {
                    if ((result[0]["Password"]?.ToString().Length ?? 0) > 0)
                    {
                        return await GetUserInfo(result[0]);
                    }
                    else
                    {
                        return new APIResult()
                        {
                            Success = false,
                            Message = "Password not set up!",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "User not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> GetUserInfo(Dictionary<string, object> result)
        {
            try
            {
                result.Remove("Password");
                result.Remove("password");

                Dictionary<string, object> jwtTokenData = new Dictionary<string, object>();
                jwtTokenData.Add("Users", result);

                int Id = Int32.Parse(result["Id"]?.ToString() ?? null);

                APIResult role = await ServiceManager.Instance.GetService<UsersService>().GetUserRole(Id);
                if (role.Success == true && role.Data is not null)
                {
                    result.Add("Role", (Dictionary<string, object>)(role.Data));
                    jwtTokenData.Add("Roles", (Dictionary<string, object>)(role.Data));
                }

                APIResult permissions = await ServiceManager.Instance.GetService<UsersService>().GetUserPermissions(Id);
                if (permissions.Success == true && permissions.Data is not null)
                {
                    result.Add("Permissions", (List<Dictionary<string, object>>)(permissions.Data));
                    jwtTokenData.Add("Permissions", (List<Dictionary<string, object>>)(permissions.Data));
                }

                result.Add("Token", JWT.GenerateJSONWebToken(_configuration, jwtTokenData));

                return new APIResult()
                {
                    Success = true,
                    Message = "User logged in Successfully!",
                    Data = result
                };
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

    }
}