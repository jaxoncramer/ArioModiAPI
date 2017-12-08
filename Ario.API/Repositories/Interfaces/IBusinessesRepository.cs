/** 
 * Filename: IBusinessesRepository.cs
 * Description: Contains method definitions for BusinessesRepository.cs
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

using System.Collections.Generic;
using Ario.API.Models;
using Ario.API.Models.DisplayModels;

namespace Ario.API.Repositories
{
	public interface IBusinessesRepository
	{
        bool IsArio(Users user, out int statusCode);
        bool IsBusinessOwner(Users user, int busID, out int status);
        bool IsBusinessAdmin(Users user, int busID, out int statusCode);
        bool IsBusinessUser(Users user, int busID, out int status);
        void Add(BusinessesDisplay item, out int status);
        IEnumerable<BusinessesDisplay> GetSelect(BusinessesDisplay item);
        BusinessesDisplay Find(int id);
        bool BusinessExists(int id);
		bool Remove(int id, out int status);
		void Update(Businesses item, out int status);
	}
}
