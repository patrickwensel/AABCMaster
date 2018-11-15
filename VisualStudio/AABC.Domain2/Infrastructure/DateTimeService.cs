namespace AABC.Domain2.Infrastructure
{
    public class DateTimeService
    {

        static IDateTimeProvider _instance;

        public static IDateTimeProvider Current {
            get
            {
                if (_instance == null) {
                    _instance = new DefaultDateTimeProvider();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

    }

}
