/** 
 * Filename: NodesDisplay.cs
 * Description: Contains model definition for the node information returned to
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
using System.Linq;
using Ario.API.Models.Objects;
using Newtonsoft.Json.Linq;

namespace Ario.API.Models.DisplayModels
{
    public class NodesDisplay
    {
        public NodesDisplay() {}

        public NodesDisplay(Nodes node) {
            this.id = node.id;
            this.nodeName = node.nodeName;
            this.parentID = node.parentID;
            this.businessID = node.businessID;

            this.position = new Position();
            this.position.x = node.xPosition;
            this.position.y = node.yPosition;
            this.position.z = node.zPosition;

            this.components = new List<NodeComponentData>();
            this.teams = new List<Int64>();
        }

        public NodesDisplay(JObject obj) {
            if (obj.GetValue("parentid", StringComparison.InvariantCultureIgnoreCase) != null)
            {
                this.parentID = (Int64)obj.GetValue("parentid", StringComparison.InvariantCultureIgnoreCase);
            }
            if (obj.GetValue("businessid", StringComparison.InvariantCultureIgnoreCase) != null)
            {
                this.businessID = (Int64)obj.GetValue("businessid", StringComparison.InvariantCultureIgnoreCase);
            }
            if (obj.GetValue("nodename", StringComparison.InvariantCultureIgnoreCase) != null)
            {
                this.nodeName = (string)obj.GetValue("nodename", StringComparison.InvariantCultureIgnoreCase);
            }
            //TODO: Make position case insensitive
            if(obj.GetValue("position", StringComparison.InvariantCultureIgnoreCase) != null) {
                this.position = new Position();
                if (obj["position"]["x"] != null)
                {
                    this.position.x = (decimal)obj["position"]["x"];
                }
                if (obj["position"]["y"] != null)
                {
                    this.position.y = (decimal)obj["position"]["y"];
                }
                if (obj["position"]["z"] != null)
                {
                    this.position.z = (decimal)obj["position"]["z"];
                }

                //if(obj.GetValue("position.x", StringComparison.InvariantCultureIgnoreCase) != null) {
                //    this.position.x = obj.SelectToken("position.x").ToObject<Decimal>();
                //}
                //if (obj.GetValue("position.y", StringComparison.InvariantCultureIgnoreCase) != null)
                //{
                //    this.position.y = (decimal)obj.GetValue("position.y", StringComparison.InvariantCultureIgnoreCase);
                //}
                //if (obj.GetValue("position.z", StringComparison.InvariantCultureIgnoreCase) != null)
                //{
                //    this.position.z = (decimal)obj.GetValue("position.z", StringComparison.InvariantCultureIgnoreCase);
                //}
            }

            if (obj.GetValue("teams", StringComparison.InvariantCultureIgnoreCase) != null) {
                JArray json = (JArray) obj.GetValue("teams", StringComparison.InvariantCultureIgnoreCase);
                this.teams = json.Select(e => (Int64) e).ToList();
            }

            this.components = new List<NodeComponentData>();
        }

        /* Core Node Info */
        public Int64 id { get; set; }
        public Int64 parentID { get; set; }
        public Int64 businessID { get; set; }
        public string nodeName { get; set; }

        public Position position { get; set; }
        /* End Core Node Info */

        public List<Int64> teams { get; set; }

        public List<NodeComponentData> components { get; set; }
    }
}
