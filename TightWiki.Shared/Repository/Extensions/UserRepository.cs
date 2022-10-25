using Dapper;
using TightWiki.Shared.ADO;
using TightWiki.Shared.Models.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TightWiki.Shared.Repository
{
    public static partial class UserRepository
	{
		public static List<Role> GetAllRoles()
		{
			using (var handler = new SqlConnectionHandler())
			{
				return handler.Connection.Query<Role>("GetAllRoles",
					null, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
			}
		}

		public static List<User> GetAllUsers(int pageNumber, int pageSize = 0)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var param = new
				{
					PageNumber = pageNumber,
					PageSize = pageSize
				};

				return handler.Connection.Query<User>("GetAllUsers",
					param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
			}
		}

		public static int CreateUser(User user)
		{
			var param = new
			{
				EmailAddress = user.EmailAddress,
				AccountName = user.AccountName,
				Navigation = user.Navigation,
				PasswordHash = user.PasswordHash,
				FirstName = user.FirstName,
				LastName = user.LastName,
				TimeZone = user.TimeZone,
				Language = user.Language,
				Country = user.Country,
				VerificationCode = user.VerificationCode
			};

			using (var handler = new SqlConnectionHandler())
			{
				return handler.Connection.ExecuteScalar<int>("CreateUser",
					param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
			}
		}

		public static void VerifyUserEmail(int userId)
		{
			var param = new
			{
				UserId = userId
			};

			using (var handler = new SqlConnectionHandler())
			{
				handler.Connection.Execute("VerifyUserEmail",
					param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
			}
		}

		public static void UpdateUserRoles(int userId, string roles)
		{
			var param = new
			{
				UserId = userId,
				Roles = roles
			};

			using (var handler = new SqlConnectionHandler())
			{
				handler.Connection.Execute("UpdateUserRoles",
					param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
			}
		}

		public static bool UpdateUserRoles(User user)
		{
			var param = new
			{
				EmailAddress = user.EmailAddress,
				AccountName = user.AccountName,
				Navigation = user.Navigation,
				PasswordHash = user.PasswordHash,
				FirstName = user.FirstName,
				LastName = user.LastName,
				TimeZone = user.TimeZone,
				Language = user.Language,
				Country = user.Country,
				VerificationCode = user.VerificationCode
			};

			using (var handler = new SqlConnectionHandler())
			{
				var result = handler.Connection.ExecuteScalar<int?>("CreateUser",
					param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
				return (result ?? 0) != 0;
			}
		}

		public static bool DoesEmailAddressExist(string emailAddress)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var result = handler.Connection.ExecuteScalar<int?>("DoesEmailAddressExist",
					new { EmailAddress = emailAddress }, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
				return (result ?? 0) != 0;
			}
		}

		public static bool DoesAccountNameExist(string accountName)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var result = handler.Connection.ExecuteScalar<int?>("DoesAccountNameExist",
					new { accountName = accountName }, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
				return (result ?? 0) != 0;
			}
		}

		public static List<Role> GetUserRolesByUserId(int userID)
		{
			using (var handler = new SqlConnectionHandler())
			{
				return handler.Connection.Query<Role>("GetUserRolesByUserId",
					new { UserID = userID }, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).ToList();
			}
		}

		public static User GetUserById(int id)
		{
			using (var handler = new SqlConnectionHandler())
			{
				return handler.Connection.Query<User>("GetUserById",
					new { Id = id }, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
			}
		}


		public static User GetUserByNavigationAndVerificationCode(string navigation, string verificationCode)
		{
			var param = new
			{
				Navigation = navigation,
				VerificationCode = verificationCode,
			};

			using (var handler = new SqlConnectionHandler())
			{
				return handler.Connection.Query<User>("GetUserByNavigationAndVerificationCode",
					param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
			}
		}

		public static User GetUserByNavigation(string navigation)
		{
			using (var handler = new SqlConnectionHandler())
			{
				return handler.Connection.Query<User>("GetUserByNavigation",
					new { Navigation = navigation }, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
			}
		}

		public static User GetUserByEmail(string emailAddress)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var param = new
				{
					EmailAddress = emailAddress
				};

				return handler.Connection.Query<User>("GetUserByEmail",
					param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
			}
		}

		public static User GetUserByEmailAndPasswordHash(string emailAddress, string passwordHash)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var param = new
				{
					EmailAddress = emailAddress,
					PasswordHash = passwordHash
				};

				return handler.Connection.Query<User>("GetUserByEmailAndPasswordHash",
					param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
			}
		}

		public static User GetUserByEmailAndPassword(string emailAddress, string password)
		{
			string passwordHash = Library.Security.Sha256(password);
			using (var handler = new SqlConnectionHandler())
			{
				var param = new
				{
					EmailAddress = emailAddress,
					PasswordHash = passwordHash
				};

				return handler.Connection.Query<User>("GetUserByEmailAndPasswordHash",
					param, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
			}
		}

		public static void UpdateUserLastLoginDateByUserId(int userId)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var param = new
				{
					UserId = userId
				};

				handler.Connection.Execute("UpdateUserLastLoginDateByUserId",
					param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
			}
		}

		public static void UpdateUserAvatar(int userId, byte[] imageData)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var param = new
				{
					UserId = userId,
					Avatar = imageData,
				};

				handler.Connection.Execute("UpdateUserAvatar",
					param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
			}
		}

		public static byte[] GetUserAvatarBynavigation(string navigation)
		{
			using (var handler = new SqlConnectionHandler())
			{
				return handler.Connection.Query<byte[]>("GetUserAvatarBynavigation",
					new
					{
						Navigation = navigation
					}, null, true, Singletons.CommandTimeout, CommandType.StoredProcedure).FirstOrDefault();
			}
		}

		public static void UpdateUserPassword(int userId, string password)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var param = new
				{
					UserId = userId,
					PasswordHash = Library.Security.Sha256(password)
				};

				handler.Connection.Execute("UpdateUserPassword",
					param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
			}
		}

		public static void UpdateUserVerificationCode(int userId, string VerificationCode)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var param = new
				{
					UserId = userId,
					VerificationCode = VerificationCode
				};

				handler.Connection.Execute("UpdateUserVerificationCode",
					param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
			}
		}

		public static void UpdateUser(User item)
		{
			using (var handler = new SqlConnectionHandler())
			{
				var param = new
				{
					Id = item.Id,
					EmailAddress = item.EmailAddress,
					AccountName = item.AccountName,
					Navigation = item.Navigation,
					PasswordHash = item.PasswordHash,
					FirstName = item.FirstName,
					LastName = item.LastName,
					TimeZone = item.TimeZone,
					Language = item.Language,
					Country = item.Country,
					AboutMe = item.AboutMe,
					ModifiedDate = item.ModifiedDate
				};

				handler.Connection.Execute("UpdateUser",
					param, null, Singletons.CommandTimeout, CommandType.StoredProcedure);
			}
		}
	}
}
