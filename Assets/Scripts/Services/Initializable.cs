using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface Initializable
    {
        public void Initialize();
    }
    public interface Initializable<T>
    {
        public void Initialize(T obj);
    }
}
