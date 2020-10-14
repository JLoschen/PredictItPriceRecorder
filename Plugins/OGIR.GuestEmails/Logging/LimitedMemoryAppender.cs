using log4net.Appender;
using log4net.Core;

namespace OGIR.GuestEmails.Logging
{
    public class LimitedMemoryAppender : MemoryAppender
    {
        private const int EventLimit = 50;

        protected override void Append(LoggingEvent loggingEvent)
        {
            base.Append(loggingEvent);
            if (m_eventsList.Count > EventLimit)
            {
                m_eventsList.RemoveAt(0);
            }
        }
    }
}