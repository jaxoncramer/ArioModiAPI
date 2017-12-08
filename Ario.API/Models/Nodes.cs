/** 
 * Filename: Nodes.cs
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
	public class Nodes
	{
        public Int64 id { get; set; }
        public Int64 parentID { get; set; }
        public Int64 businessID { get; set; }
        public string nodeName { get; set; }
        public decimal xPosition { get; set; }
        public decimal yPosition { get; set; }
        public decimal zPosition { get; set; }

        public Nodes() {}

        public Nodes(NodesDisplay disp) {
            this.id = disp.id;
            this.parentID = disp.parentID;
            this.businessID = disp.businessID;
            this.nodeName = disp.nodeName;
            if (disp.position != null)
            {
                this.xPosition = disp.position.x;
                this.yPosition = disp.position.y;
                this.zPosition = disp.position.z;
            }
        }

	}
}
