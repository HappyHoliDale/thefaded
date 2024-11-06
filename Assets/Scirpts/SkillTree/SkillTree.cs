using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Skill
{
    public string SkillName, SkillParent;
    public int price;
    public Sprite SkillIcon;
}

public class SkillTree : MonoBehaviour
{
    public GameObject skillPannel;
    public Text coinText;
    Transform posControlPannel;
    Player playerScript;
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
    const float width = 200, height = 300;
    void Awake()
    {
        CreateSkillNode();
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
        posControlPannel = transform.GetChild(0);
        firstPannelPos = (Vector2)posControlPannel.transform.position;
        pointer = skillTree.StartNode.children;
        // CreateSkillTree();
        NewPannel(0, 0, FirstSkillName);
        CreatePannel(pointer, 1);
        AddLimCheck(skillTree.StartNode.children);
        AddLimit("vertical", height * maxDeepth);
    }
    void CreateSkillNode()
    {
        foreach (Skill item in skills)
        {
            skillIcon.Add(item.SkillName, item.SkillIcon);
            if (item.SkillParent != "Null")
                skillTree.AddSkill(item.SkillParent, item.SkillName, item.price);
            else
            {
                skillTree = new Tree(item.SkillName); // 첫스킬
            }
        }
    }
    void Update()
    {
        MouseBtn();
        PannelMove();
        EconomicSystem();
    }
    void EconomicSystem()
    {
        coinText.text = "﷼ " + playerScript.coin.ToString();
    }
    // 패널 만드는부분임
    void CreatePannel(List<Node> pointer, int deepth, float x = 0)
    {
        int dir = 1;
        float s = 0;
        int first = 0;
        float isodd = (pointer.Count % 2 == 1) ? 1.5f : 1;
        // Debug.Log(isodd);
        foreach (Node item in pointer)
        {
            int under = CheckUnder(item.children);
            s = (float)((pointer.Count > 1) ? (first == 0 && isodd != 1) ? x : ((dir % 2) == 0 ? (dir - 1) * width * ((isodd != 1 && first < 3) ? 1.5 : 1) + x : dir * -width * ((isodd != 1 && first < 3) ? 1.5 : 1) + x) : x);
            NewPannel(s, deepth * height, item.name, GameObject.Find(item.parent.name));
            dir++;
            if (under > 1) dir += (under % 2 == 0) ? under : under / 2 * 2;
            // else s--;
            if (item.children.Count != 0)
            {
                // s = item.children.Count;
                CreatePannel(item.children, deepth + 1, s);
            }
            if (first == 0 && isodd != 1)
            {
                dir--;
            }
            first++;
        }
    } // 겹치는거 고치면 끝

    int CheckUnder(List<Node> pointer)
    {
        int under = 0;
        if (pointer.Count == 0) return 1;
        else
        {
            foreach (Node item in pointer)
            {
                under += CheckUnder(item.children);
            }
            return under;
        }
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
            // 선을 그리기 위해 LineRenderer 추가
            LineRenderer line = sp.AddComponent<LineRenderer>();
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.positionCount = 2;
            line.material = new Material(Shader.Find("Sprites/Default")); // 기본 소재 설정
            line.startColor = Color.white;
            line.endColor = Color.white;

            // 부모와 자식 패널 위치를 연결
            Vector3 parentPos = parent.transform.position;
            Vector3 childPos = sp.transform.position;
            line.SetPosition(0, parentPos);
            line.SetPosition(1, childPos);
        }
    }

    // 다음 스킬크리가 이전 스킬트리 해제해야 가능하게 하는거 
    //addeventlistener를 각각 컴포넌트 순회하면서 화살표함수로 꽂아주면 될듯
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
        Node clickedNode = skillTree.FindNode(skillTree.StartNode, skill.name);
        if (clickedNode == null) return;
        if (clickedNode.parent != null && clickedNode.parent.isSold != true) return;
        if (clickedNode.isSold == true) return;
        if (clickedNode.price > playerScript.coin) return;
        playerScript.coin -= clickedNode.price;
        clickedNode.isSold = true;
        skill.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        // 여따 스킬 능력 추가도 넣을듯
    }
}


public class Node
{
    public string name;
    public int price;
    public bool isSold;
    public Node parent;
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
        child.parent = this;
    }
}
public class Tree
{
    public Node StartNode;

    public Tree(string startName)
    {
        StartNode = new Node(startName, 0);
        StartNode.parent = null;
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
