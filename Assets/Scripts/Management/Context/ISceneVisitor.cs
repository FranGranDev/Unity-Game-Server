using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management
{
    public interface ISceneVisitor
    {
        public void Visited(MenuSceneContext sceneContext);
        public void Visited(GameSceneContext sceneContext);
    }
}
