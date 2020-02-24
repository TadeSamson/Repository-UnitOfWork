using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleArchitecture.Core.Implementations
{
    public struct Blob
    {
        string container { get; set; }
        public string Container { get { return this.container; } set { container = value.Trim().ToLower(); } }
        public string Filename { get; set; }
        public byte[] Bytes { get; set; }
    }
}
