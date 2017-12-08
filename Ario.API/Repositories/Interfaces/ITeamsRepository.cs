/** 
 * Filename: ITeamsRepository.cs
 * Description: Contains method definitions for TeamsRepository.cs
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
	public interface ITeamsRepository
	{
        bool AddByBusiness(int busID, int userID, TeamsDisplay item, out int status);	
        IEnumerable<TeamsDisplay> GetSelect(Teams item);
        IEnumerable<TeamsDisplay> GetUserTeams(int busID, int userID);
        IEnumerable<TeamsDisplay> GetByBusiness(int id);
        TeamsDisplay Find(int id);
        bool TeamExists(int busID, int teamID);
        bool IsTeamOnBusiness(int busID, int teamID, out int status);
        bool UserIsMember(int busID, int teamID, int userID);
        bool RemoveByBusiness(int busID, int teamID, out int status);
		void Update(int teamID, int busID, Teams item, out int status);
        bool EditRole(int teamID, int userID, TeamRoles item, out int status);
	}
}
