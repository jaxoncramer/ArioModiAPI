/** 
 * Filename: TeamsRepository.cs
 * Description: Contains the source logic for API endpoints for managing and 
 *              editing teams registered with the Ario platform.
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
    /// editing teams registered with the Ario platform.
    /// </summary>
    /// <seealso cref="Controllers.TeamsController"/>
    /// <seealso cref="Controllers.BusinessesController"/>
	public class TeamsRepository : ITeamsRepository
	{
      	TeamsContext _teamContext;
        UsersContext _userContext;
        UserTeamJoinContext _userTeamContext;
        BusinessUserJoinContext _busUserContext;
        TeamRolesContext _teamRoleContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Ario.API.Repositories.TeamsRepository"/> class.
        /// </summary>
        /// <param name="context">Teams table context from database.</param>
        /// <param name="userContext">Users table context from database.</param>
        /// <param name="userTeamContext">User-Team Join table context from database.</param>
        /// <param name="busUserContext">Business-User table context from database.</param>
        /// <param name="teamRoleContext">Team roles table context from database.</param>
        public TeamsRepository(TeamsContext context, UsersContext userContext, UserTeamJoinContext userTeamContext, BusinessUserJoinContext busUserContext, TeamRolesContext teamRoleContext)
		{
			_teamContext = context;
            _userContext = userContext;
            _userTeamContext = userTeamContext;
            _busUserContext = busUserContext;
            _teamRoleContext = teamRoleContext;
		}

        /// <summary>
        /// Find and return the specified team, if exists.
        /// </summary>
        /// <returns>The team object found, or null.</returns>
        /// <param name="id">Team identifier.</param>
        public TeamsDisplay Find(int id)
		{
            Teams team = _teamContext.Teams.Where(e => e.teamID == id).SingleOrDefault();
            if (team != null)
            {
                return CreateDisplay(team);
            }
            return null;
		}

        /// <summary>
        /// Checks if the team exists for specified business in the Ario database
        /// </summary>
        /// <returns><c>true</c>, if team exists, <c>false</c> otherwise.</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="teamID">Team Identifier.</param>
        public bool TeamExists(int busID, int teamID) {
            Teams team = _teamContext.Teams.Where(e => e.teamID == teamID && e.businessID == busID).SingleOrDefault();
            if (team != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determine if the team is a part of the business
        /// </summary>
        /// <returns><c>true</c>, if team on business was ised, <c>false</c> otherwise.</returns>
        /// <param name="busID">Business identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="status">Status.</param>
        public bool IsTeamOnBusiness(int busID, int teamID, out int status)
        {
            Teams team = _teamContext.Teams.SingleOrDefault(t => t.teamID == teamID);
            if (team != null) {
                if (team.businessID != busID) {
                    status = 400;
                    return false;
                }
                status = 200;
                return true;
            }
            status = 400;
            return false;
        }

        /// <summary>
        /// Checks of a user is part of a team, and that team is assigned 
        /// to the user's business.
        /// </summary>
        /// <returns><c>true</c>, if user is assigned to the team, <c>false</c> otherwise.</returns>
        /// <param name="busID">Business identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="userID">User identifier.</param>
        public bool UserIsMember(int busID, int teamID, int userID) {
            BusinessUserJoin busUser = _busUserContext.BusinessUserJoin.SingleOrDefault(b => b.businessID == busID && b.userID == userID);
            if(busUser != null) {
                UserTeamJoin userTeam = _userTeamContext.UserTeamJoin.SingleOrDefault(u => u.userID == userID && u.teamID == teamID);
                if(userTeam != null) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a list of teams filtered by business object parameter
        /// </summary>
        /// <returns>A list of filtered teams.</returns>
        /// <param name="item">Team object contained in user database table with desired fields.</param>
		public IEnumerable<TeamsDisplay> GetSelect(Teams item)
		{

            List<TeamsDisplay> displayList = new List<TeamsDisplay>();

            if (item != null)
            {
                List<Teams> teamList = _teamContext.Teams.ToList();

                foreach (Teams entry in teamList)
                {
                    bool match = true;
                    TeamsDisplay disp = CreateDisplay(entry);

                    Type type = disp.GetType();
                    foreach (PropertyInfo propertyInfo in type.GetProperties())
                    {
                        if (propertyInfo.CanRead)
                        {
                            if (propertyInfo.GetValue(item) != null /*&& !propertyInfo.GetValue(item).ToString().Equals("0")*/)
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
        /// Gets the teams that the user is a member of.
        /// </summary>
        /// <returns>List of teams that have the user as a member</returns>
        /// <param name="busID">Business identifier.</param>
        /// <param name="userID">User identifier.</param>
        public IEnumerable<TeamsDisplay> GetUserTeams(int busID, int userID) {

            List<TeamsDisplay> dispList = new List<TeamsDisplay>();

            List<Teams> teamList = _teamContext.Teams.Where(t => t.businessID == busID).ToList();
            foreach (Teams team in teamList) {
                if (_userTeamContext.UserTeamJoin.SingleOrDefault(u => u.userID == userID && u.teamID == team.teamID) != null) {
                    dispList.Add(CreateDisplay(team));
                }
            }

            return dispList;
        }

        /// <summary>
        /// Get all of the teams in a specified business
        /// </summary>
        /// <returns>List of the teams in the business (empty if none are found).</returns>
        /// <param name="id">Business Identifier.</param>
        public IEnumerable<TeamsDisplay> GetByBusiness(int id) {
            List<TeamsDisplay> dispList = new List<TeamsDisplay>();

            List<Teams> teamList = _teamContext.Teams.Where(t => t.businessID == id).ToList();
            if (teamList != null)
            {
                foreach (Teams team in teamList)
                {
                    TeamsDisplay disp = CreateDisplay(team);
                    dispList.Add(disp);
                }
            }
            return dispList;
        }

        /// <summary>
        /// Creates a display mask from a database team object.
        /// </summary>
        /// <returns>The team display mask object.</returns>
        /// <param name="team">Team object contained in team database table.</param>
        private TeamsDisplay CreateDisplay(Teams team) {

            TeamsDisplay disp = new TeamsDisplay(team);

            List<UserTeamJoin> joinList = _userTeamContext.UserTeamJoin.Where(j => j.teamID == disp.teamID).ToList();
            foreach(UserTeamJoin joinEntry in joinList) {
                Users user = _userContext.Users.Where(u => u.userID == joinEntry.userID).SingleOrDefault();
                if(user != null) {

                    UsersDisplay userDisp = CreateUserDisplay((int) team.businessID, (int) team.teamID, user);

                    disp.users.Add(userDisp);
                }
            }
            return disp;
        }

        /// <summary>
        /// Creates a display mask from a database user object.
        /// </summary>
        /// <remarks>NOTE: This is a redundant function to one contained in UsersRepository.cs</remarks>
        /// <returns>The user display mask object.</returns>
        /// <param name="busID">Business identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="user">User object contained in user database table.</param>
        private UsersDisplay CreateUserDisplay(int busID, int teamID, Users user)
        {
            BusinessUserJoin tempJoin = _busUserContext.BusinessUserJoin.Where(j => j.businessID == busID && j.userID == user.userID).SingleOrDefault();

            UserTeamJoin teamJoin = _userTeamContext.UserTeamJoin.Where(t => t.userID == user.userID && t.teamID == teamID).SingleOrDefault();

            UsersDisplay disp = new UsersDisplay(user, tempJoin, teamJoin);

            return disp;
        }

        /// <summary>
        /// Adds the team to the specified business.
        /// </summary>
        /// <returns><c>true</c>, if the team was added successfully, <c>false</c> otherwise.</returns>
        /// <param name="busID">Business Identifier.</param>
        /// <param name="userID">Current User Identifier.</param>
        /// <param name="item">Team display object to add.</param>
        /// <param name="status">Status code pointer.</param>
        public bool AddByBusiness(int busID, int userID, TeamsDisplay item, out int status) {
           
            Teams team = new Teams(busID, item);
            team.teamCreatorID = userID;
            Teams tempTeam = _teamContext.Teams.OrderByDescending(t => t.teamID).FirstOrDefault();
            if (tempTeam != null)
            {
                team.teamID = tempTeam.teamID + 1;
            }
            else
            {
                team.teamID = 1;
            }

            _teamContext.Teams.Add(team);
            try
            {
                _teamContext.SaveChanges();
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

        /// <summary>
        /// Update the specified team name in the team table.
        /// </summary>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="busID">Business identifier.</param>
        /// <param name="item">Team object containing team name to change.</param>
        /// <param name="status">Status code pointer.</param>
        public void Update(int teamID, int busID, Teams item, out int status)
        {
            var itemToUpdate = _teamContext.Teams.SingleOrDefault(r => r.teamID == teamID && r.businessID == busID);
            if (itemToUpdate != null)
            {
                //itemToUpdate.teamCreatorID = item.teamCreatorID;
                if (item.teamName != null)
                {
                    itemToUpdate.teamName = item.teamName;
                }
                _teamContext.SaveChanges();
                status = 200;
            } else {
                status = 400;
            }

        }

        /// <summary>
        /// Edits the user role with regards to the team editing permissions
        /// </summary>
        /// <returns><c>true</c>, if role was edited successfully, <c>false</c> otherwise.</returns>
        /// <param name="teamID">Team identifier.</param>
        /// <param name="userID">User identifier.</param>
        /// <param name="item">Item.</param>
        public bool EditRole(int teamID, int userID, TeamRoles item, out int status) {
            UserTeamJoin joinEntry = _userTeamContext.UserTeamJoin.SingleOrDefault(j => j.userID == userID && j.teamID == teamID);
            if (joinEntry != null) {
                if(item.roleID != 0) {
                    joinEntry.roleID = item.roleID;
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
                if(item.roleName != null) {
                    TeamRoles role = _teamRoleContext.TeamRoles.SingleOrDefault(r => r.roleName.Equals(item.roleName));
                    if(role != null) {
                        joinEntry.roleID = role.roleID;
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
                }
            }
            status = 400;
            return false;
        }

        /// <summary>
        /// Removes the team from the specified business.
        /// </summary>
        /// <returns><c>true</c>, if team was successfully removed from the business, <c>false</c> otherwise.</returns>
        /// <param name="busID">Business identifier.</param>
        /// <param name="teamID">Team identifier.</param>
        public bool RemoveByBusiness(int busID, int teamID, out int status) {
            Teams team = _teamContext.Teams.Where(t => t.businessID == busID && t.teamID == teamID).SingleOrDefault();
            if (team != null) {
                List<UserTeamJoin> joinList = _userTeamContext.UserTeamJoin.Where(j => j.teamID == team.teamID).ToList();
                foreach(UserTeamJoin joinEntry in joinList) {
                    _userTeamContext.UserTeamJoin.Remove(joinEntry);
                    try
                    {
                        _userTeamContext.SaveChanges();
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                    {
                        Console.WriteLine(e);
                        status = 507;
                        return false;
                    }                }
                _teamContext.Teams.Remove(team);
                try
                {
                    _teamContext.SaveChanges();
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

