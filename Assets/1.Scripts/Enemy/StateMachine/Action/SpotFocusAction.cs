using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이동하지 않고, 한 지점을 바라봄.
[CreateAssetMenu(menuName = "PluggableAI/Actions/SpotFocusAction")]
public class SpotFocusAction : Action
{
    public override void Act(StateController controller)
    {
        controller.nav.destination = controller.personalTarget;
        controller.nav.speed = 0f;
    }
}
