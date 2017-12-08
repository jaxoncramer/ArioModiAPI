using Ario.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ario.API.Contexts
{
	public class NodeStatusesContext : DbContext
	{
		public NodeStatusesContext(DbContextOptions<NodeStatusesContext> options)
			: base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            modelBuilder.Entity<NodeStatuses>().HasKey(x => x.NodeStatusID);
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<NodeStatuses> NodeStatuses { get; set; }
	}
}
