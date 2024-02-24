using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dan.Main;

public class LeaderboardSystem : MonoBehaviour
{
    [SerializeField] private List<Text> names;
    [SerializeField] private List<Text> scores;

    private string publicLeaderboardKey = "b3d1a9d94b41d4cac45bfb474857244855a183958c2255ca0f7e0833c81b02fc";

    private void Start()
    {
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
            //LeaderboardCreator.ResetPlayer();
        });
    }
}
