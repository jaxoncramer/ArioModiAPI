/** 
 * Filename: UsersDisplay.cs
 * Description: Contains model definition for the user information returned to
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

namespace Ario.API.Models.DisplayModels
{
	public class UsersDisplay
	{
        public UsersDisplay() {}

        public UsersDisplay(Users user) {
            this.userID = user.userID;
            this.industry = user.industry;
            this.firstName = user.firstName;
            this.lastName = user.lastName;
            this.email = user.email;
            this.company = user.company;
            this.phoneNumber = user.phoneNumber;
            this.lastModified = user.lastModified;
            this.userPhoto = user.userPhoto;
            this.isArio = user.isArio;
        }

        public UsersDisplay(Users user, BusinessUserJoin joinEntry) {
            this.userID = user.userID;
            this.businessRoleID = joinEntry.roleID;
            this.businessID = joinEntry.businessID;
            this.industry = user.industry;
            this.firstName = user.firstName;
            this.lastName = user.lastName;
            this.email = user.email;
            this.company = user.company;
            this.phoneNumber = user.phoneNumber;
            this.lastModified = user.lastModified;
            this.userPhoto = user.userPhoto;
            this.isArio = user.isArio;
        }

        public UsersDisplay(Users user, BusinessUserJoin joinEntry, UserTeamJoin teamJoinEntry)
        {
            this.userID = user.userID;
            this.businessRoleID = joinEntry.roleID;
            this.teamRoleID = teamJoinEntry.roleID;
            this.businessID = joinEntry.businessID;
            this.industry = user.industry;
            this.firstName = user.firstName;
            this.lastName = user.lastName;
            this.email = user.email;
            this.company = user.company;
            this.phoneNumber = user.phoneNumber;
            this.lastModified = user.lastModified;
            this.userPhoto = user.userPhoto;
            this.isArio = user.isArio;
        }

        public Int64 userID { get; set; }
        public Int64 businessID { get; set; }
        public Int64 teamRoleID { get; set; }
        public Int64 businessRoleID { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string email { get; set; }
		public string company { get; set; }
        public Int64 industry { get; set; }
		public string phoneNumber { get; set; }
        public string lastModified { get; set; }
		public string userPhoto { get; set; }
        public Int64 isArio { get; set; }
	}
}
