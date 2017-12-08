using System;
namespace Ario.API.Models.Objects
{
    public class PDFNodeComponentData : NodeComponentData
    {
        public string pdfLink { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
}
