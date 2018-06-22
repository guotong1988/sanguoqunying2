using UnityEngine;
using System.Collections;
using System.IO;

public class test : MonoBehaviour {

    public string[] generalName;

	void Start() {

        //Debug.Log(ZhongWen.Instance.GetCityName(37));
        for (int g = 0; g < generalName.Length; g++)
        {
            for (int i = 0; i < Informations.Instance.generalNum; i++)
            {
                if (generalName[g] == ZhongWen.Instance.GetGeneralName(i))
                {
                    Debug.Log(generalName[g] + ": " + i);
                    break;
                }
            }
        }

        /*
        FileStream fs = new FileStream(Application.dataPath + "/1.txt", FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Flush();

        for (int i = 0; i < cityNum; i++)
        {
            string str = string.Format("<Item Name=\"{0}\" King=\"{1}\" Population=\"{2}\" Money=\"{3}\" ReservistMax=\"{4}\" Reservist=\"{5}\" Defense=\"{6}\"/>",
                ZhongWen.Instance.GetCityName(i),
                cities[i].king,
                cities[i].population,
                cities[i].money,
                cities[i].reservistMax,
                cities[i].reservist,
                cities[i].defense);
            sw.WriteLine(str);
        }

        sw.Flush();
        sw.Close();
        */
    }
}
