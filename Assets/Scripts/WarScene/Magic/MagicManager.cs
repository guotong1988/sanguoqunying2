using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MagicManager {

	private string xmlName = "MagicConfig";
	private Dictionary<int, MagicDataInfo> dataList = new Dictionary<int, MagicDataInfo>();

	private static MagicManager mInstance;
	public static MagicManager Instance {
		get {
			if (mInstance == null) {
				mInstance = new MagicManager();
			}
			return mInstance;
		}
	}

	public void LoadConfig() {
		if (dataList.Count > 0) return;

		XmlDocument xmlDoc = new XmlDocument();
		TextAsset textAsset = (TextAsset)Resources.Load(xmlName);
		xmlDoc.LoadXml(textAsset.text.ToString().Trim());
		XmlElement root = xmlDoc.DocumentElement;
		XmlNodeList nodes = root.ChildNodes;
		foreach (XmlElement node in nodes) {
			MagicDataInfo info = new MagicDataInfo();
			info.SEQUENCE = int.Parse(node.GetAttribute("SEQUENCE"));
			info.NAME = node.GetAttribute("NAME");
			info.MP = int.Parse(node.GetAttribute("MP"));
			info.POWER = int.Parse(node.GetAttribute("POWER"));
			info.ATTACK = int.Parse(node.GetAttribute("ATTACK"));
			info.SCRIPT = node.GetAttribute("SCRIPT");
			info.ATTRIB = node.GetAttribute("ATTRIB");
			info.TITLE = node.GetAttribute("TITLE");
			info.NOTE = node.GetAttribute("NOTE");
			info.ACTIVE = node.GetAttribute("ACTIVE");
			info.TYPE = int.Parse(node.GetAttribute("TYPE"));
			info.TIME = float.Parse(node.GetAttribute("TIME"));

			dataList.Add(info.SEQUENCE, info);
		}
	}

	public MagicDataInfo GetMagicDataInfo(int index) {
		if (dataList.ContainsKey(index)) {
			return dataList[index];
		}

		return null;
	}
}

public class MagicDataInfo {

	private int no;
	public int SEQUENCE {
		get { return no; }
		set { no = value; }
	}

	private string name;
	public string NAME {
		get { return name; }
		set { name = value; }
	}

	private int mp;
	public int MP {
		get { return mp; }
		set { mp = value; }
	}

	private int power;
	public int POWER {
		get { return power; }
		set { power = value; }
	}

	private int attack;
	public int ATTACK {
		get { return attack; }
		set { attack = value; }
	}

	private string script;
	public string SCRIPT {
		get { return script; }
		set { script = value; }
	}

	private string attrib;
	public string ATTRIB {
		get { return attrib; }
		set { attrib = value; }
	}

	private string title;
	public string TITLE {
		get { return title; }
		set { title = value; }
	}

	private string note;
	public string NOTE {
		get { return note; }
		set { note = value; }
	}

	private string active;
	public string ACTIVE {
		get { return active; }
		set { active = value; }
	}

	private int type;
	public int TYPE {
		get { return type; }
		set { type = value; }
	}

	private float time;
	public float TIME {
		get { return time; }
		set { time = value; }
	}
}