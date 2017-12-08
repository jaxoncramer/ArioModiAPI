/** 
 * Filename: TeamsController.cs
 * Description: Contains API endpoints for managing and editing teams
 *              registered with the Ario platform
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

namespace Ario.API.Controllers
{
	[Route("api/[controller]")]
	public class TeamsController : Controller
	{
		public ITeamsRepository TeamsRepo { get; set; }
        public IBusinessesRepository BusinessesRepo { get; set; }
        public IUsersRepository UsersRepo { get; set; }

        /// <summary>
        /// Constructor creates a link between repository source code 
        /// and these controller endpoints
        /// </summary>
        /// <param name="_repo">Teams Repository context.</param>
        /// <param name="_busRepo">Businesses Repository context.</param>
        /// <param name="_userRepo">Users Repository context.</param>
        public TeamsController(ITeamsRepository _repo, IBusinessesRepository _busRepo, IUsersRepository _userRepo)
		{
			TeamsRepo = _repo;
            BusinessesRepo = _busRepo;
            UsersRepo = _userRepo;
		}

        /// <summary>
        /// Get Teams from the Teams table in the database
        /// optionally filterable by the Teams fields contained in the 
        /// html query
        /// </summary>
        /// <remarks>Only accessible by Ario employees.</remarks>
        /// <returns>Filtered list of teams</returns>
        /// <param name="item">Teams object from html query</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpGet("all")]
        public IEnumerable<TeamsDisplay> GetAll([FromQuery] Teams item, [FromHeader] string authorization)
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
			return TeamsRepo.GetSelect(item);
		}

        /// <summary>
        /// Gets a specific team by its identifier.
        /// </summary>
        /// <remarks>Only accessible by Ario employees.</remarks>
        /// <returns>The returned team, or null if no exist.</returns>
        /// <param name="id">Team Identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpGet("{id}", Name = "Teams")]
        public IActionResult GetById(int id, [FromHeader] string authorization)
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
			var item = TeamsRepo.Find(id);
			if (item == null)
			{
                this.Response.StatusCode = 400;
                return new EmptyResult();
			}
			return new ObjectResult(item);
		}
	}
}

