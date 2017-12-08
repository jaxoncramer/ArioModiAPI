/** 
 * Filename: UserTeamJoin.cs
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

namespace Ario.API.Models
{
	public class UserTeamJoin
	{
		public Int64 id { get; set; }
		public Int64 userID { get; set; }
		public Int64 teamID { get; set; }
        public Int64 roleID { get; set; }
	}
}
