/** 
 * Filename: NodesContext.cs
 * Description: Linker file to connect the API to the Nodes table in the 
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
	public class NodesContext : DbContext
	{
		public NodesContext(DbContextOptions<NodesContext> options)
			: base(options) { }

		public DbSet<Nodes> Nodes { get; set; }
	}
}
