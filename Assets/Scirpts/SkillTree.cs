using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// [Serializable] 
// public class Skill
// {
//     public string SkillName, SkillParent;
// }

public class SkillTree : MonoBehaviour
{
    public GameObject skillPannel;
    const int addLim = 100;
    Transform posControlPannel;
    Player_Move playerScript;
    Vector2 mouseDragPoint, pannelDragPoint, firstPannelPos;
    float top = 400, left = 800, right = 800, bottom = 400;
    bool onMouseDown;
    Tree skillTree = new Tree("Dash"); // 첫스킬
    List<Node> pointer;
    // public Skill FirstSkill;
    // public Skill[] skills;

    void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player_Move>();
        posControlPannel = transform.GetChild(0);
        firstPannelPos = (Vector2)posControlPannel.transform.position;
        NewPannel(0, 0, "Dash");
        pointer = skillTree.StartNode.children;
        CreateSkillTree();
        CreatePannel(pointer, 1);
    }
    void Update()
    {
        MouseBtn();
        PannelMove();
    }
    // 스킬트리 생성부분임
    void CreateSkillTree()
    {
        // foreach (Skill item in skills)
        // {
        //     skillTree.AddSkill(item.SkillName, item.SkillParent);
        //     Debug.Log(item.SkillName + "...." + item.SkillParent);
        // }
        skillTree.AddSkill("Dash", "Attack");
        skillTree.AddSkill("Dash", "Paring");
        skillTree.AddSkill("Dash", "Hi");
        skillTree.AddSkill("Dash", "Zi");
        skillTree.AddSkill("Attack", "Nice");
        skillTree.AddSkill("Attack", "It");
        skillTree.AddSkill("Attack", "Its");
        skillTree.AddSkill("Attack", "Itss");
        skillTree.AddSkill("Attack", "Itsss");
        // skillTree.AddSkill("Attack", "ReA");
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
            s = (float)((pointer.Count > 1) ? (first == 0 && isodd != 1) ? x : ((dir % 2) == 0 ? (dir - 1) * 100 * ((isodd != 1 && first < 3) ? 1.5 : 1) + x : dir * -100 * ((isodd != 1 && first < 3) ? 1.5 : 1) + x) : x);
            NewPannel(s, deepth * 300, item.name);
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

    void NewPannel(float x, float y, string name, string dir = null, GameObject parent = null)
    {
        GameObject sp = Instantiate(skillPannel);
        sp.transform.SetParent(posControlPannel, false);
        sp.transform.localPosition = new Vector2(x, y);
        sp.gameObject.name = name;
        if (dir != null)
        {
            AddLimit(dir, addLim);
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
            posControlPannel.position = pannelDragPoint + mouseMoved * 0.005f;
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
    void AddLimit(string dir, int add)
    {
        if (dir == "top") top += add;
        else if (dir == "left") left += add;
        else if (dir == "right") right += add;
        else if (dir == "bottom") bottom += add;
    }
}


class Node
{
    public string name;
    public Node parent;
    public List<Node> children;
    public Node(string name)
    {
        this.name = name;
        children = new List<Node>();
    }
    public void AddChild(Node child)
    {
        children.Add(child);
        child.parent = this;
    }
}
class Tree
{
    public Node StartNode;

    public Tree(string startName)
    {
        StartNode = new Node(startName);
    }

    public Node AddSkill(string parentName, string skillName)
    {
        var parentNode = FindNode(StartNode, parentName);
        if (parentNode != null)
        {
            var newSkillNode = new Node(skillName);
            parentNode.AddChild(newSkillNode);
            return newSkillNode;
        }
        return null;
    }

    private Node FindNode(Node currentNode, string nodeName)
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
