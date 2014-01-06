using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Hubs
{
    [HubName("livescores")]
    public class LiveScoresHub : Hub
    {
        private readonly LiveScores liveScores;

        public LiveScoresHub() : this(LiveScores.Instance) { }

        public LiveScoresHub(LiveScores liveScoresInstance)
        {
            liveScores = liveScoresInstance;
        }

        public void GetTotalMatches()
        {
            // liveScores.GetTotalMatches(
            Clients.Caller.TotalMatches(200);
        }
    }

    // singleton class that is used to persist variable values between 
    // different calls to hub methods
    public class LiveScores
    {
        private readonly static Lazy<LiveScores> _instance = new Lazy<LiveScores>(
            () => new LiveScores(GlobalHost.ConnectionManager.GetHubContext<LiveScoresHub>()));

        private LiveScores(IHubContext hub)
        {
            Hub = hub;
        }

        private IHubContext Hub
        {
            get;
            set;
        }

        public static LiveScores Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public void BroadcastTotalMatches(int totalMatches)
        {
            Hub.Clients.All.RefreshMatches(totalMatches);
        }
    }
}