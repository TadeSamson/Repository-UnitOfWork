using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleArchitecture.Core.Interfaces
{

    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the identifier of the model object.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        string Id { get; set; }
    
    }


}
