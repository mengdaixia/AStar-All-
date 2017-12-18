using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarInAll : MonoBehaviour {
    public RandomMap Rm;
    public List<NodeOfGrid> Op = new List<NodeOfGrid>();
    public Dictionary<NodeOfGrid, GameObject> PathObj = new Dictionary<NodeOfGrid, GameObject>();
    //public Dictionary<NodeOfGrid, GameObject> LastObj = new Dictionary<NodeOfGrid, GameObject>();
    public HashSet<NodeOfGrid> Close = new HashSet<NodeOfGrid>();
    NodeOfGrid EndNode;
    NodeOfGrid StartNode;
    public GameObject PathPrefab;
    public Material NowPathMat;
    public Material LastPathMat;
    public Material PathMat;
    void Start()
    {
        StartNode = Rm.GetNearestNode(Rm.StartPos.position);
        EndNode = Rm.GetNearestNode(Rm.StartPos.position);
        Op.Add(StartNode);
    }
    public void FindPathInAll()
    {
        if (Op.Contains(EndNode))
        {
            GeneratePath(GetPath(StartNode, EndNode));
        }
        for (int i = 0,max=Op.Count; i < max;)
        {
            foreach (NodeOfGrid node in Rm.GetAroundNOG(Op[i].x, Op[i].y))
            {
                if (node.IsWall)
                {
                    continue;
                }
                if (Close.Contains(node))
                {
                    continue;
                }
                int NewCost = Op[i].StarCost + GetStartCost(Op[i], node);
                if (NewCost<node.StarCost||!Op.Contains(node))
                {
                    node.StarCost = NewCost;
                    node.parent = Op[i];
                    if (!Op.Contains(node))
                    {
                        Op.Add(node);
                        GameObject o = Instantiate(PathPrefab);
                        o.transform.position = node.pos;
                        o.GetComponent<MeshRenderer>().material = NowPathMat;
                        PathObj.Add(node, o);
                    }
                }
            }
            if (PathObj.ContainsKey(Op[i]))
            {
                PathObj[Op[i]].GetComponent<MeshRenderer>().material = LastPathMat;
            }
            Close.Add(Op[i]);
            Op.Remove(Op[i]);
            --max;
        }
            
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitinfo;
            if (Physics.Raycast(ray, out hitinfo, 1000, 1 << 9))
            {
                NodeOfGrid node = Rm.GetNearestNode(hitinfo.point);
                if (Rm.TargetPos == null)
                {
                    GameObject o = Instantiate(Rm.TargetPrefab);
                    o.transform.position = node.pos;
                    Rm.TargetPos = o.transform;
                }
                else
                {
                    Rm.TargetPos.position = node.pos;
                }
                StartNode = Rm.GetNearestNode(Rm.StartPos.position);
                EndNode = Rm.GetNearestNode(Rm.TargetPos.position);
            }
        }
    }
    public List<NodeOfGrid> GetPath(NodeOfGrid start,NodeOfGrid end)
    {
        List<NodeOfGrid> temp = new List<NodeOfGrid>();
        NodeOfGrid Nog = end;
        while (Nog!=null)
        {
            temp.Add(Nog);
            Debug.Log(Nog.StarCost);
            Nog = Nog.parent;
        }
        //temp.Add(Nog);
        temp.Reverse();
        return temp;
    }
    public void GeneratePath(List<NodeOfGrid> nodelist)
    {
        foreach (NodeOfGrid node in nodelist)
        {
            if (!PathObj.ContainsKey(node))
            {
                continue;
            }
            PathObj[node].GetComponent<MeshRenderer>().material = PathMat;
        }
    }
    int GetStartCost(NodeOfGrid a,NodeOfGrid b)
    {
        if (a.x == b.x || a.y == b.y)
        {
            return 10;
        }
        else {
            return 14;
        }
    }
}
