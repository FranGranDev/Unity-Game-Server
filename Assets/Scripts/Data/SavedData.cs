using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class SavedData
    {
        public static string PlayerName
        {
            get
            {
                return PlayerPrefs.GetString("playerName", $"New Player {Random.Range(1, 100)}");
            }
            set
            {
                PlayerPrefs.SetString("playerName", value);
            }
        }

    }
}