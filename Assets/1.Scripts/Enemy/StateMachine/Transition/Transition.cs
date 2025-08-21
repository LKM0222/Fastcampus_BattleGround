using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Transition
{
    public Decision decision; // 상태 결정 
    public State trueState; // true일때 상태
    public State falseState;  // false일때 상태 

}
