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

        public static int HostPort
        {
            get
            {
                return PlayerPrefs.GetInt("portNumber", 8001);
            }
            set
            {
                PlayerPrefs.SetInt("portNumber", value);
            }
        }

        public static string JoinAddress
        {
            get
            {
                return PlayerPrefs.GetString("joinAddress", "127.0.0.1");
            }
            set
            {
                PlayerPrefs.SetString("joinAddress", value);
            }
        }
        public static int JoinPort
        {
            get
            {
                return PlayerPrefs.GetInt("joinPortNumber", 8001);
            }
            set
            {
                PlayerPrefs.SetInt("joinPortNumber", value);
            }
        }
    }
}