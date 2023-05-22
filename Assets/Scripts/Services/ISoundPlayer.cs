using UnityEngine;

namespace Services
{
    public interface ISoundPlayer
    {
        void PlaySound(string id, float volume = 1f, float tone = 1f, float Delay = 0);
        void PlayMusic(string id, float volume = 1f, float tone = 1f, float Delay = 0);
    }
    
}
