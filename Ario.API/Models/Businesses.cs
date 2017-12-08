/** 
 * Filename: Businesses.cs
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
	public class Businesses
	{
        public Businesses() {}

        public Businesses(BusinessesDisplay disp) {
            this.businessID = 0;
            this.industry = disp.industry;
            this.businessName = disp.businessName;
            this.address = disp.address;
            this.phoneNumber = disp.phoneNumber;
            this.website = disp.website;
        }

		public Int64 businessID { get; set; }
        public Int64 industry { get; set; }
		public string businessName { get; set; }
		public string address { get; set; }
		public string phoneNumber { get; set; }
		public string website { get; set; }
	}
}
