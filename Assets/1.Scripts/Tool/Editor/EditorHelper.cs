using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using UnityObject = UnityEngine.Object;

public class EditorHelper
{

	/// <summary>
	/// 경로 계산 함수.
	/// </summary>
	/// <param name="p_clip"></param>
	/// <returns></returns>
	public static string GetPath(UnityEngine.Object p_clip)
	{
		string retString = string.Empty;
		retString = AssetDatabase.GetAssetPath(p_clip);
		string[] path_node = retString.Split('/'); //Assets/9.ResourcesData/Resources/Sound/BGM.wav
		bool findResource = false;
		for (int i = 0; i < path_node.Length - 1; i++)
		{
			if (findResource == false)
			{
				if (path_node[i] == "Resources")
				{
					findResource = true;
					retString = string.Empty;
				}
			}
			else
			{
				retString += path_node[i] + "/";
			}

		}

		return retString;
	}

	/// <summary>
	/// Data 리스트를 enum structure로 뽑아주는 함수.
	/// </summary>
	public static void CreateEnumStructure(string enumName, StringBuilder data)
	{
		string templateFilePath = "Assets/Editor/EnumTemplate.txt";

		string entittyTemplate = File.ReadAllText(templateFilePath);

		entittyTemplate = entittyTemplate.Replace("$DATA$", data.ToString());
		entittyTemplate = entittyTemplate.Replace("$ENUM$", enumName);
		string folderPath = "Assets/1.Scripts/GameData/";
		if (Directory.Exists(folderPath) == false)
		{
			Directory.CreateDirectory(folderPath);
		}

		string FilePath = folderPath + enumName + ".cs";
		if (File.Exists(FilePath))
		{
			File.Delete(FilePath);
		}
		File.WriteAllText(FilePath, entittyTemplate);
	}

	// 상단 구역과 툴에 필요한 데이터 목록을 생성해줄, 미리 만들어두는 함수.
	/// <summary>
	/// 상단구역을 만들어주는 함수
	/// </summary>
	/// <param name="baseData"></param>
	/// <param name="selection"></param>
	/// <param name="source"></param>
	/// <param name="uiWidth"></param>
	public static void EditorToolTopLayer(BaseData baseData, ref int selection, ref UnityObject source, int uiWidth)
	{
		// 특정 좌표값을 넣어 Rect를 만들어줌.
		EditorGUILayout.BeginHorizontal();
		{ // 구분 위한 중괄호
		  //버튼이 클릭되면
			if (GUILayout.Button("ADD", GUILayout.Width(uiWidth)))
			{
				baseData.AddData("New Data");
				selection = baseData.GetDataCount() - 1; // 최종 리스트를 선택
				source = null;
			}

			if (GUILayout.Button("Copy", GUILayout.Width(uiWidth)))
			{
				baseData.Copy(selection);
				source = null;
				selection = baseData.GetDataCount() - 1;
			}

			if (baseData.GetDataCount() > 1)
			{
				if (GUILayout.Button("Remove", GUILayout.Width(uiWidth)))
				{
					source = null;
					baseData.RemoveData(selection);
				}
			}

			if (selection > baseData.GetDataCount() - 1)
			{
				selection = baseData.GetDataCount() - 1;
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	/// <summary>
	/// 목록을 만들어주는 함수
	/// </summary>
	/// <param name="scrollPos"></param>
	/// <param name="data"></param>
	/// <param name="selection"></param>
	/// <param name="source"></param>
	/// <param name="uiWidth"></param>
	public static void EditorToolListLayer(ref Vector2 scrollPos, BaseData data, ref int selection, ref UnityObject source, int uiWidth)
	{
		EditorGUILayout.BeginVertical(GUILayout.Width(uiWidth));
		{
			EditorGUILayout.Separator(); //한칸 띄움.
			EditorGUILayout.BeginVertical("box");
			{
				scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
				{
					if (data.GetDataCount() > 0)
					{
						int lastSelection = selection;
						selection = GUILayout.SelectionGrid(selection, data.GetNameList(true), 1); // 1줄짜리 그리드를 만들겠다.
						if (lastSelection != selection)
						{
							source = null;
						}
					}
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
	}

}
