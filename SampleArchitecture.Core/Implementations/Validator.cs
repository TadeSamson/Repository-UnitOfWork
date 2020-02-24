using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleArchitecture.Core.Implementations
{
    public class Validator<T>
    {
        private Func<T, Task<ValidationResult>> validationDelegate;
        public Validator(Func<T, Task<ValidationResult>> _validationDelegate)
        {
            this.validationDelegate = _validationDelegate;
        }
        public Task<ValidationResult> Validate(T obj)
        {
            if (validationDelegate == null)
            {
                throw new Exception("");
            }
           return  validationDelegate.Invoke(obj);
        }
    }
}