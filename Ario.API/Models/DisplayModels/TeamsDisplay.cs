/** 
 * Filename: TeamsDisplay.cs
 * Description: Contains model definition for the team information returned to
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

using System;
using System.Collections.Generic;

namespace Ario.API.Models.DisplayModels
{
	public class TeamsDisplay
	{
        public TeamsDisplay() {}

        public TeamsDisplay(Teams team) {
            this.teamID = team.teamID;
            this.businessID = team.businessID;
            this.teamCreatorID = team.teamCreatorID;
            this.teamName = team.teamName;

            this.users = new List<UsersDisplay>();
        }

		public Int64 teamID { get; set; }
		public Int64 businessID { get; set; }
		public Int64 teamCreatorID { get; set; }
		public string teamName { get; set; }

        public List<UsersDisplay> users { get; set; }
	}
}
