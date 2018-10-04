using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Parts
{
    public class LeaderboardItem : MonoBehaviour
    {
        public Text TextRanking;

        public Text TextName;

        public Text TextScore;


        private const string SCORE_FORMAT = "{0}\n Wave";
        private const string RANK_FORMAT = "Rank {0}";

        /// <summary>
        /// Init View
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="score"></param>
        /// <param name="username"></param>
        public void InitView(int rank, int score, string username)
        {
            TextName.text = username;
            TextScore.text = String.Format(SCORE_FORMAT,score);

            TextRanking.text = String.Format(RANK_FORMAT, rank);
        }
        
    }
}