/** 
 * Filename: NodeComponents.cs
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
using Ario.API.Models.Objects;

namespace Ario.API.Models
{
	public class NodeComponents
	{
        public NodeComponents() {}

        public NodeComponents(Nodes node, string type) {
            this.id = 0;
            this.nodeID = node.id;
            this.type = type;
        }

        public NodeComponents(NodeComponentData data) {
            this.id = 0;
            this.nodeID = data.nodeID;
            this.type = data.type;
        }

		public Int64 id { get; set; }
        public Int64 nodeID { get; set; }
		public string type { get; set; }
	}
}
