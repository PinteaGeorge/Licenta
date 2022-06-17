namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();
        public Task<bool> UserConnected(string username,string connectionId)
        {
            bool isONline = false;
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> {connectionId});
                    isONline = true;
                }
            }
            return Task.FromResult(isONline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username))
                {
                    return Task.FromResult(isOffline);
                }
                OnlineUsers[username].Remove(connectionId);
                if(OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }
            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUesrs;
            lock (OnlineUsers)
            {
                onlineUesrs = OnlineUsers.OrderBy(x => x.Key).Select(x => x.Key).ToArray();
            }
            return Task.FromResult(onlineUesrs);
        }

        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }
            return Task.FromResult(connectionIds);
        }
    }
}
