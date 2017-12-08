/** 
 * Filename: IRolesRepository.cs
 * Description: Contains method definitions for RolesRepository.cs
 *              for validation and sanity checks. Interfaces are used in 
 *              controllers, not the repos themselves.
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/

using Ario.API.Models.DisplayModels;

namespace Ario.API.Repositories
{
    public interface IRolesRepository
	{
        RolesDisplay GetAll();
	}
}
