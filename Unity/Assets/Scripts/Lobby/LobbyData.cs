using System;
using UnityEngine;

namespace CorruptStateRP.Lobby {
    [Serializable] public sealed class LobbyPlayerData {
        public string displayName="New Citizen"; public int level=1; public int xp=0; public int xpToNext=1000; public int money=1000;
    }
    [Serializable] public sealed class MissionData { public string title; public int rewardMoney; public int rewardXp; public bool completed; }
    public sealed class LobbyDataProvider : MonoBehaviour {
        public LobbyPlayerData Player {get;private set;}=new LobbyPlayerData();
        public MissionData[] DailyMissions {get;private set;} = new MissionData[]{
            new MissionData{title="Complete your first citizen task",rewardMoney=500,rewardXp=100},
            new MissionData{title="Visit the bank",rewardMoney=750,rewardXp=150},
            new MissionData{title="Meet 5 citizens",rewardMoney=1000,rewardXp=250}
        };
        public void RefreshFromServer(LobbyPlayerData serverData){if(serverData!=null)Player=serverData;}
    }
}
