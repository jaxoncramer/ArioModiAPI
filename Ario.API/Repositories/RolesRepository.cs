/** 
 * Filename: RolesRepository.cs
 * Description: Contains the source logic for API endpoint(s) for retrieving 
 *              role enumeration info for team and business roles
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/

using System.Collections.Generic;
using System.Linq;
using Ario.API.Models;
using Ario.API.Contexts;
using Ario.API.Models.DisplayModels;

namespace Ario.API.Repositories
{
    /// <summary>
    /// This class Contains the source logic for API endpoint(s) for retrieving 
    /// role enumeration info for team and business roles
    /// <seealso cref="Controllers.RolesController"/>
    /// </summary>
    public class RolesRepository : IRolesRepository
	{
        TeamRolesContext _teamContext;
        BusinessRolesContext _businessContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Ario.API.Repositories.RolesRepository"/> class.
        /// </summary>
        /// <param name="teamContext">Team roles table context from database.</param>
        /// <param name="businessContext">Business roles table context from database.</param>
        public RolesRepository(TeamRolesContext teamContext, BusinessRolesContext businessContext)
		{
            _teamContext = teamContext;
            _businessContext = businessContext;
		}

        /// <summary>
        /// Gets both role enumerations from Ario database
        /// </summary>
        /// <returns>Formatted role enumerations object.</returns>
        public RolesDisplay GetAll()
		{
            List<TeamRoles> team = _teamContext.TeamRoles.ToList();
            List<BusinessRoles> business = _businessContext.BusinessRoles.ToList();

            return new RolesDisplay(team, business);
		}
	}
}

