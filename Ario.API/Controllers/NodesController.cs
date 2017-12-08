/** 
 * Filename: NodesController.cs
 * Description: Contains API endpoints for manipulating the placement and 
 *              management of nodes contained in the Ario database.
 *              
 * Info: Nodes will appear in world-space if they have a label component 
 *          associated with the node container
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/


using Ario.API.Models;
using Ario.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Ario.API.Models.DisplayModels;
using Ario.API.Models.ModelBinders;
using Ario.API.Models.Objects;

namespace Ario.API.Controllers
{
    /// <summary>
    /// This class controls the placement and management of nodes for the
    /// Ario platform application.
    /// <remarks>
    ///  The following endpoints are found at .../api/nodes
    /// </remarks>
    /// </summary>
    [Route("api/[controller]")]
	public class NodesController : Controller
	{
		public INodesRepository NodesRepo { get; set; }
        public IBusinessesRepository BusinessesRepo { get; set; }
        public IUsersRepository UsersRepo { get; set; }

        /// <summary>
        /// Constructor creates a link between repository source code 
        /// and these controller endpoints
        /// </summary>
        /// <param name="_repo">Nodes repository context.</param>
        /// <param name="_busRepo">Businesses repository context.</param>
        /// <param name="_userRepo">Users repository context.</param>
        public NodesController(INodesRepository _repo, IBusinessesRepository _busRepo, IUsersRepository _userRepo)
		{
			NodesRepo = _repo;
            BusinessesRepo = _busRepo;
            UsersRepo = _userRepo;
		}

        /// <summary>
        /// Get Nodes from the Nodes table in the database
        /// optionally filterable by the Nodes fields contained in the 
        /// html query
        /// </summary>
        /// <remarks>Only accessible by Ario employees.</remarks>
        /// <returns>Filtered list of nodes</returns>
        /// <param name="item">Nodes object from html query</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpGet]
        public IEnumerable<NodesDisplay> GetSelect([FromQuery] NodesDisplay item, [FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
            else if (!BusinessesRepo.IsArio(user, out status))
            {
                this.Response.StatusCode = status;
                return null;
            }
            this.Response.StatusCode = status;
            return NodesRepo.GetSelect(item);
		}

        /// <summary>
        /// Gets a specific node by its identifier.
        /// </summary>
        /// <returns>The returned node, or null if no exist.</returns>
        /// <param name="id">Node Identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpGet("{id}", Name = "Nodes")]
        public IActionResult GetById(int id, [FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }

            NodesDisplay disp = NodesRepo.Find(id);

            if (disp == null)
			{
                this.Response.StatusCode = 400;
                return new EmptyResult();
			}
            else if (!BusinessesRepo.IsBusinessUser(user, (int) disp.businessID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = 200;
			return new ObjectResult(disp);
		}

        /// <summary>
        /// Gets the nodes children, if the node is assigned to an appropriate business.
        /// </summary>
        /// <returns>List of node childern, or null permissions denied or node does not exist.</returns>
        /// <param name="nodeID">Node identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpGet("{nodeID}/children")]
        public IEnumerable<NodesDisplay> GetNodeChildren(int nodeID, [FromHeader] string authorization)
        {
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
            NodesDisplay disp = NodesRepo.Find(nodeID);
            if (disp == null) {
                this.Response.StatusCode = 400;
                return null;
            }
            else if (!BusinessesRepo.IsBusinessUser(user, (int) disp.businessID, out status))
            {
                this.Response.StatusCode = status;
                return null;
            }
            var nodeList = NodesRepo.GetChildren((int) disp.businessID, nodeID, out status);
            this.Response.StatusCode = status;
            if (status == 200)
            {
                return nodeList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create a new Node.
        /// </summary>
        /// <returns>Returns the node entered in the html body, or null if
        /// malformed request</returns>
        /// <param name="item">New Node object from body.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpPost]
        public IActionResult Create([ModelBinder (BinderType = typeof(NodesDisplayModelBinder))][FromBody] NodesDisplay item, [FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }

			if (item == null)
			{
                this.Response.StatusCode = 400;
                return new EmptyResult();
			}
            else if (!BusinessesRepo.IsBusinessUser(user, (int) item.businessID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
			NodesRepo.Add(item, out status);
            if (status != 201) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
			return CreatedAtRoute("Nodes", new { Controller = "Nodes", id = item.id }, item);
		}

        /// <summary>
        /// Update node information.
        /// </summary>
        /// <remarks>Non-null (or 0 for ints) fields are not updated</remarks>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="id">Node Identifier.</param>
        /// <param name="item">Node fields to update from html body.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpPut("{id}")]
        public IActionResult Update(int id, [ModelBinder(BinderType = typeof(NodesDisplayModelBinder))][FromBody] NodesDisplay item, [FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }

			if (item == null)
			{
                this.Response.StatusCode = 400;
                return new EmptyResult();
			}
            else if (!NodesRepo.NodeExists(id))
			{
                this.Response.StatusCode = 400;
                return new EmptyResult();
			}
            else if (!BusinessesRepo.IsBusinessUser(user, (int)item.businessID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
			NodesRepo.Update(id, item, out status);
            this.Response.StatusCode = status;
            if (status != 200) {
                return new EmptyResult();
            }
            return new OkResult();
		}

        /// <summary>
        /// Adds the specified component to a particular existing node. Component type must be manually defined.
        /// </summary>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="id">Node Identifier.</param>
        /// <param name="item">Component to add from html body.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpPost("{id}/components")]
        public IActionResult AddComponent(int id, [ModelBinder (BinderType = typeof(NodeComponentDataModelBinder))][FromBody] NodeComponentData item, [FromHeader] string authorization) 
        {
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            if(item == null)
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }

            NodesDisplay disp = NodesRepo.Find(id);

            if (disp == null) {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessUser(user, (int)disp.businessID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }

            NodesRepo.AddComponent(id, item, out status);
            this.Response.StatusCode = status;
            if (status != 201) {
                return new EmptyResult();
            }
            return CreatedAtRoute("Nodes", new { Controller = "Nodes", id = id }, item);
        }

        /// <summary>
        /// Deletes the specified component from a particular existing node. Component type does not need to be defined.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="nodeID">Node identifier.</param>
        /// <param name="compID">Component to drop from html body.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpDelete("{nodeID}/components/{compID}")]
        public IActionResult DropComponent(int nodeID, int compID, [FromHeader] string authorization) {
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }

            NodesDisplay disp = NodesRepo.Find(nodeID);
            if (disp == null)
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessUser(user, (int)disp.businessID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }

            if(!NodesRepo.DropComponent(nodeID, compID, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return new OkResult();
        }

        /// <summary>
        /// Delete the specified node and all associated components.
        /// </summary>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="id">Node Identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }

            NodesDisplay disp = NodesRepo.Find(id);
            if (disp == null)
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessUser(user, (int)disp.businessID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }

            if(!NodesRepo.Remove(id, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return new OkResult();
		}
	}
}

