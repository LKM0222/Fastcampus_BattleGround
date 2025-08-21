using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 갖고있던 transition, action을 갖고있고 화면에 보여주기위한 기즈모 가지고있음
/// 여기서 액션 호출, 스테이트 바뀌면 여기서 호출하게 됨.
/// 스크립터블 오브젝트로 만들어서 각각 정의함.
/// </summary>
[CreateAssetMenu(menuName = ("PluggableAI/State"))]
public class State : ScriptableObject
{
    public Action[] actions;
    public Transition[] transitions;
    public Color sceneGizmoColor = Color.gray; // 상태에 따른 기즈모 색상 변경위함.

    public void DoActions(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }

    public void OnEnableActions(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].OnReadyAction(controller);
        }

        for (int i = transitions.Length - 1; i >= 0; i--)
        {
            transitions[i].decision.OnEnableDecision(controller);
        }
    }

    public void CheckTransitions(StateController controller)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            bool decision = transitions[i].decision.Decide(controller);

            if (decision)
            {
                controller.TransitionToState(transitions[i].trueState, transitions[i].decision);
            }
            else
            {
                controller.TransitionToState(transitions[i].falseState, transitions[i].decision);
            }

            if (controller.currentState != this) // 만약 현재 스테이트가 이 스크립트와 다르다면 상태가 바뀐것
            {
                controller.currentState.OnEnableActions(controller);
                break;
            }
        }

    }
}
