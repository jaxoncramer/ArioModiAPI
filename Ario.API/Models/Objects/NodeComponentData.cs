using System;
namespace Ario.API.Models.Objects
{
    public abstract class NodeComponentData
    {
        public Int64 nodeComponentID { get; set; }
        public Int64 nodeID { get; set; }
        public string type { get; set; }
    }
}
