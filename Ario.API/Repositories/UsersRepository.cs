/** 
 * Filename: UsersRepository.cs
 * Description: Contains the source logic for API endpoints for managing and 
 *              editing users registered with the Ario platform.
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
using Ario.API.Models.Authentication;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace Ario.API.Repositories
{

    /// <summary>
    /// This class contains the source logic for API endpoints for managing and 
    /// editing users registered with the Ario platform.
    /// <seealso cref="Controllers.BusinessesController"/>
    /// <seealso cref="Controllers.UsersController"/>
    /// <seealso cref="Controllers.TeamsController"/>
    /// <seealso cref="Controllers.NodesController"/>
    /// </summary>
	public class UsersRepository : IUsersRepository
	{
		UsersContext _userContext;
        BusinessUserJoinContext _busUserJoinContext;
        BusinessesContext _busContext;
        UserTeamJoinContext _userTeamContext;
        BusinessRolesContext _businessRoleContext;
        TeamRolesContext _teamRoleContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Ario.API.Repositories.UsersRepository"/> class.
        /// </summary>
        /// <param name="userContext">User table context from database.</param>
        /// <param name="busUserJoinContext">Business-User Join table context from database.</param>
        /// <param name="busContext">Business table context from database.</param>
        /// <param name="userTeamContext">User-Team Join table context from database.</param>
        /// <param name="businessRoleContext">Business Roles table context from database.</param>
        /// <param name="teamRoleContext">Team Roles table context from database.</param>
        public UsersRepository(UsersContext userContext, 
                               BusinessUserJoinContext busUserJoinContext, 
                               BusinessesContext busContext, 
                               UserTeamJoinContext userTeamContext, 
                               BusinessRolesContext businessRoleContext,
                               TeamRolesContext teamRoleContext)
		{
            _userContext = userContext;
            _busUserJoinContext = busUserJoinContext;
            _busContext = busContext;
            _userTeamContext = userTeamContext;
            _businessRoleContext = businessRoleContext;
            _teamRoleContext = teamRoleContext;
		}

        /// <summary>
        /// Validates that user infomation received from Okta corresponds 
        /// to an appropriate user in the Okta user table
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="authorization">Authorization token from Okta.</param>
        /// <param name="statusCode">Status code pointer.</param>
        public Users AuthenticateUser(string authorization, out int statusCode) {
            AuthUserInfo info;

            if (authorization == null)
            {
                statusCode = 401;
                return null;
            }

            using (var client = new WebClient())
            {
                client.Headers.Add("Authorization", authorization);

                try
                {
                    Stream data = client.OpenRead("https://dev-932160.oktapreview.com/oauth2/v1/userinfo");
                    StreamReader reader = new StreamReader(data);
                    string s = reader.ReadToEnd();

                    info = JsonConvert.DeserializeObject<AuthUserInfo>(s);

                    data.Close();
                    reader.Close();
                }
                catch (WebException e)
                {
                    Console.Write(e);
                    statusCode = 401;
                    return null;
                }
            }

            if (info != null)
            {
                Console.WriteLine(info.email);
                Users user = _userContext.Users.SingleOrDefault(u => u.email.Equals(info.email));
                if(user == null) {
                    statusCode = 404;
                    return null;
                } else {
                    statusCode = 200;
                    return user;
                }
            }
            statusCode = 404;
            return null;
        }

        /// <summary>
        /// Find and return the specified user, if exists.
        /// </summary>
        /// <returns>The user object found, or null.</returns>
        /// <param name="id">User identifier.</param>
        public UsersDisplay Find(int id)
        {
            int status;

            Users user = _userContext.Users.SingleOrDefault(e => e.userID == id);
            UsersDisplay disp = CreateDisplay(user, out status);

            return disp;
        }

        /// <summary>
        /// Checks if the user exists in the Ario database, and is assigned to 
        /// the specified business
        /// </summary>
        /// <returns><c>true</c>, if user exists, <c>false</c> otherwise.</returns>
        /// <param name="userID">User Identifier.</param>
        /// <param name="busID">Business Identifier.</param>
		public bool UserExists(int userID, int busID)
		{
            Users user = _userContext.Users.Where(e => e.userID == userID).SingleOrDefault();
            if (user != null) {
                return true;
            }
            return false;
		}

        /// <summary>
        /// Converts a User database object into a display mask to send to the end user.
        /// </summary>
        /// <returns>The user display mask object.</returns>
        /// <param name="user">Business object contained in user database table.</param>
        /// <param name="statusCode">Status code pointer.</param>
        public UsersDisplay CreateDisplay(Users user, out int statusCode) {

            UsersDisplay disp = new UsersDisplay(user);

            BusinessUserJoin userJoin = _busUserJoinContext.BusinessUserJoin.FirstOrDefault(j => j.userID == disp.userID);
            if(userJoin != null) {
                disp.businessID = userJoin.businessID;
                disp.businessRoleID = userJoin.roleID;
                statusCode = 200;
                return disp;
            }
            statusCode = 400;
            return disp;
        }

        /// <summary>
        /// Gets a list of users filtered by user object parameter
        /// </summary>
        /// <returns>A list of filtered users.</returns>
        /// <param name="item">User object contained in user database table with desired fields.</param>
		public IEnumerable<UsersDisplay> GetAll(Users item)
		{

            List<UsersDisplay> displayList = new List<UsersDisplay>();

            if (item != null)
            {
                List<Users> nodeList = _userContext.Users.ToList();

                foreach (Users entry in nodeList)
                {
                    bool match = true;
                    UsersDisplay disp = CreateDisplay(entry);

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
        /// Gets all users in the specified business.
        /// </summary>
        /// <returns>The list of users in a business.</returns>
        /// <param name="id">Business Identifier.</param>
        public IEnumerable<UsersDisplay> GetByBusiness(int id) {
            
            List<UsersDisplay> dispList = new List<UsersDisplay>();

            List<BusinessUserJoin> joinList = _busUserJoinContext.BusinessUserJoin.Where(j => j.businessID == id).ToList();
            if (joinList != null)
            {
                foreach (BusinessUserJoin joinEntry in joinList)
                {
                    Users user = _userContext.Users.Where(u => u.userID == joinEntry.userID).SingleOrDefault();
                    if (user != null)
                    {
                        UsersDisplay disp = new UsersDisplay(user, joinEntry);
                        dispList.Add(disp);
                    }
                }
            }
            return dispList;
        }

        /// <summary>
        /// Gets all of the users on a specified team contained in a specified business.
        /// </summary>
        /// <returns>A list of users on the team.</returns>
        /// <param name="busID">Business identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        public IEnumerable<UsersDisplay> GetUsersOnTeam(int busID, int teamID)
        {
            List<UsersDisplay> dispList = new List<UsersDisplay>();

            List<UserTeamJoin> joinList = _userTeamContext.UserTeamJoin.Where(j => j.teamID == teamID).ToList();
            foreach (UserTeamJoin joinEntry in joinList)
            {
                Users tempUser = _userContext.Users.Where(u => u.userID == joinEntry.userID).SingleOrDefault();
                UsersDisplay disp = CreateDisplay(busID, teamID, tempUser);

                dispList.Add(disp);
            }

            return dispList;
        }

        //TODO: May be a redundant function
        /// <summary>
        /// Converts a User database object into a display mask to send to the end user.
        /// </summary>
        /// <returns>The user display mask object.</returns>
        /// <param name="user">Business object contained in user database table.</param>
        private UsersDisplay CreateDisplay(Users user)
        {
            BusinessUserJoin tempJoin = _busUserJoinContext.BusinessUserJoin.Where(j => j.userID == user.userID).SingleOrDefault();

            UsersDisplay disp = new UsersDisplay(user, tempJoin);

            return disp;
        }

        /// <summary>
        /// Converts a User database object into a display mask, with apprpriate team and business information.
        /// </summary>
        /// <returns>The user display mask object.</returns>
        /// <param name="user">Business object contained in user database table.</param>
        /// <param name="busID">Business identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        private UsersDisplay CreateDisplay(int busID, int teamID, Users user) {
            BusinessUserJoin tempJoin = _busUserJoinContext.BusinessUserJoin.Where(j => j.businessID == busID && j.userID == user.userID).SingleOrDefault();

            UserTeamJoin teamJoin = _userTeamContext.UserTeamJoin.Where(t => t.userID == user.userID && t.teamID == teamID).SingleOrDefault();

            UsersDisplay disp = new UsersDisplay(user, tempJoin, teamJoin);

            return disp;
        }

        /// <summary>
        /// Adds a user to a specified business
        /// </summary>
        /// <returns><c>true</c>, if user was successfully added to the business, <c>false</c> otherwise.</returns>
        /// <param name="id">Business Identifier.</param>
        /// <param name="item">The user display object containing information to be added</param>
        public bool AddByBusiness(int id, UsersDisplay item, out int status) {

            Users existingUser = _userContext.Users.Where(u => u.email.Equals(item.email)).FirstOrDefault();
            if (existingUser != null) {
                status = 409;
                return false;
            }

            Businesses bus = _busContext.Businesses.Where(b => b.businessID == id).SingleOrDefault();
            if (bus != null)
            {
                Users user = new Users(item, bus);
                Users tempUser = _userContext.Users.OrderByDescending(u => u.userID).FirstOrDefault();
                if (tempUser != null)
                {
                    user.userID = tempUser.userID + 1;
                }
                else
                {
                    user.userID = 1;
                }

                _userContext.Users.Add(user);
                try
                {
                    _userContext.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e);
                    status = 507;
                    return false;
                }
                BusinessUserJoin joinEntry = new BusinessUserJoin();
                joinEntry.businessID = id;
                joinEntry.userID = user.userID;
                joinEntry.roleID = 5;

                BusinessUserJoin tempJoin = _busUserJoinContext.BusinessUserJoin.OrderByDescending(j => j.id).FirstOrDefault();
                if (tempJoin != null)
                {
                    joinEntry.id = tempJoin.id + 1;
                }
                else
                {
                    joinEntry.id = 1;
                }

                _busUserJoinContext.BusinessUserJoin.Add(joinEntry);
                try
                {
                    _busUserJoinContext.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e);
                    status = 507;
                    return false;
                }
                status = 201;
                return true;
            }
            status = 400;
            return false;
        }

        /// <summary>
        /// Adds the user to a specified team, contained in a specified business.
        /// </summary>
        /// <returns><c>true</c>, if user was added to the team successfully, <c>false</c> otherwise.</returns>
        /// <param name="busID">Business identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="userID">User identifier.</param>
        public bool AddUserToTeam(int busID, int teamID, int userID, out int status) {
            BusinessUserJoin busUser = _busUserJoinContext.BusinessUserJoin.Where(j => j.businessID == busID && j.userID == userID).SingleOrDefault();
            if (busUser != null)
            {

                UserTeamJoin joinEntry = new UserTeamJoin();
                joinEntry.roleID = 3;
                joinEntry.userID = userID;
                joinEntry.teamID = teamID;

                UserTeamJoin tempJoin = _userTeamContext.UserTeamJoin.OrderByDescending(u => u.id).FirstOrDefault();
                if (tempJoin != null)
                {
                    joinEntry.id = tempJoin.id + 1;
                }
                else
                {
                    joinEntry.id = 1;
                }

                _userTeamContext.UserTeamJoin.Add(joinEntry);
                try
                {
                    _userTeamContext.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                {
                    Console.WriteLine(e);
                    status = 507;
                    return false;
                }
                status = 201;
                return true;
            }
            status = 400;
            return false;
        }

        /// <summary>
        /// Update the specified user information with the non-null fields contained
        /// in the user display mask provided
        /// </summary>
        /// <param name="userID">User identifier.</param>
        /// <param name="busID">Business identifier.</param>
        /// <param name="item">User display mask with fields to update.</param>
        public void Update(int userID, int busID, UsersDisplay item, out int status)
		{
			Users itemToUpdate = _userContext.Users.SingleOrDefault(r => r.userID == userID);
            BusinessUserJoin joinEntry = _busUserJoinContext.BusinessUserJoin.SingleOrDefault(j => j.businessID == busID && j.userID == userID);
            if (joinEntry != null)
            {
                //bool admin = (joinEntry.roleID <= 2 && joinEntry.roleID > 0);

                if (itemToUpdate != null)
                {
                    if (item.firstName != null)
                    {
                        itemToUpdate.firstName = item.firstName;
                    }
                    if (item.lastName != null)
                    {
                        itemToUpdate.lastName = item.lastName;
                    }
                    //TODO: Updating email will disable Okta login. Must resolve.
                    if (item.email != null)
                    {
                        itemToUpdate.email = item.email;
                    }
                    if(item.phoneNumber != null) {
                        itemToUpdate.phoneNumber = item.phoneNumber;
                    }
                    if(item.userPhoto != null) {
                        itemToUpdate.userPhoto = item.userPhoto;
                    }
                    if(item.company != null /*&& admin*/) {
                        itemToUpdate.company = item.company;
                    }
                    if (item.industry != 0 /*&& admin*/)
                    {
                        itemToUpdate.industry = item.industry;
                    }
                    if (item.businessRoleID != 0 /*&& admin*/) {
                        joinEntry.roleID = item.businessRoleID;
                        try
                        {
                            _busUserJoinContext.SaveChanges();
                        }
                        catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                        {
                            Console.WriteLine(e);
                            status = 507;
                            return;
                        }                    
                    }
                    itemToUpdate.lastModified = DateTime.Now.ToString();
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
                }
                status = 200;
                return;
            }
            status = 400;
            return;
		}

        //TODO: Users in multiple businesses are not handled for MVP.
        /// <summary>
        /// Removes the specified user from the specified business.
        /// Deletes the user from the Ario database.
        /// </summary>
        /// <returns><c>true</c>, if user was removed from business successfully, <c>false</c> otherwise.</returns>
        /// <param name="busID">Business identifier.</param>
        /// <param name="userID">User identifier.</param>
        public bool RemoveByBusiness(int busID, int userID, out int status) {

            Users userToRemove = _userContext.Users.Where(u => u.userID == userID).SingleOrDefault();
            if(userToRemove != null) {
                BusinessUserJoin joinToRemove = _busUserJoinContext.BusinessUserJoin.Where(j => j.businessID == busID && j.userID == userID).SingleOrDefault();
                if(joinToRemove != null) {

                    _busUserJoinContext.BusinessUserJoin.Remove(joinToRemove);
                    try
                    {
                        _busUserJoinContext.SaveChanges();
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                    {
                        Console.WriteLine(e);
                        status = 507;
                        return false;
                    }
                    _userContext.Users.Remove(userToRemove);
                    try
                    {
                        _userContext.SaveChanges();
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
            }
            status = 400;
            return false;
        }

        /// <summary>
        /// Removes the specified user from the specified team.
        /// </summary>
        /// <returns><c>true</c>, if user was removed from team successfully, <c>false</c> otherwise.</returns>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="userID">User identifier.</param>
        public bool RemoveUserFromTeam(int teamID, int userID, out int status) {
            UserTeamJoin joinToRemove = _userTeamContext.UserTeamJoin.Where(j => j.userID == userID && j.teamID == teamID).SingleOrDefault();
            if(joinToRemove != null) {
                _userTeamContext.UserTeamJoin.Remove(joinToRemove);
                try
                {
                    _userTeamContext.SaveChanges();
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

