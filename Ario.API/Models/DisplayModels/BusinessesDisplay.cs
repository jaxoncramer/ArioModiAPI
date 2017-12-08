/** 
 * Filename: BusinessesDisplay.cs
 * Description: Contains model definition for the business information returned to
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
	public class BusinessesDisplay
	{
        public BusinessesDisplay() {}

        public BusinessesDisplay(Businesses bus) {
            this.businessID = bus.businessID;
            this.industry = bus.industry;
            this.businessName = bus.businessName;
            this.address = bus.address;
            this.phoneNumber = bus.phoneNumber;
            this.website = bus.website;

            //this.owner = new Users();
        }

		public Int64 businessID { get; set; }
        public Int64 industry { get; set; }
		public string businessName { get; set; }
		public string address { get; set; }
		public string phoneNumber { get; set; }
		public string website { get; set; }

        public Users owner { get; set; }
	}
}
