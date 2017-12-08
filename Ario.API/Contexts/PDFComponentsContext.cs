/** 
 * Filename: PDFComponentsContext.cs
 * Description: Linker file to connect the API to the PDFComponents table in the 
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
	public class PDFComponentsContext : DbContext
	{
		public PDFComponentsContext(DbContextOptions<PDFComponentsContext> options)
			: base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PDFComponents>().HasKey(x => x.id);
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<PDFComponents> PDFComponents { get; set; }
	}
}
