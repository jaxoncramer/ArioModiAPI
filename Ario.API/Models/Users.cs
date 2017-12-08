/** 
 * Filename: Users.cs
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
	public class Users
	{
        public Users() {}

        public Users(UsersDisplay disp) {
            this.userID = disp.userID;
            this.firstName = disp.firstName;
            this.lastName = disp.lastName;
            this.email = disp.email;
            this.phoneNumber = disp.phoneNumber;
            this.userPhoto = disp.userPhoto;
            this.lastModified = DateTime.Now.ToString();
            this.isArio = disp.isArio;
        }

        public Users(UsersDisplay disp, Businesses bus) {
            this.industry = bus.industry;
            this.firstName = disp.firstName;
            this.lastName = disp.lastName;
            this.email = disp.email;
            this.company = bus.businessName;
            this.phoneNumber = disp.phoneNumber;
            this.userPhoto = disp.userPhoto;
            this.lastModified = DateTime.Now.ToString();
            this.isArio = disp.isArio;
        }

        public Int64 userID { get; set; }
		public Int64 industry { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string email { get; set; }
		public string company { get; set; }
		public string phoneNumber { get; set; }
        public string lastModified { get; set; }
		public string userPhoto { get; set; }
        public Int64 isArio { get; set; }
	}
}
