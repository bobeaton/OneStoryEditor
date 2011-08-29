using System;
using System.Text;

namespace Gapi.Core
{
    public class GapiException : Exception
    {
        public GapiException(string message)
            : base(message)
        {
        }
    }
}
