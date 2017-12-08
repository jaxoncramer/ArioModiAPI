namespace Ario.API.Models.DisplayModels
{
	public class NodeStatusesDisplay
	{
		public int? NodeStatusID { get; set; }
		public int? NodeID { get; set; }
		public int? RoleID { get; set; }
		public int? StatusTypeID { get; set; }
		public string NodeName { get; set; }
		public string RoleName { get; set; }
		public string TimeStamp { get; set; }
		public string StatusText { get; set; }
		public string StatusStyle { get; set; }
		public string StatusTypeDescription { get; set; }
	}
}
