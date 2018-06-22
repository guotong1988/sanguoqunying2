using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MyPathfinding : MonoBehaviour {
	
	public int maxDistance = 30;
	public Transform[] cityPoints;
	
	private static bool isInit = false;
	private static List<int> cityPointsIdx = new List<int>();
	
	private static List<List<int>> connectList = new List<List<int>>();
	//private List<List<int>> nearbyCities = new List<List<int>>();
	
	private static List<List<int>> nearbyCities = new List<List<int>> {
		new List<int>{1},
		new List<int>{0, 2, 4},
		new List<int>{1, 4, 3},
		new List<int>{2, 10, 6},
		new List<int>{5, 7, 2, 1},
		new List<int>{4, 7, 6, 8},
		new List<int>{9, 18, 3, 10, 5, 8},
		new List<int>{5, 4, 16, 17},
		new List<int>{6, 5, 9, 17, 19},
		new List<int>{8, 17, 19, 6, 18},
		new List<int>{3, 6, 18, 21, 11},
		new List<int>{22, 10, 12},
		new List<int>{11, 13, 37},
		new List<int>{12, 14},
		new List<int>{13, 15, 38},
		new List<int>{14},
		new List<int>{7, 17, 32},
		new List<int>{8, 9, 19, 7, 16, 31},
		new List<int>{6, 9, 19, 10},
		new List<int>{20, 8, 9, 17, 18},
		new List<int>{31, 19, 21},
		new List<int>{10, 20, 22},
		new List<int>{23, 24, 21, 11},
		new List<int>{25, 26, 22, 24},
		new List<int>{23, 22, 37, 41},
		new List<int>{35, 23, 26, 28},
		new List<int>{41, 27, 25, 23},
		new List<int>{41, 26, 28, 30},
		new List<int>{27, 30, 25, 29, 36},
		new List<int>{30, 28},
		new List<int>{27, 28, 29},
		new List<int>{32, 35, 17, 20},
		new List<int>{16, 33, 31, 35},
		new List<int>{32, 34},
		new List<int>{33, 47, 46},
		new List<int>{32, 31, 25, 46},
		new List<int>{28, 45, 46},
		new List<int>{38, 12, 24},
		new List<int>{14, 37, 39},
		new List<int>{38, 40},
		new List<int>{39, 43, 42},
		new List<int>{42, 24, 27, 26},
		new List<int>{40, 41},
		new List<int>{40, 44},
		new List<int>{43},
		new List<int>{36},
		new List<int>{35, 34, 36},
		new List<int>{34}
	};
	
	// Use this for initialization
	void Awake () {
		
		if (!isInit) {
			isInit = true;
			InitConnectList();
			InitNearbyCities();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void InitConnectList() {
		
		int count = transform.childCount;
		
		for (int i=0; i<count; i++) {
			
			List<int> nodeConnectList = new List<int>();
			
			for (int j=0; j<count ;j++) {
				
				if (i == j) continue;
				
				bool breakFlag = false;
				Transform child_i = transform.GetChild(i);
				Transform child_j = transform.GetChild(j);
				
				LinkBreak linkBreak_i = child_i.GetComponent<LinkBreak>();
				if (linkBreak_i != null) {
					
					for (int k=0; k<linkBreak_i.node.Length; k++) {
						
						if (linkBreak_i.node[k] == child_j) {
							
							breakFlag = true;
							break;
						}
					}
					
					if (breakFlag) continue;
				} 
				
					
				LinkBreak linkBreak_j = child_j.GetComponent<LinkBreak>();
				if (linkBreak_j != null) {
					
					for (int k=0; k<linkBreak_j.node.Length; k++) {
						
						if (linkBreak_j.node[k] == child_i) {
							
							breakFlag = true;
							break;
						}
					}
					
					if (breakFlag) continue;
				} 
				
				float distance = Vector3.Distance(child_i.position, child_j.position);
				if (distance <= maxDistance) {
					
					nodeConnectList.Add(j);
				}
			}
			
			connectList.Add(nodeConnectList);
		}
	}
	
	private void InitNearbyCities() {
		
		int count = cityPoints.Length;
		
		for (int i=0; i<count; i++) {
			
			cityPointsIdx.Add(GetPointIdx(i));
		}
		
		/*
		for (int i=0; i<count; i++) {
			
			List<int> nbc = new List<int>();
			
			int[] flag = new int[transform.childCount];
			List<int> connect = connectList[cityPointsIdx[i]];
			
			flag[cityPointsIdx[i]] = 1;
			for (int n=0; n<connect.Count; n++) {
				FindNearbyCitiesX(connect[n], nbc, flag);
			}
			
			nearbyCities.Add(nbc);
		}
		
		Debug.Log(Application.dataPath);
		FileStream fs = new FileStream(Application.dataPath+"/1.txt", FileMode.Create, FileAccess.Write);
		StreamWriter sw = new StreamWriter(fs);
		sw.Flush();
		
		for (int i=0; i<count; i++) {
			string str = "new List<int>{";
			for (int j=0; j<nearbyCities[i].Count; j++) {
				str += nearbyCities[i][j];
				if (j != nearbyCities[i].Count-1)
					str += ", ";
			}
			str += "},";
			Debug.Log(str);
			sw.WriteLine(str);
		}
		
		sw.Flush();
		sw.Close();
		*/
	}
	
	private void FindNearbyCitiesX(int idx, List<int> nbc, int[] flag) {
		
		if (flag[idx] == 1) return;
		flag[idx] = 1;
		
		int cityIdx = -1;
		for (int i=0; i<cityPointsIdx.Count; i++) {
			if (cityPointsIdx[i] == idx) {
				
				cityIdx = i;
				break;
			}
		}
		
		if (cityIdx != -1) {
			
			nbc.Add(cityIdx);
			return;
		}
		
		List<int> connect = connectList[idx];
		for (int n=0; n<connect.Count; n++) {
			
			FindNearbyCitiesX(connect[n], nbc, flag);
		}
	}
	
	public static List<int> GetCityNearbyIdx(int cIdx) {
		
		if (cIdx < 0 || cIdx >= Informations.Instance.cityNum) return null;
		
		return nearbyCities[cIdx];
	}
	
	private int GetPointIdx(Vector3 pos) {
		
		float minDistance = float.MaxValue;
		int closest = -1;
		int count = transform.childCount;
		
		for (int i=0; i<count; i++) {
			
			Transform child = transform.GetChild(i);
			
			float distance = Vector3.Distance(pos, child.position);
			
			if (distance < 1) return i;
			if (distance < minDistance) {
				
				minDistance = distance;
				closest = i;
			}
		}
		
		return closest;
	}
	
	private int GetPointIdx(Transform node) {
		
		int count = transform.childCount;
		
		for (int i=0; i<count; i++) {
			
			if (transform.GetChild(i) == node) {
				
				return i;
			}
		}
		
		return -1;
	}
	
	private int GetPointIdx(int city) {
		
		return GetPointIdx(cityPoints[city]);
	}
	
	public int GetCityIndex(Vector3 pos, int distance) {
		
		int ret = -1;
		
		for (int i=0; i<cityPoints.Length; i++) {
			
			if (Vector3.Distance(pos, cityPoints[i].position) < distance) {
				ret = i;
				break;
			}
		}
		
		return ret;
	}
	
	public Vector3 GetCityPos(int city) {
		
		if (city < 0 || city >= Informations.Instance.cityNum) return Vector3.zero;
		
		return cityPoints[city].position;
	}
	
	public List<Vector3> GetRoute(Vector3 startPos, Vector3 endPos) {
		
		return GetRoutePoints(GetPointIdx(startPos), GetPointIdx(endPos));
	}
	
	public List<Vector3> GetRoute(Vector3 startPos, int endCity) {
		return GetRoutePoints(GetPointIdx(startPos), GetPointIdx(endCity));
	}
	
	public List<Vector3> GetRoute(int startCity, int endCity) {
		
		return GetRoutePoints(GetPointIdx(startCity), GetPointIdx(endCity));
	}
	
	public List<Vector3> GetRoute(Transform start, Transform end) {
		
		return GetRoutePoints(GetPointIdx(start), GetPointIdx(end));
	}
	
	private List<Vector3> GetRoutePoints(int start, int end) {
		
		List<Vector3> list = new List<Vector3>();
		
		if (!isInit) {
			
			InitConnectList();
		}
		
		if (start == end) {
			
			list.Add(transform.GetChild(end).position);
			return list;
		}
		
		bool isFinished = false;
		int[] flag = new int[transform.childCount];
		List<int> nodes = new List<int>();
		int index = 1;
		
		flag[end] = index;
		nodes.AddRange(connectList[end]);
		
		while (!isFinished) {
			
			index++;
			
			int count = nodes.Count;
			for (int i=count-1; i>=0; i--) {
				
				int checking = nodes[i];
				
				if (checking == start) {
					
					isFinished = true;
					nodes.Clear();
					break;
				}
				
				if (flag[checking] == 0) {
					flag[checking] = index;
					nodes.AddRange(connectList[checking]);
				}
				
				nodes.RemoveAt(i);
			}
		}
		
		nodes = connectList[start];
		
		while (index > 1) {
			
			index--;
			
			for (int i=0; i<nodes.Count; i++) {
				
				int checking = nodes[i];
				if (flag[checking] == index) {
					
					nodes = connectList[checking];
					list.Add(transform.GetChild(checking).position);
					
					break;
				}
			}
		}
		
		return list;
	}
	
	
	
	
	void OnDrawGizmos () {
		
		if (!isInit) return;
		
		Gizmos.color = new Color (1,0,0,1F);
			
		for (int i=0; i<connectList.Count; i++) {
			
			List<int> connect = connectList[i];
			for (int j=0; j<connect.Count; j++) {
				
				Gizmos.DrawLine (transform.GetChild(i).position, transform.GetChild(connect[j]).position);
			}
		}
	}
	
}
