/** 
 * Filename: IUsersRepository.cs
 * Description: Contains method definitions for UsersRepository.cs
 *              for validation and sanity checks. Interfaces are used in 
 *              controllers, not the repos themselves.
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/

using System.Collections.Generic;
using Ario.API.Models;
using Ario.API.Models.DisplayModels;

namespace Ario.API.Repositories
{
	public interface IUsersRepository
	{
        Users AuthenticateUser(string authorization, out int statusCode);
        bool AddByBusiness(int id, UsersDisplay item, out int status);
        bool AddUserToTeam(int busID, int teamID, int userID, out int status);
        UsersDisplay CreateDisplay(Users user, out int statusCode);
		IEnumerable<UsersDisplay> GetAll(Users item);
        IEnumerable<UsersDisplay> GetByBusiness(int id);
        IEnumerable<UsersDisplay> GetUsersOnTeam(int busID, int teamID);
        UsersDisplay Find(int id);
		bool UserExists(int userID, int busID);
        bool RemoveByBusiness(int busID, int userID, out int status);
        bool RemoveUserFromTeam(int teamID, int userID, out int status);
        void Update(int userID, int busID, UsersDisplay item, out int status);
	}
}
