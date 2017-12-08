/** 
 * Filename: RolesDisplay.cs
 * Description: Contains model definition for the role information returned to
 *              the user as a layer of abstraction from information stored in
 *              the Ario database.
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/

using System.Collections.Generic;

namespace Ario.API.Models.DisplayModels
{
    public class RolesDisplay
    {
        public RolesDisplay() {}

        public RolesDisplay(List<TeamRoles> team, List<BusinessRoles> business) {
            this.businessRoles = business;
            this.teamRoles = team;
        }

        public List<TeamRoles> teamRoles { get; set; }
        public List<BusinessRoles> businessRoles { get; set; }
    }
}
