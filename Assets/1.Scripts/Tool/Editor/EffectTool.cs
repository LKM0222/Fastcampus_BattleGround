using UnityEngine;
using UnityEditor; // Editor 폴더 아래에 넣어야만 제대로 동작함.
using System.Text;
using UnityObject = UnityEngine.Object;

/// <summary>
/// 
/// </summary>
public class EffectTool : EditorWindow // 상속해서 Tool Window를 띄움.
{
    // UI 그리는데 필요한 변수들.
    public int uiWidthLarge = 300; // px
    public int uiWidthMiddle = 200; // px
    private int selection = 0; // 몇번째 요소 선택했는지
    private Vector2 SP1 = Vector2.zero; // ScrollPos
    private Vector2 SP2 = Vector2.zero; // ScrollPos


}
