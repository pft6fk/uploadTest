using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uploadTest.Shared
{
    public class UploadResult
    {
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
    }
    public class Element
    {
        public int Number { get; set; }
        public string Sign { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public double Molar { get; set; }
    }
}
