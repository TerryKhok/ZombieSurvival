using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dan.Main;

public class LeaderboardSystem : MonoBehaviour
{
    [SerializeField] private List<Text> names;
    [SerializeField] private List<Text> scores;

    private string publicLeaderboardKey = "d729ab99dc6390022de73053a61bb3bff483937573f7e890fe565c40df5c9da6";

    private void Start() {
        GetLeadeboard();
    }

    public void GetLeadeboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, (msg) =>
        {
            int loopLength = (msg.Length < names.Count ? msg.Length : names.Count);
            for (int i = 0; i < loopLength; i++)
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }
        });
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, (_) =>
        {
            GetLeadeboard();
        });
    }
}
