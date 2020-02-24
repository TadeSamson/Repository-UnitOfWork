using SampleArchitecture.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleArchitecture.Infrastructure.Utils
{
    internal static class Extension
    {

        public static  GetOptionToDocumentQuery<T> ToDocumentQuery<T>(this GetOption<T> option)
        {
            GetOptionToDocumentQuery<T> getOptionToDocumentQuery = new GetOptionToDocumentQuery<T>(option);
            return getOptionToDocumentQuery;
        }
    }
}
