using System.Collections.Generic;

namespace FlightSearchFunction
{
    public class CallReturnActivityCommand
    {
        public CallReturnActivityCommand()
        {
        }

        public string ReturnUrl { get; set; }
        public SearchFlightResult Result { get; set; }
    }
}