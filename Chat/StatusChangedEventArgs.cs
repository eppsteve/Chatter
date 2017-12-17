using System;

namespace Chat
{
    // Holds the arguments for the StatusChanged event
    public class StatusChangedEventArgs : EventArgs
    {
        // Property for retrieving and setting the event message
        public string EventMessage { get; set; }

        // Constructor for setting the event message
        public StatusChangedEventArgs(string strEventMsg)
        {
            EventMessage = strEventMsg;
        }
    }
}
