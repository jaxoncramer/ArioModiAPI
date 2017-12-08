/** 
 * Filename: BusinessesController.cs
 * Description: Contains API endpoints for managing and editing businesses (clients)
 *              registered with the Ario platform, as well as the teams and users
 *              associated with existing businesses.
 *              
 * Info: Certain endpoints may only be executed by Ario employees, and others 
 *          may only be used by administrators of the appropriate business from
 *          http://portal.ario.com
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
using Microsoft.AspNetCore.Authorization;

namespace Ario.API.Controllers
{
    /// <summary>
    /// This class controls the business and user management needed for the 
    /// admin web portal of the Ario platform.
    /// <remarks>
    ///  The following endpoints are found at .../api/businesses
    /// </remarks>
    /// </summary>
	[Route("api/[controller]")]
	public class BusinessesController : Controller
	{
		public IBusinessesRepository BusinessesRepo { get; set; }
        public IUsersRepository UsersRepo { get; set; }
        public ITeamsRepository TeamsRepo { get; set; }
        public INodesRepository NodesRepo { get; set; }

        /// <summary>
        /// Constructor creates a link between repository source code 
        /// and these controller endpoints
        /// </summary>
        /// <param name="_repo">Businesses repository context</param>
        /// <param name="_userRepo">Users repository context</param>
        /// <param name="_teamsRepo">Teams repository context</param>
        /// <param name="_nodesRepo">Nodes repository context</param>
        public BusinessesController(IBusinessesRepository _repo, IUsersRepository _userRepo, ITeamsRepository _teamsRepo, INodesRepository _nodesRepo)
		{
			BusinessesRepo = _repo;
            UsersRepo = _userRepo;
            TeamsRepo = _teamsRepo;
            NodesRepo = _nodesRepo;
		}

        /// <summary>
        /// Get Businesses from the Businesses table in the database
        /// optionally filterable by the Businesses fields contained in the 
        /// html query
        /// </summary>
        /// <remarks>Only accessible by Ario employees.</remarks>
        /// <returns>Filtered list of businesses</returns>
        /// <param name="item">Businesses object from html query</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpGet]
        public IEnumerable<BusinessesDisplay> GetSelect([FromQuery] BusinessesDisplay item, [FromHeader] string authorization)
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
			return BusinessesRepo.GetSelect(item);
		}

        /// <summary>
        /// Gets a specific business by its identifier.
        /// </summary>
        /// <returns>The returned business, or null if no exist.</returns>
        /// <param name="id">Business Identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpGet("{id}", Name = "Businesses")]
        public IActionResult GetById(int id, [FromHeader] string authorization)
		{
            int status;
            if (!BusinessesRepo.BusinessExists(id))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, id, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
			var item = BusinessesRepo.Find(id);
			if (item == null)
			{
                this.Response.StatusCode = 400;
                return new EmptyResult();
			}
			return new ObjectResult(item);
		}

        /// <summary>
        /// Gets all nodes assigned to a specific business.
        /// </summary>
        /// <returns>A list of filtered nodes.</returns>
        /// <param name="id">Nodes Identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpGet("{id}/nodes")]
        public IEnumerable<NodesDisplay> GetNodes(int id, [FromQuery] NodesDisplay item, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(id))
            {
                this.Response.StatusCode = 400;
                return null;
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
            else if(!BusinessesRepo.IsBusinessAdmin(user, id, out status)) {
                this.Response.StatusCode = status;
                return null;
            }
            return NodesRepo.GetByBusiness(id, item);
        }

        /// <summary>
        /// Gets all of the users in the business.
        /// </summary>
        /// <returns>List of users.</returns>
        /// <param name="id">Business Identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpGet("{id}/users")]
        public IEnumerable<UsersDisplay> GetUsers(int id, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(id))
            {
                this.Response.StatusCode = 400;
                return null;
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, id, out status))
            {
                this.Response.StatusCode = status;
                return null;
            }
            this.Response.StatusCode = status;
            return UsersRepo.GetByBusiness(id);
        }

        [HttpGet("{id}/teams")]
        public IEnumerable<TeamsDisplay> GetUserTeams(int id, [FromHeader] string authorization)
        {
            int status;
            if (!BusinessesRepo.BusinessExists(id))
            {
                this.Response.StatusCode = 400;
                return null;
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
            else if (!BusinessesRepo.IsBusinessUser(user, id, out status))
            {
                this.Response.StatusCode = status;
                return null;
            }
            this.Response.StatusCode = status;
            return TeamsRepo.GetUserTeams(id, (int) user.userID);
        }

        /// <summary>
        /// Gets all the teams in the business
        /// </summary>
        /// <returns>List of teams.</returns>
        /// <param name="id">Business Identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpGet("{id}/teams/all")]
        public IEnumerable<TeamsDisplay> GetTeams(int id, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(id))
            {
                this.Response.StatusCode = 400;
                return null;
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, id, out status))
            {
                this.Response.StatusCode = status;
                return null;
            }
            this.Response.StatusCode = status;
            return TeamsRepo.GetByBusiness(id);
        }

        /// <summary>
        /// Gets a specific team by its identifier in a business.
        /// </summary>
        /// <returns>The team, or null.</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpGet("{busID}/teams/{teamID}")]
        public IActionResult GetTeamById(int busID, int teamID, [FromHeader] string authorization)
        {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!TeamsRepo.IsTeamOnBusiness(busID, teamID, out status)) 
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            var item = TeamsRepo.Find(teamID);
            if (item == null)
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            return new ObjectResult(item);
        }

        /// <summary>
        /// Gets the users on a team in a business.
        /// </summary>
        /// <returns>List of users on team.</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpGet("{busID}/teams/{teamID}/users")]
        public IEnumerable<UsersDisplay> GetUsersOnTeam(int busID, int teamID, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return null;
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return null;
            }
            else if (!TeamsRepo.IsTeamOnBusiness(busID, teamID, out status))
            {
                this.Response.StatusCode = status;
                return null;
            }
            this.Response.StatusCode = status;
            return UsersRepo.GetUsersOnTeam(busID, teamID);
        }

        /// <summary>
        /// Gets the nodes visible by a particular team.
        /// </summary>
        /// <returns>List of users on team.</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpGet("{busID}/teams/{teamID}/nodes")]
        public IEnumerable<NodesDisplay> GetNodesOnTeam(int busID, int teamID, [FromHeader] string authorization)
        {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return null;
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
            else if (!BusinessesRepo.IsBusinessUser(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return null;
            } 
            else if (!TeamsRepo.IsTeamOnBusiness(busID, teamID, out status))
            {
                this.Response.StatusCode = status;
                return null;
            }
            this.Response.StatusCode = status;
            return NodesRepo.GetNodesOnTeam(teamID);
        }

        /// <summary>
        /// Create a new Business.
        /// </summary>
        /// <returns>Returns the business entered in the html body, or null if
        /// malformed request</returns>
        /// <param name="item">New Business object from body.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpPost]
        public IActionResult Create([FromBody] BusinessesDisplay item, [FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsArio(user, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
			if (item == null)
			{
                this.Response.StatusCode = 400;
				return new EmptyResult();
			}
			BusinessesRepo.Add(item, out status);
            this.Response.StatusCode = status;
            if (status != 201) {
                return new EmptyResult();
            }
            return CreatedAtRoute("Businesses", new { Controller = "Businesses", id = item.businessID }, item);
		}

        /// <summary>
        /// Adds a user to a business.
        /// </summary>
        /// <returns>Returns the user entered in the html body, or null if
        /// malformed request</returns>
        /// <param name="id">Business Identifier.</param>
        /// <param name="item">New User object.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpPost("{id}/users")]
        public IActionResult AddUser(int id, [FromBody] UsersDisplay item, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(id))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, id, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            if (item == null) {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            if(!UsersRepo.AddByBusiness(id, item, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return CreatedAtRoute("Businesses", new { Controller = "Businesses", id = item.userID }, item);
        }

        /// <summary>
        /// Adds a team to a business.
        /// </summary>
        /// <returns>Returns the team entered in the html body, or null if
        /// malformed request</returns>
        /// <param name="id">Business Identifier.</param>
        /// <param name="item">New Team object.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpPost("{id}/teams")]
        public IActionResult AddTeam(int id, [FromBody] TeamsDisplay item, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(id))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, id, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            if(item == null) {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            if(!TeamsRepo.AddByBusiness(id, (int) user.userID, item, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return CreatedAtRoute("Businesses", new { Controller = "Businesses", id = item.teamID }, item);
        }

        /// <summary>
        /// Adds a user a team contained in a business.
        /// </summary>
        /// <returns>Returns the user entered in the html body, or null if
        /// malformed request</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="userID">User identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpPost("{busID}/teams/{teamID}/users/{userID}")]
        public IActionResult AddUserToTeam(int busID, int teamID, int userID, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!TeamsRepo.IsTeamOnBusiness(busID, teamID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            if(!UsersRepo.AddUserToTeam(busID, teamID, userID, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return new OkResult();
        }

        /// <summary>
        /// Update business information.
        /// </summary>
        /// <remarks>Non-null (or 0 for ints) fields are not updated</remarks>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="id">Business Identifier.</param>
        /// <param name="item">Business fields to update from html body.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Businesses item, [FromHeader] string authorization)
		{
            int status;
            if (!BusinessesRepo.BusinessExists(id))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            item.businessID = id;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, id, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
			if (item == null)
			{
                this.Response.StatusCode = 400;
                return new EmptyResult();
			}
			BusinessesRepo.Update(item, out status);
            if (status != 200) {
                return new EmptyResult();
            }
            return new OkResult();
		}

        /// <summary>
        /// Edits user information contained in a business.
        /// </summary>
        /// <remarks>Non-null (or 0 for ints) fields are not updated</remarks>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="userID">User identifier.</param>
        /// <param name="item">User fields to update from html body.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpPut("{busID}/users/{userID}")]
        public IActionResult EditUser(int busID, int userID, [FromBody] UsersDisplay item, [FromHeader] string authorization) 
        {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            if (item == null)
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            if (!UsersRepo.UserExists(userID, busID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            UsersRepo.Update(userID, busID, item, out status);
            if (status != 200) {
                return new EmptyResult();
            }
            return new OkResult();
        }

        /// <summary>
        /// Edits team information contained in a business.
        /// </summary>
        /// <remarks>Non-null (or 0 for ints) fields are not updated</remarks>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="item">Team name to update from html body.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpPut("{busID}/teams/{teamID}")]
        public IActionResult EditTeam(int busID, int teamID, [FromBody] Teams item, [FromHeader] string authorization) 
        {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            if (item == null)
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            if (!TeamsRepo.TeamExists(busID, teamID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!TeamsRepo.IsTeamOnBusiness(busID, teamID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            TeamsRepo.Update(teamID, busID, item, out status);
            this.Response.StatusCode = status;
            if (status == 200) {
                return new OkResult();
            }
            else {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// Edits the role of the user in a specific team of a business.
        /// </summary>
        /// <remarks>Non-null (or 0 for ints) fields are not updated</remarks>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="userID">User identifier.</param>
        /// <param name="item">User Role to update from html body.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpPut("{busID}/teams/{teamID}/users/{userID}")]
        public IActionResult EditTeamRole(int busID, int teamID, int userID, [FromBody] TeamRoles item, [FromHeader] string authorization) 
        {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            if (item == null)
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            if (!TeamsRepo.UserIsMember(busID, teamID, userID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessOwner(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!TeamsRepo.IsTeamOnBusiness(busID, teamID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            if(!TeamsRepo.EditRole(teamID, userID, item, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return new OkResult();
        }

        /// <summary>
        /// Delete the specified business.
        /// </summary>
        /// <remarks>This endpoint is only accessible by Ario employees.</remarks>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="id">Business Identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpDelete("{id}")]
        public IActionResult Delete(int id, [FromHeader] string authorization)
		{
            int status;
            if (!BusinessesRepo.BusinessExists(id))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsArio(user, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            if(!BusinessesRepo.Remove(id, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return new OkResult();
		}

        /// <summary>
        /// Deletes the user from a specified business.
        /// </summary>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="userID">User identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpDelete("{busID}/users/{userID}")]
        public IActionResult RemoveUser(int busID, int userID, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            if(!UsersRepo.RemoveByBusiness(busID, userID, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return new OkResult();
        }

        /// <summary>
        /// Removes the team from a specified business.
        /// </summary>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpDelete("{busID}/teams/{teamID}")]
        public IActionResult RemoveTeam(int busID, int teamID, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!TeamsRepo.IsTeamOnBusiness(busID, teamID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            if(!TeamsRepo.RemoveByBusiness(busID, teamID, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return new OkResult();
        }

        /// <summary>
        /// Removes the user from a team in a business.
        /// </summary>
        /// <returns>An OK (200) result, or null if malformed request</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="userID">User identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpDelete("{busID}/teams/{teamID}/users/{userID}")]
        public IActionResult RemoveUserFromTeam(int busID, int teamID, int userID, [FromHeader] string authorization) {
            int status;
            if (!BusinessesRepo.BusinessExists(busID))
            {
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, busID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            else if (!TeamsRepo.IsTeamOnBusiness(busID, teamID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            if (!UsersRepo.RemoveUserFromTeam(teamID, userID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
            return new OkResult();
        }
	}
}

