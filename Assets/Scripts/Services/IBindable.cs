using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public interface IBindable<T>
    {
        public void Bind(T obj);
    }
}