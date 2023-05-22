using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBindable<T>
{
    public void Bind(T obj);
}
