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

    // 이펙트용 클립(툴용 클립)
    private GameObject effectSource = null;
    private static EffectData effectData;

    [MenuItem("Tools/Effect Tool")]
    static void Init()
    {
        effectData = ScriptableObject.CreateInstance<EffectData>();
        effectData.LoadData();

        EffectTool window = GetWindow<EffectTool>(false, "Effect Tool");
        window.Show();
    }

    private void OnGUI()
    {
        if (effectData == null)
        {
            return;
        }

        EditorGUILayout.BeginVertical();
        {
            // 상단 Add, Rmove, Copy 영역
            UnityObject source = effectSource;
            EditorHelper.EditorToolTopLayer(effectData, ref selection, ref source, uiWidthMiddle);
            effectSource = (GameObject)source;

            // 중간, 데이터 목록 
            EditorGUILayout.BeginHorizontal();
            {
                EditorHelper.EditorToolListLayer(ref SP1, effectData, ref selection, ref source, uiWidthLarge);
                effectSource = (GameObject)source;

                // 설정 부분
                EditorGUILayout.BeginVertical();
                {
                    SP2 = EditorGUILayout.BeginScrollView(SP2);
                    {
                        if (effectData.GetDataCount() > 0)
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("ID", selection.ToString(), GUILayout.Width(uiWidthLarge));

                                effectData.names[selection] = EditorGUILayout.TextField("이름.", effectData.names[selection], GUILayout.Width(uiWidthLarge * 1.5f));
                                effectData.effectClips[selection].effectType = (EffectType)EditorGUILayout.EnumPopup("이펙트 타입", effectData.effectClips[selection].effectType, GUILayout.Width(uiWidthLarge));

                                EditorGUILayout.Separator();
                                if (effectSource == null && effectData.effectClips[selection].effectName != string.Empty)
                                {
                                    effectData.effectClips[selection].PreLoad();
                                    effectSource = Resources.Load(effectData.effectClips[selection].effectPath +
                                                            effectData.effectClips[selection].effectName) as GameObject;
                                }

                                effectSource = (GameObject)EditorGUILayout.ObjectField("이펙트", effectSource, typeof(GameObject), false, GUILayout.Width(uiWidthLarge * 1.5f));
                                if (effectSource != null)
                                {
                                    effectData.effectClips[selection].effectPath = EditorHelper.GetPath(effectSource);
                                    effectData.effectClips[selection].effectName = effectSource.name;
                                }
                                else
                                {
                                    effectData.effectClips[selection].effectPath = string.Empty;
                                    effectData.effectClips[selection].effectName = string.Empty;
                                }
                                EditorGUILayout.Separator();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        // 하단 Reload Save
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Reload Settings"))
            {
                effectData = CreateInstance<EffectData>();
                effectData.LoadData();
                selection = 0;
                effectSource = null;
            }

            if (GUILayout.Button("Save"))
            {
                EffectTool.effectData.SaveData();
                CreateEnumStructure();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate); // 한번 리프레쉬해라
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public void CreateEnumStructure()
    {
        string enumName = "EffectList";
        StringBuilder builder = new StringBuilder();
        builder.AppendLine();
        for (int i = 0; i < effectData.names.Length; i++)
        {
            if (effectData.names[i] != string.Empty)
            {
                builder.AppendLine("    " + effectData.name[i] + "=" + i + ",");
            }
        }
        EditorHelper.CreateEnumStructure(enumName, builder);
    }

}
