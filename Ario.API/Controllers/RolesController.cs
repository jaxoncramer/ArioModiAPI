/** 
 * Filename: RolesController.cs
 * Description: Contains API endpoints for managing and editing roles
 *              registered with the Ario platform
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
//using Ario.API.Repositories.Interfaces;

namespace Ario.API.Controllers
{
	[Route("api/[controller]")]
	public class RolesController : Controller
	{
		public IRolesRepository RolesRepo { get; set; }
        public IBusinessesRepository BusinessesRepo { get; set; }
        public IUsersRepository UsersRepo { get; set; }

        /// <summary>
        /// Constructor creates a link between repository source code 
        /// and these controller endpoints
        /// </summary>
        /// <param name="_repo">Roles Repository context.</param>
        public RolesController(IRolesRepository _repo, IBusinessesRepository _busRepo, IUsersRepository _userRepo)
		{
			RolesRepo = _repo;
            BusinessesRepo = _busRepo;
            UsersRepo = _userRepo;
		}

        /// <summary>
        /// Get Roles enumarations from database for both Team roles and 
        /// Business roles
        /// </summary>
        /// <returns>Two role enumerations</returns>
        /// <param name="authorization">Authorization token from Okta.</param>
		[HttpGet]
        public RolesDisplay GetAll([FromHeader] string authorization)
		{
            int status;
            Users user = UsersRepo.AuthenticateUser(authorization, out status);
            if (user == null)
            {
                this.Response.StatusCode = status;
                return null;
            }
			return RolesRepo.GetAll();
		}
	}
}

