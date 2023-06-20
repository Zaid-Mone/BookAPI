namespace BookAPI.Utility
{
    public class ReponseStatus
    {
        public static string ReponseStateMessage(int state)
        {
            string stateMessage = string.Empty;
            switch (state)
            {
                case 200:
                    stateMessage = "Success";
                    break;
                case 400:
                    stateMessage = "Something Went Wrong";
                    break;
                case 401:
                    stateMessage = "Unauthorized";
                    break;
                case 500:
                    stateMessage = "Server Error";
                    break;
                case 403:
                    stateMessage = "Forbidden";
                    break;
                case 404:
                    stateMessage = "Not Found";
                    break;
                default:
                    stateMessage = "Unknown State";
                    break;
            }
            return stateMessage;
        }

    }
}
