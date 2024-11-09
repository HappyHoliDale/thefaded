using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Skill
{
    public string SkillName, SkillParent;
    public int price;
    public Sprite SkillIcon;
}

public class SkillTree : MonoBehaviour, ISavable
{
    public GameObject skillPannel;
    public Text coinText;
    public bool isSkillTreeLoaded = false;
    public Transform posControlPannel;
    public Player playerScript;
    Vector2 mouseDragPoint, pannelDragPoint, firstPannelPos;
    float top = 400, left = 800, right = 800, bottom = 400;
    bool onMouseDown;
    public Tree skillTree;
    public string FirstSkillName;
    List<Node> pointer;
    // public Skill FirstSkill;
    public Skill[] skills;
    Dictionary<string, Sprite> skillIcon = new();
    int maxDeepth = 0;
    const float width = 200, height = 600;
    void Start()
    {
        firstPannelPos = (Vector2)posControlPannel.transform.position;
        // CreateSkillTree();
        NewPannel(0, 0, FirstSkillName);
        AddLimCheck(skillTree.StartNode.children);
        AddLimit("vertical", height * maxDeepth);
        CreatePannel(pointer, 1);
        CheckPnlisSold();
    }
    public void LoadData(Database data)
    {
        Debug.Log(data.playerData.st);
        if (data.playerData.st == null)
        {
            Debug.Log("data load failled");
            CreateSkillNode();
            pointer = skillTree.StartNode.children;
        }
        else
        {
            Debug.Log("data loaded");
            skillTree = data.playerData.st;
            foreach (Skill item in skills)
            {
                skillIcon.Add(item.SkillName, item.SkillIcon);
            }
            pointer = skillTree.StartNode.children;
        }
    }
    public void SaveData(ref Database data)
    {
        data.playerData.st = skillTree;
        Debug.Log("data saved");
        Debug.Log(data.playerData.st);
    }
    public Tree CreateSkillNode()
    {
        skillTree = new Tree(FirstSkillName); // 첫스킬

        foreach (Skill item in skills)
        {
            skillIcon.Add(item.SkillName, item.SkillIcon);
            if (item.SkillParent != "Null")
                skillTree.AddSkill(item.SkillParent, item.SkillName, item.price);
        }
        return skillTree;
    }
    void Update()
    {
        MouseBtn();
        PannelMove();
        EconomicSystem();
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log(skillTree.StartNode.name);
            Debug.Log(skillTree.StartNode.isSold);
            Debug.Log(skillTree.StartNode.children.Count);

        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            CreatePannel(pointer, 1);

        }
    }
    void EconomicSystem()
    {
        coinText.text = "﷼ " + playerScript.coin.ToString();
    }
    // 패널 만드는부분임
    void CreatePannel(List<Node> pointer, int deepth, float x = 0)
    {
        if (pointer == null)
        {
            Debug.LogError("pointer가 null입니다.");
            return;
        }

        int dir = 1;
        float s = 0;
        int first = 0;
        float isodd = (pointer.Count % 2 == 1) ? 1.5f : 1;

        bool isOdd = (pointer.Count % 2 == 1) ? true : false;
        bool isFirst = true;
        bool isRight = true;
        int num = 1;
        float lPlus = 0;
        float rPlus = 0;
        foreach (Node item in pointer)
        {
            if (item == null)
            {
                Debug.LogError("노드없음");
                continue;
            }
            Vector2 under = CheckUnder(item.children);
            if (pointer.Count < 2 || isFirst && isOdd)
            {
                s = x;
            }
            else
            {
                if (isOdd)
                {
                    if (num < 4)
                    {
                        if (isRight)
                        {
                            s = width * 2 + x + rPlus;
                        }
                        else
                        {
                            s = width * -2 + x + lPlus;
                        }
                    }
                    else
                    {
                        if (isRight)
                        {
                            s = width * (num - 1) + x + rPlus;
                        }
                        else
                        {
                            s = width * -num + x + lPlus;
                        }
                    }
                }
                else
                {
                    if (isRight)
                    {
                        s = width * num + x + rPlus;
                    }
                    else
                    {
                        s = width * -(num - 1) + x + lPlus;
                    }
                }
            }
            num++;
            if (!(under.x < 2))//isFirst || 
            {
                float p = under.x * width * 0.6f;
                if (isRight)
                {
                    rPlus += p * 2 * under.y;
                    s += p;

                }
                else
                {
                    s += -p;
                    lPlus += -p * 2 * under.y;
                }
            }
            isRight = isRight ? false : true;
            isFirst = isFirst ? false : false;

            Debug.Log("||>>>>>>>>>>>>>" + item.name);
            NewPannel(s, deepth * height, item.name, GameObject.Find(item.parentName));

            if (item.children.Count != 0)
            {
                CreatePannel(item.children, deepth + 1, s);
            }
            if (first == 0 && isodd != 1)
            {
                dir--;
            }
            first++;
        }
    } // 겹치는거 고쳐라


    Vector2 CheckUnder(List<Node> pointer, int deepth = 0)
    {
        int under = 0;
        int dth = 0;
        int cdth = 0;
        if (pointer.Count == 0) return new Vector2(1, deepth);
        else
        {
            foreach (Node item in pointer)
            {
                under += (int)CheckUnder(item.children, deepth + 1).x;
                cdth = (int)CheckUnder(item.children, deepth + 1).y;
                if (dth < cdth)
                    dth = cdth;
            }
        }
        return new Vector2(under, dth);
    }

    void AddLimCheck(List<Node> pointer, int deepth = 0)
    {
        if (deepth > maxDeepth) maxDeepth = deepth;
        if (pointer.Count == 0)
        {
            AddLimit("horizontal", width);
        }
        else
        {
            foreach (Node item in pointer)
            {
                AddLimCheck(item.children, deepth + 1);
            }
        }
    }
    public GameObject linePrefab;
    void NewPannel(float x, float y, string name, GameObject parent = null)
    {
        GameObject sp = Instantiate(skillPannel);
        sp.transform.SetParent(posControlPannel, false);
        sp.transform.localPosition = new Vector2(x, y);
        sp.gameObject.name = name;
        if (skillIcon.ContainsKey(name))
            sp.GetComponent<Image>().sprite = skillIcon[name];
        if (parent != null)
        {
            GameObject line = Instantiate(linePrefab, posControlPannel);
            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;

            Vector3 startPos = parent.transform.position + new Vector3(0, 50);
            Vector3 endPos = sp.transform.position + new Vector3(0, -50);

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
            // LineRenderer를 위치 업데이트하도록 연결
            StartCoroutine(UpdateLine(lineRenderer, parent.transform, sp.transform));

        }
        else
        {
            Debug.Log(">>>>부모없음");
        }
    }
    public float lineOffsetControl = 0.01f;
    IEnumerator UpdateLine(LineRenderer lineRenderer, Transform start, Transform end)
    {
        while (true)
        {
            Vector3 startPos = start.position;
            Vector3 endPos = end.position;

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
            lineRenderer.transform.parent.SetParent(posControlPannel, false);


            yield return null;
        }
    }
    void PannelMove()
    {
        if (onMouseDown)
        {
            Vector2 mousePos = Camera.main.WorldToScreenPoint(Input.mousePosition);
            Vector2 mouseMoved = mousePos - mouseDragPoint;
            posControlPannel.position = pannelDragPoint + mouseMoved * 0.0001f;
        }
    }
    void MouseBtn()
    {
        if (Input.GetMouseButtonDown(1) && onMouseDown == false)
        {
            onMouseDown = true;
            mouseDragPoint = Camera.main.WorldToScreenPoint(Input.mousePosition);
            pannelDragPoint = (Vector2)posControlPannel.position;
        }
        else if (Input.GetMouseButtonUp(1) && onMouseDown == true)
        {
            onMouseDown = false;
            try
            {
                if (posControlPannel.position.x > firstPannelPos.x + right || posControlPannel.position.x < firstPannelPos.x - left)
                {
                    posControlPannel.position = new Vector2(pannelDragPoint.x, posControlPannel.position.y);
                }
                if (posControlPannel.position.y < firstPannelPos.y - bottom || posControlPannel.position.y > firstPannelPos.y + top)
                {
                    posControlPannel.position = new Vector2(posControlPannel.position.x, pannelDragPoint.y);
                }
            }
            catch
            {
                Debug.Log("error");
            }
        }
    }
    void AddLimit(string dir, float add)
    {
        if (dir == "horizontal")
        {
            left += add;
            right += add;
        }
        else if (dir == "vertical")
        {
            // top += add;
            bottom += add;
        }
    }
    public void PnlClicked(GameObject skill)
    {
        Debug.Log(skill.name);

        // skill.name으로 노드를 찾고, 찾은 노드가 null인지 확인
        Node clickedNode = skillTree.FindNode(skillTree.StartNode, skill.name);
        Debug.Log(clickedNode.isSold);
        // 클릭된 노드가 null일 경우, 또는 노드가 판매되지 않은 상태, 가격이 부족할 경우, 부모 노드가 판매되지 않은 경우 처리
        if (clickedNode == null ||
            clickedNode.isSold == true ||
            clickedNode.price > playerScript.coin ||
            (clickedNode.parentName != null && skillTree.FindNode(clickedNode, clickedNode.parentName)?.isSold == false))
        {
            return;
        }

        playerScript.coin -= clickedNode.price;
        clickedNode.isSold = true;
        skill.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        // 스킬 능력 추가 코드 여따 넣을거임
    }
    public void CheckPnlisSold()
    {
        foreach (Skill skill in skills)
        {
            Node node_ = skillTree.FindNode(skillTree.StartNode, skill.SkillName);
            if (node_.isSold == true)
            {
                GameObject.Find(node_.name).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
    }
}


public class Node
{
    public string name;
    public int price;
    public bool isSold;
    public string parentName;  // parentName으로 대체
    public List<Node> children;

    public Node(string name, int price)
    {
        this.name = name;
        this.price = price;
        isSold = false;
        children = new List<Node>();
    }

    public void AddChild(Node child)
    {
        children.Add(child);
        child.parentName = this.name;  // parentName을 설정
    }
}

public class Tree
{
    public Node StartNode;

    public Tree(string startName)
    {
        StartNode = new Node(startName, 0);
        StartNode.parentName = null;
    }

    public Node AddSkill(string parentName, string skillName, int price)
    {
        var parentNode = FindNode(StartNode, parentName);
        if (parentNode != null)
        {
            var newSkillNode = new Node(skillName, price);
            parentNode.AddChild(newSkillNode);
            return newSkillNode;
        }
        return null;
    }

    public Node FindNode(Node currentNode, string nodeName)
    {
        if (currentNode.name == nodeName)
            return currentNode;

        foreach (var child in currentNode.children)
        {
            var foundNode = FindNode(child, nodeName);
            if (foundNode != null)
                return foundNode;
        }

        return null;
    }
}
