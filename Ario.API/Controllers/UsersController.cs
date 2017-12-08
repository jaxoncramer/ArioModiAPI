/** 
 * Filename: UsersController.cs
 * Description: Contains API endpoints for managing and editing users
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
    /// <summary>
    /// This class gets user information needed to login to the Ario
    /// platform and web portal, as well as getting user information
    /// for administrator purposes
    /// <remarks>
    ///  The following endpoints are found at .../api/users
    /// </remarks>
    /// </summary>
    [Route("api/[controller]")]
	public class UsersController : Controller
	{
		public IUsersRepository UsersRepo { get; set; }
        public IBusinessesRepository BusinessesRepo { get; set; }

        /// <summary>
        /// Constructor creates a link between repository source code 
        /// and these controller endpoints
        /// </summary>
        /// <param name="_repo">User repository context.</param>
        public UsersController(IUsersRepository _repo, IBusinessesRepository _busRepo)
		{
			UsersRepo = _repo;
            BusinessesRepo = _busRepo;
		}

        /// <summary>
        /// Gets the user information embedded in the provided Okta authorization
        /// token for purposes of login.
        /// </summary>
        /// <returns>The current active user.</returns>
        /// <param name="authorization">Okta Authorization token.</param>
		[HttpGet]
        public UsersDisplay GetUser([FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
            UsersDisplay disp = UsersRepo.CreateDisplay(user, out status);
            if (disp != null)
            {
                this.Response.StatusCode = status;
                return disp;
            }
            this.Response.StatusCode = status;
            return null;
		}

        /// <summary>
        /// Get Users from the Users table in the database
        /// optionally filterable by the Users fields contained in the 
        /// html query
        /// </summary>
        /// <remarks>Only accessible by Ario employees.</remarks>
        /// <returns>Filtered list of users</returns>
        /// <param name="item">Users object from html query</param>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpGet("all")]
        public IEnumerable<UsersDisplay> GetAll([FromQuery] Users item, [FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null) {
                this.Response.StatusCode = status;
                return null;
            } else if(!BusinessesRepo.IsArio(user, out status)) {
                this.Response.StatusCode = status;
                return null;
            }
            this.Response.StatusCode = status;
			return UsersRepo.GetAll(item);
		}

        /// <summary>
        /// Gets a specific user by their identifier.
        /// </summary>
        /// <remarks>Only accessible by Ario employees.</remarks>
        /// <returns>The returned user, or null if no exist.</returns>
        /// <param name="id">User Identifier.</param>
        /// <param name="authorization">Authorization token from Okta.</param>
        [HttpGet("{id}", Name = "Users")]
        public IActionResult GetById(int id, [FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            } 

            UsersDisplay item = UsersRepo.Find(id);

            if (item == null)
			{
                this.Response.StatusCode = 400;
                return new EmptyResult();
            }
            else if (!BusinessesRepo.IsBusinessAdmin(user, (int) item.businessID, out status))
            {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;

            if (BusinessesRepo.IsArio(user, out status))
            {
                this.Response.StatusCode = status;
                return new ObjectResult(item);
            }
            else if (!BusinessesRepo.IsBusinessUser(new Users(item), (int) item.businessID, out status)) {
                this.Response.StatusCode = status;
                return new EmptyResult();
            }
            this.Response.StatusCode = status;
			return new ObjectResult(item);
		}
	}
}

