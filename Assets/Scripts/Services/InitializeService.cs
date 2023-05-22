using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public static class InitializeService
    {
        public static void Initialize<T>(Component origin, T obj, bool includeInactive = true)
        {
            origin.GetComponentsInChildren<Initializable<T>>(includeInactive)
                .ToList()
                .ForEach(x => x.Initialize(obj));
        }
        public static void Initialize(Component origin, bool includeInactive = true)
        {
            origin.GetComponentsInChildren<Initializable>(includeInactive)
                .ToList()
                .ForEach(x => x.Initialize());
        }

        public static void Bind<T>(Component origin, T obj, bool includeInactive = true)
        {
            origin.GetComponentsInChildren<IBindable<T>>(includeInactive)
                .ToList()
                .ForEach(x => x.Bind(obj));
        }
    }
}

