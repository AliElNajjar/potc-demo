using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UnitDataManager : MonoBehaviour
{
    private void Start()
    {
        //  Save();
    }

    void SaveData()
    {
        UnitClasses info = new UnitClasses();

        string json = JsonUtility.ToJson(info, true);
        File.WriteAllText(Application.dataPath + "/Yahia/Data" + "/UnitClassesData.json", json);
    }

    void Save()
    {
        //string path = Application.dataPath + "/" + "Resources" + "/" + "UnitsParametersData.txt"; //Resources.Load("UnitClassesData").ToString();

        //Units info = new Units();

        //UnitParametersInfo y = info.Unit;
        //for (int i = 0; i < info.Unit.Length; i++)
        //{
        // for (int j = 0; j < 10; j++)
        // {

        //y = new UnitParametersInfo
        //{
        //    unitName = "Terra", 
        //    unitStatsParams = new UnitStatsParams  { 
        //      Strenght = new Strength
        //      {

        //      }
        //    }
        //};
        // }
        //info.Unit = y;
        //string json = JsonUtility.ToJson(info, true);
        //File.WriteAllText(path, json);
        // }
    }

    //void Save()
    //{
    //    UnitClasses info = new UnitClasses();

    //    UnitClassInfo[] y = info.UnitsClassesValues;
    //    for (int i = 0; i < info.UnitsClassesValues.Length; i++)
    //    {
    //        for (int j = 0; j < 10; j++)
    //        {

    //            y[j] = new UnitClassInfo
    //            {
    //                Slash = 2
    //            };
    //        }
    //        info.UnitsClassesValues = y;
    //        string json = JsonUtility.ToJson(info, true);
    //        File.WriteAllText(Application.dataPath + "/Yahia/Data" + "/UnitClassesData.json", json);
    //    }

    //}
}

[Serializable]
public class UnitClassInfo
{
    public string unitClass;
    public float Slash,
                 Crush,
                 Pierce,
                 Health,
                 Mind,
                 Sound,
                 Time,
                 Soul;
}

[Serializable]
public class UnitClasses
{
    public UnitClassInfo[] UnitsClassesValues = new UnitClassInfo[10];
}

[Serializable]
public class UnitParametersInfo
{
    public string unitName;
    public Endurance endurance;
    public Strength Strenght;
    public Intelligence intelligence;
    public Wisdom wisdom;
    public Dexterity dexterity;
    public Spirit spirit;
}

[Serializable]
public class Units
{
    public UnitParametersInfo[] Unit = new UnitParametersInfo[1];
}