using System;

namespace Services
{
    public interface IGameEvents
    {
        public event Action OnStart;
        public event Action OnEnd;
    }
}