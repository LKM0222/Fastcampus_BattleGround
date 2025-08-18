using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// data의 기본 클래스입니다.
/// </summary>
public class BaseData : ScriptableObject
{
    public const string dataDirectory = "/9.ResourcesData/Resources/Data/";
    public string[] names = null;

    public BaseData() { }

    public int GetDataCount()
    {
        int retValue = 0;
        if (names != null)
        {
            retValue = names.Length;
        }
        return retValue;
    }

    /// <summary>
    /// 툴에 출력하기 위한 이름 목록을 만들어주는 함수
    /// </summary>
    /// <param name="showId"></param>
    /// <param name="filterWord"></param>
    /// <returns></returns>
    public string[] GetNameList(bool showId, string filterWord = " ")
    {
        string[] retList = new string[0];

        if (names == null)
        {
            return retList;
        }

        retList = new string[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            if (filterWord != "") // 필터가 지정되어있다면
            {
                // 필터를 비교하기위해 전체 소문자로 바꿔서 포함되는지 확인
                if (names[i].ToLower().Contains(filterWord.ToLower()) == false)
                {
                    continue;
                }
            }

            if (showId == true)
            {
                retList[i] = i.ToString() + ":" + names[i];
            }
            else
            {
                retList[i] = names[i];
            }
        }

        return retList;
    }

    public virtual int AddData(string newName)
    {
        return GetDataCount();
    }

    public virtual void RemoveData(int index)
    {

    }

    public virtual void Copy(int index)
    {

    }
}
