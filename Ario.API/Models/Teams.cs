/** 
 * Filename: Teams.cs
 * Description: Contains model definition for the information stored in
 *              the Ario database table with the same name as this file.
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/

using System;
using Ario.API.Models.DisplayModels;

namespace Ario.API.Models
{
	public class Teams
	{
        public Teams() {}

        public Teams(int busID, TeamsDisplay disp) {
            this.businessID = busID;
            this.teamName = disp.teamName;
        }

        public Int64 teamID { get; set; }
		public Int64 businessID { get; set; }
		public Int64 teamCreatorID { get; set; }
		public string teamName { get; set; }
	}
}
