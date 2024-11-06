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
    void Start()
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
    public Tree CreateSkillNode()
    {
        foreach (Skill item in skills)
        {
            skillIcon.Add(item.SkillName, item.SkillIcon);
            if (item.SkillParent != "Null")
                skillTree.AddSkill(item.SkillParent, item.SkillName, item.price);
            else
            {
                skillTree = new Tree(FirstSkillName); // 첫스킬
            }
        }
        return skillTree;
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

            RectTransform parentRect = parent.GetComponent<RectTransform>();
            RectTransform spRect = sp.GetComponent<RectTransform>();

            Vector3 startPos = parent.transform.position;
            Vector3 endPos = sp.transform.position;

            Vector3 direction = (endPos - startPos).normalized;
            float parentOffset = Mathf.Min(parentRect.rect.width, parentRect.rect.height) / 2;
            float spOffset = Mathf.Min(spRect.rect.width, spRect.rect.height) / 2;

            startPos += direction * parentOffset;
            endPos -= direction * spOffset;

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

            // LineRenderer를 위치 업데이트하도록 연결
            StartCoroutine(UpdateLine(lineRenderer, parent.transform, sp.transform, parentRect, spRect));

        }
    }
    public float lineOffsetControl = 0.01f;
    IEnumerator UpdateLine(LineRenderer lineRenderer, Transform start, Transform end, RectTransform startRect, RectTransform endRect)
    {
        while (true)
        {
            Vector3 startPos = start.position;
            Vector3 endPos = end.position;

            Vector3 direction = (endPos - startPos).normalized;
            float lineOffset = Vector3.Distance(endPos, startPos) * lineOffsetControl;
            float startOffset = Mathf.Min(startRect.rect.width, startRect.rect.height) * 0.01f * lineOffset;
            float endOffset = Mathf.Min(endRect.rect.width, endRect.rect.height) * 0.01f * lineOffset;

            startPos += direction * startOffset;
            endPos -= direction * endOffset;

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

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
        Debug.Log(skillTree.StartNode + ">>");
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
