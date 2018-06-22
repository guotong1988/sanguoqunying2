using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.Text;

public class MODLoadController {

    private static MODLoadController mInstance;
    public static MODLoadController Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new MODLoadController();
            }
            return mInstance;
        }
    }
    
	public void LoadMOD(int index)
    {
        //Informations.Reset();
        Controller.MODSelect = index;
        Informations.Instance.kingNum = Informations.Instance.modKingNum[Controller.MODSelect];

        string modName = "MOD0" + (index+1);
        XmlDocument xmlDoc = new XmlDocument();
        TextAsset textAsset = (TextAsset)Resources.Load(modName);
        xmlDoc.LoadXml(textAsset.text.ToString().Trim());
        XmlElement root = xmlDoc.DocumentElement;

        XmlElement node = (XmlElement)root.SelectSingleNode("King");
        XmlNodeList nodeList = node.ChildNodes;
        int i = 0;
        foreach (XmlElement kingNode in nodeList)
        {
            KingInfo kInfo = new KingInfo();
            kInfo.active = int.Parse(kingNode.GetAttribute("Active"));
            kInfo.generalIdx = int.Parse(kingNode.GetAttribute("GeneralIdx"));

            Informations.Instance.SetKingInfo(i++, kInfo);
        }

        i = 0;
        node = (XmlElement)root.SelectSingleNode("City");
        nodeList = node.ChildNodes;
        foreach (XmlElement cityNode in nodeList)
        {
            CityInfo cInfo = new CityInfo();
            cInfo.king = int.Parse(cityNode.GetAttribute("King"));
            cInfo.population = int.Parse(cityNode.GetAttribute("Population"));
            cInfo.money = int.Parse(cityNode.GetAttribute("Money"));
            cInfo.reservist = int.Parse(cityNode.GetAttribute("Reservist"));
            cInfo.reservistMax = int.Parse(cityNode.GetAttribute("ReservistMax"));
            cInfo.defense = int.Parse(cityNode.GetAttribute("Defense"));

            Informations.Instance.SetCityInfo(i++, cInfo);
        }

        KingInfo k = new KingInfo();
        k.generalIdx = 0;
        Informations.Instance.SetKingInfo(Informations.Instance.kingNum, k);

        i = 0;
        node = (XmlElement)root.SelectSingleNode("General");
        nodeList = node.ChildNodes;
        foreach (XmlElement generalNode in nodeList)
        {
            GeneralInfo gInfo = new GeneralInfo();
            gInfo.king = int.Parse(generalNode.GetAttribute("King"));
            gInfo.city = int.Parse(generalNode.GetAttribute("City"));
            gInfo.magic[0] = int.Parse(generalNode.GetAttribute("Magic0"));
            gInfo.magic[1] = int.Parse(generalNode.GetAttribute("Magic1"));
            gInfo.magic[2] = int.Parse(generalNode.GetAttribute("Magic2"));
            gInfo.magic[3] = int.Parse(generalNode.GetAttribute("Magic3"));
            gInfo.equipment = int.Parse(generalNode.GetAttribute("Equipment"));
            gInfo.strength = int.Parse(generalNode.GetAttribute("Strength"));
            gInfo.intellect = int.Parse(generalNode.GetAttribute("Intellect"));
            gInfo.experience = int.Parse(generalNode.GetAttribute("Experience"));
            gInfo.level = int.Parse(generalNode.GetAttribute("Level"));
            gInfo.healthMax = int.Parse(generalNode.GetAttribute("HealthMax"));
            gInfo.healthCur = int.Parse(generalNode.GetAttribute("HealthCur"));
            gInfo.manaMax = int.Parse(generalNode.GetAttribute("ManaMax"));
            gInfo.manaCur = int.Parse(generalNode.GetAttribute("ManaCur"));
            gInfo.soldierMax = int.Parse(generalNode.GetAttribute("SoldierMax"));
            gInfo.soldierCur = int.Parse(generalNode.GetAttribute("SoldierCur"));
            gInfo.knightMax = int.Parse(generalNode.GetAttribute("KnightMax"));
            gInfo.knightCur = int.Parse(generalNode.GetAttribute("KnightCur"));
            gInfo.arms = int.Parse(generalNode.GetAttribute("Arms"));
            gInfo.formation = int.Parse(generalNode.GetAttribute("Formation"));
            
            Informations.Instance.SetGeneralInfo(i++, gInfo);
        }

        Informations.Instance.InitKingInfo();
        Informations.Instance.InitCityPrefect();
        Informations.Instance.InitCityInfo();
        Informations.Instance.InitGeneralInfo();
    }
}
