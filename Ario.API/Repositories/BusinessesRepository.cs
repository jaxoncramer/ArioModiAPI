/** 
 * Filename: BusinessesRepository.cs
 * Description: Contains the source logic for API endpoints for managing and 
 *              editing businesses (clients) registered with the Ario platform.
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/

using System.Collections.Generic;
using System.Linq;
using Ario.API.Models;
using Ario.API.Contexts;
using Ario.API.Models.DisplayModels;
using System;
using System.Reflection;
using Ario.API.Attributes;

namespace Ario.API.Repositories
{
    /// <summary>
    /// This class contains the source logic for API endpoints for managing and 
    /// editing businesses(clients) registered with the Ario platform.
    /// <seealso cref="Controllers.BusinessesController"/>
    /// <seealso cref="Controllers.NodesController"/>
    /// <seealso cref="Controllers.UsersController"/>
    /// <seealso cref="Controllers.TeamsController"/>
    /// <seealso cref="Controllers.RolesController"/>
    /// </summary>
	public class BusinessesRepository : IBusinessesRepository
	{
		BusinessesContext _businessContext;
        BusinessUserJoinContext _businessUserJoinContext;
        UsersContext _userContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Ario.API.Repositories.BusinessesRepository"/> class.
        /// </summary>
        /// <param name="businessContext">Business table context from database.</param>
        /// <param name="businessUserJoinContext">Business user join table context from database.</param>
        /// <param name="userContext">User table context from database.</param>
        public BusinessesRepository(BusinessesContext businessContext, 
                                    BusinessUserJoinContext businessUserJoinContext, 
                                    UsersContext userContext)
		{
            _businessContext = businessContext;
            _businessUserJoinContext = businessUserJoinContext;
            _userContext = userContext;
		}

        /// <summary>
        /// Checks if user is an Ario employee or not.
        /// </summary>
        /// <returns><c>true</c>, if user is ario employee, <c>false</c> otherwise.</returns>
        /// <param name="user">User object contained in user database table.</param>
        /// <param name="status">Status code pointer.</param>
        public bool IsArio (Users user, out int status) {
            if(user.isArio == 1) {
                status = 200;
                return true;
            } else {
                status = 403;
                return false;
            }
        }

        //public bool IsArio (Users user, out int status) {
        //    BusinessUserJoin userJoin = _businessUserJoinContext.BusinessUserJoin.FirstOrDefault(j => j.userID == user.userID);
        //    if (userJoin != null)
        //    {
        //        if (userJoin.roleID == 1)
        //        {
        //            status = 200;
        //            return true;
        //        }
        //        status = 403;
        //        return false;
        //    }
        //    status = 404;
        //    return false;
        //}

        /// <summary>
        /// Checks if user the owner (i.e. superadmin) of the business
        /// </summary>
        /// <returns><c>true</c>, if user is business admin, <c>false</c> otherwise.</returns>
        /// <param name="user">User object contained in user database table.</param>
        /// <param name="busID">Business identifier.</param>
        /// <param name="status">Status code pointer.</param>
        public bool IsBusinessOwner(Users user, int busID, out int status)
        {
            BusinessUserJoin userJoin = _businessUserJoinContext.BusinessUserJoin.SingleOrDefault(j => j.userID == user.userID && j.businessID == busID);
            if (userJoin != null)
            {
                if (userJoin.roleID == 2)
                {
                    status = 200;
                    return true;
                }
            }
            if (user.isArio == 1)
            {
                status = 200;
                return true;
            }
            else
            {
                status = 403;
                return false;
            }
        }

        /// <summary>
        /// Checks if user is a business admin/Ario employee or not.
        /// </summary>
        /// <returns><c>true</c>, if user is business admin, <c>false</c> otherwise.</returns>
        /// <param name="user">User object contained in user database table.</param>
        /// <param name="busID">Business identifier.</param>
        /// <param name="status">Status code pointer.</param>
        public bool IsBusinessAdmin(Users user, int busID, out int status) {
            BusinessUserJoin userJoin = _businessUserJoinContext.BusinessUserJoin.SingleOrDefault(j => j.userID == user.userID && j.businessID == busID);
            if(userJoin != null) {
                if(userJoin.roleID <= 3 && userJoin.roleID > 1) {
                    status = 200;
                    return true;
                }
            }
            if (user.isArio == 1)
            {
                status = 200;
                return true;
            } else {
                status = 403;
                return false;
            }
        }

        /// <summary>
        /// Ises the business user.
        /// </summary>
        /// <returns><c>true</c>, if user is a member of the specified business, <c>false</c> otherwise.</returns>
        /// <param name="user">User object representing the logged in user.</param>
        /// <param name="busID">Business identifier.</param>
        /// <param name="status">Status code pointer.</param>
        public bool IsBusinessUser(Users user, int busID, out int status) {
            BusinessUserJoin userJoin = _businessUserJoinContext.BusinessUserJoin.SingleOrDefault(j => j.userID == user.userID && j.businessID == busID);
            if (userJoin != null)
            {
                status = 200;
                return true;
            }
            if (user.isArio == 1)
            {
                status = 200;
                return true;
            }
            else
            {
                status = 403;
                return false;
            }
        }

        /// <summary>
        /// Find and return the specified business, if exists.
        /// </summary>
        /// <returns>The business object found, or null.</returns>
        /// <param name="id">Business identifier.</param>
        public BusinessesDisplay Find (int id)
		{
            Businesses bus = _businessContext.Businesses.Where(e => e.businessID == id).SingleOrDefault();
            if(bus != null) {
                return CreateDisplay(bus);
            }
            return null;
		}

        /// <summary>
        /// Checks if the business exists in the Ario database
        /// </summary>
        /// <returns><c>true</c>, if business exists, <c>false</c> otherwise.</returns>
        /// <param name="id">Business Identifier.</param>
        public bool BusinessExists (int id) {
            Businesses bus = _businessContext.Businesses.Where(e => e.businessID == id).SingleOrDefault();
            if (bus != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a list of businesses filtered by business object parameter
        /// </summary>
        /// <returns>A list of filtered businesses.</returns>
        /// <param name="item">Business object contained in user database table with desired fields.</param>
        public IEnumerable<BusinessesDisplay> GetSelect(BusinessesDisplay item)
		{

            List<BusinessesDisplay> displayList = new List<BusinessesDisplay>();

            if (item != null)
            {
                List<Businesses> nodeList = _businessContext.Businesses.ToList();

                foreach (Businesses entry in nodeList)
                {
                    bool match = true;
                    BusinessesDisplay disp = CreateDisplay(entry);

                    Type type = disp.GetType();
                    foreach (PropertyInfo propertyInfo in type.GetProperties())
                    {
                        if (propertyInfo.CanRead)
                        {
                            if (propertyInfo.GetValue(item) != null && !propertyInfo.GetValue(item).ToString().Equals("0"))
                            {
                                if (Attribute.IsDefined(propertyInfo, typeof(BlackListed)))
                                {
                                    match = false;
                                    break;
                                }
                                if (propertyInfo.GetValue(disp) == null)
                                {
                                    match = false;
                                    break;
                                }
                                if (!propertyInfo.GetValue(disp).Equals(propertyInfo.GetValue(item)))
                                {
                                    match = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (match) { displayList.Add(disp); }

                }
            }

            return displayList;
		}

        /// <summary>
        /// Creates a display mask from a database business object.
        /// </summary>
        /// <returns>The business display mask object.</returns>
        /// <param name="bus">Business object contained in user database table.</param>
        private BusinessesDisplay CreateDisplay(Businesses bus) {

            BusinessesDisplay disp = new BusinessesDisplay(bus);

            BusinessUserJoin entry = _businessUserJoinContext.BusinessUserJoin.Where
                                        (b => b.businessID == disp.businessID && b.roleID == 2).SingleOrDefault();
            if(entry != null) {
                disp.owner = _userContext.Users.Where(u => u.userID == entry.userID).SingleOrDefault();
            }

            return disp;

        }

        /// <summary>
        /// Add a business to the database, along with the owner as a user, if included.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="item">Business object contained in user database table.</param>
        public void Add(BusinessesDisplay item, out int status)
        {
            Businesses bus = new Businesses(item);
            Businesses tempBus = _businessContext.Businesses.OrderByDescending(b => b.businessID).FirstOrDefault();
            if(tempBus != null) {
                bus.businessID = tempBus.businessID + 1;
            } else {
                bus.businessID = 1;
            }

            //Update business table with business information
            _businessContext.Businesses.Add(bus);
            try
            {
                _businessContext.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                Console.WriteLine(e);
                status = 507;
                return;
            }
            if(item.owner != null) {
                Users tempUser = _userContext.Users.OrderByDescending(u => u.userID).FirstOrDefault();
                if(tempUser != null) {
                    item.owner.userID = tempUser.userID + 1;
                } else {
                    item.owner.userID = 1;
                }
                item.owner.lastModified = DateTime.Now.ToString();

                //If owner was included, add the owner to the users table
                _userContext.Users.Add(item.owner);
                try
                {
                    _userContext.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e);
                    status = 507;
                    return;
                }
                BusinessUserJoin joinEntry = new BusinessUserJoin();
                joinEntry.roleID = 2;
                joinEntry.businessID = bus.businessID;
                joinEntry.userID = item.owner.userID;

                BusinessUserJoin tempJoin = _businessUserJoinContext.BusinessUserJoin.OrderByDescending(j => j.id).FirstOrDefault();
                if(tempJoin != null) {
                    joinEntry.id = tempJoin.id + 1;
                } else {
                    joinEntry.id = 1;
                }

                //update the join link between the owner and the new business
                _businessUserJoinContext.BusinessUserJoin.Add(joinEntry);
                try
                {
                    _businessUserJoinContext.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e);
                    status = 507;
                    return;
                }            
            }
            status = 201;
        }

        /// <summary>
        /// Update the specified business information withing the business table.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="item">Business object contained in user database table.</param>
        public void Update(Businesses item, out int status)
		{
			var itemToUpdate = _businessContext.Businesses.SingleOrDefault(r => r.businessID == item.businessID);
			if (itemToUpdate != null)
			{
                if(item.businessName != null) {
                    itemToUpdate.businessName = item.businessName;
                }
                if(item.address != null) {
                    itemToUpdate.address = item.address;
                }
                if(item.phoneNumber != null) {
                    itemToUpdate.phoneNumber = item.phoneNumber;
                }
                if(item.industry != 0) {
                    itemToUpdate.industry = item.industry;
                }
                if(item.website != null) {
                    itemToUpdate.website = item.website;
                }
                try
                {
                    _businessContext.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e);
                    status = 507;
                    return;
                }			
            }
            status = 200;
            return;
		}

        /// <summary>
        /// Remove the specified business from database, if exists.
        /// </summary>
        /// <returns><c>true</c> if business exists, <c>false</c> otherwise.</returns>
        /// <param name="id">Identifier.</param>
        public bool Remove(int id, out int status)
        {
            var itemToRemove = _businessContext.Businesses.SingleOrDefault(r => r.businessID == id);
            if (itemToRemove != null)
            {
                _businessContext.Businesses.Remove(itemToRemove);
                try
                {
                    _businessContext.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e);
                    status = 507;
                    return false;
                }
                status = 200;
                return true;
            }
            status = 400;
            return false;
        }
	}
}

