using System;
using System.Collections.Generic;
using SampleArchitecture.Core.Interfaces;

namespace SampleArchitecture.Core.Models
{
    public partial class Tag : IEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string UserId { get; set; }
    }
}
