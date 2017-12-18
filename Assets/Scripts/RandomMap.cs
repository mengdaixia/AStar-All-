using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NodeOfGrid {
    public bool IsWall;
    public int x;
    public int y;
    public Vector3 pos;
    public NodeOfGrid parent;
    public int StarCost;
    public NodeOfGrid(bool iswall,int x,int y)
    {
        this.IsWall = iswall;
        this.x = x;
        this.y = y;
    }
}
public class RandomMap : MonoBehaviour {
    public GameObject Wall;
    public int MapSizeX;
    public int MapSizeY;
    public Material NormalMat;
    public Material WallMat;
    public GameObject TargetPrefab;
    public Transform TargetPos;
    public Transform StartPos;
    public NodeOfGrid[,] GridInfo;
    public Transform[,] WallInfo;
    public Dictionary<Vector2, int> MapInfo = new Dictionary<Vector2, int>();
	void Awake () {
        GridInfo = new NodeOfGrid[MapSizeX, MapSizeY];
        WallInfo = new Transform[MapSizeX, MapSizeY];
        MapInit();
	}
    void MapInit()
    {
        for (int i = 0; i < MapSizeX; i++)
        {
            for (int j = 0; j < MapSizeY; j++)
            {
                GridInfo[i, j] = new NodeOfGrid(false, i, j);
                GridInfo[i, j].pos = new Vector3(i + 0.5f, 0, j + 0.5f);
                int a = Random.Range(0, 100);
                if (a>45)
                {
                    WallInfo[i, j] = null;
                    MapInfo.Add(new Vector2(i, j), 0);
                    continue;
                }
                
                GameObject o = Instantiate(Wall);
                o.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
                o.transform.SetParent(transform);   
                WallInfo[i, j] = o.transform;
                MapInfo.Add(new Vector2(i, j), 1);
            }
        }
    }
    public void UpdateMapInfo()
    {
        Dictionary<Vector2, int> Adhoc = new Dictionary<Vector2, int>();
        for (int i = 0; i < MapSizeX; i++)
        {
            for (int j = 0; j < MapSizeY; j++)
            {
                int count = GetAroundWall(i, j);
                if (MapInfo[new Vector2(i,j)] == 1)
                {
                    if (count >= 4)
                    {
                        Adhoc.Add(new Vector2(i, j), 1);
                    }
                    else
                    {
                        Adhoc.Add(new Vector2(i, j), 0);
                    }
                }
                else {
                    if (count >= 5)
                    {
                        Adhoc.Add(new Vector2(i, j), 1);
                    }
                    else {
                        Adhoc.Add(new Vector2(i, j), 0);
                    }
                }
            }
        }
        MapToChange(Adhoc);
    }
    void MapToChange(Dictionary<Vector2,int> dic)
    {
        for (int i = 0; i < MapSizeX; i++)
        {
            for (int j = 0; j < MapSizeY; j++)
            {
                int result = dic[new Vector2(i, j)];
                if (result != MapInfo[new Vector2(i, j)]) 
                {
                    MapInfo[new Vector2(i, j)] = result;
                    if (result == 1)
                    {
                        GameObject o = Instantiate(Wall);
                        o.transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
                        WallInfo[i, j] = o.transform;
                    }
                    else {
                        Destroy(WallInfo[i, j].gameObject);
                        WallInfo[i, j] = null;
                    }
                }
            }
        }
    }
    public int GetAroundWall(int m,int n)
    {
        int count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if (m + i > MapSizeX-1 || m + i < 0 || n + j > MapSizeY-1 || n + j < 0) 
                {
                    ++count;
                    continue;
                }
                if (MapInfo[new Vector2(m + i, n + j)] == 1)
                { 
                    ++count;
                }
            }
        }
        //Debug.Log("(" + (m+1).ToString() + "," + (n+1).ToString() + ")" + count);
        return count;
    }
    public int GetFarAroundWall(int m, int n)
    {
        int count = 0;
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                if (m + i > MapSizeX - 2 || m + i < 0 || n + j > MapSizeY - 2 || n + j < 0|| m + i > MapSizeX - 1|| n + j > MapSizeY - 1)
                {
                    ++count;
                    continue;
                }
                if (MapInfo[new Vector2(m + i, n + j)] == 1)
                {
                    ++count;
                }
            }
        }
        //Debug.Log("(" + (m+1).ToString() + "," + (n+1).ToString() + ")" + count);
        return count;
    }
    public void GridInfoUpdate()
    {
        for (int i = 0; i < MapSizeX; i++)
        {
            for (int j = 0; j < MapSizeY; j++)
            {
                if (Physics.CheckSphere(new Vector3(i + 0.5f, 0, j + 0.5f), 0.45f, 1 << 8))
                {
                    GridInfo[i, j].IsWall = true;
                    WallInfo[i, j].GetComponent<MeshRenderer>().material = WallMat;
                }
                else {
                    GridInfo[i, j].IsWall = false;
                    //WallInfo[i, j].GetComponent<MeshRenderer>().material = NormalMat;
                }
            }
        }
    }
    public List<NodeOfGrid> GetAroundNOG(int m,int n)
    {
        List<NodeOfGrid> Adhoc = new List<NodeOfGrid>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i==0&&j==0)
                {
                    continue;
                }
                if (m + i < 0 || m + i > MapSizeX - 1 || n + j < 0 || n + j > MapSizeY - 1) 
                {
                    continue;
                }
                Adhoc.Add(GridInfo[m + i, n + j]);
            }
        }
        return Adhoc;
    }
    public List<NodeOfGrid> GetAroundNOGWithSelf(int m,int n)
    {
        List<NodeOfGrid> Adhoc = new List<NodeOfGrid>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (m + i < 0 || m + i > MapSizeX - 1 || n + j < 0 || n + j > MapSizeY - 1)
                {
                    continue;
                }
                Adhoc.Add(GridInfo[m + i, n + j]);
            }
        }
        return Adhoc;
    }
    public NodeOfGrid GetNearestNode(Vector3 pos)
    {
        float PersenX = (pos.x - transform.position.x) / (GridInfo[MapSizeX - 1, MapSizeY - 1].pos.x - transform.position.x);
        float PersenY = (pos.z - transform.position.z) / (GridInfo[MapSizeX - 1, MapSizeY - 1].pos.z - transform.position.z);
        int x = Mathf.RoundToInt(PersenX * 100);
        int y = Mathf.RoundToInt(PersenY * 100);
        List<NodeOfGrid> dic = GetAroundNOGWithSelf(x, y);
        float min = float.MaxValue;
        NodeOfGrid NearestNode = null;
        foreach (NodeOfGrid node in dic)
        {
            if (Vector3.Distance(pos, node.pos) < min) 
            {
                min = Vector3.Distance(pos, node.pos);
                NearestNode = node;
            }
        }
        return NearestNode;
    }
}
