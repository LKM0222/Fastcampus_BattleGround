using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 실제 동작하는 업데이트 컴포넌트 
/// </summary>

public abstract class Action : ScriptableObject
{
    public abstract void Act(StateController controller);

    public virtual void OnReadyAction(StateController controller)
    {

    }
}
