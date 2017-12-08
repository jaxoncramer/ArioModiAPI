/** 
 * Filename: UsersContext.cs
 * Description: Linker file to connect the API to the Users table in the 
 *              Ario database
 * 
 * Author: Jaxon Cramer, November 2017
 *
 * Copyright: The following source code is the sole property of Ario, Inc., all
 *              rights reserved. This code should not be copied or distributed
 *              without the express written consent of Ario, Inc.
 * 
 **/

using Ario.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ario.API.Contexts
{
	public class UsersContext : DbContext
	{
		public UsersContext(DbContextOptions<UsersContext> options)
			: base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            modelBuilder.Entity<Users>().HasKey(x => x.userID);
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Users> Users { get; set; }
	}
}
