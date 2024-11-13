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
    public string SkillName, SkillParent, description;
    public int price;
    public float xOffset;
    public Sprite SkillIcon;
}

public class SkillTree : MonoBehaviour, ISavable
{
    public SkillAffect skillAffect;
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
    public Dictionary<string, string> skillSulMyong = new();
    int maxDeepth = 0;
    const float width = 200, height = 300;
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
                skillSulMyong.Add(item.SkillName, item.description);
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
                skillTree.AddSkill(item.SkillParent, item.SkillName, item.price, item.xOffset);
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



        foreach (Node item in pointer)
        {
            float s = x + item.xOffset;

            if (item == null)
            {
                Debug.LogError("노드없음");
                continue;
            }
            NewPannel(s, deepth * height, item.name, GameObject.Find(item.parentName));

            if (item.children.Count != 0)
            {
                CreatePannel(item.children, deepth + 1, s);
            }
        }
    } // 겹치는거 고쳐라


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
        sp.transform.SetParent(posControlPannel.GetChild(1), false);
        sp.transform.localPosition = new Vector2(x, y);
        sp.gameObject.name = name;
        if (skillIcon.ContainsKey(name))
            sp.GetComponent<Image>().sprite = skillIcon[name];
        if (parent != null)
        {
            GameObject line = Instantiate(linePrefab, posControlPannel.GetChild(0));
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
        else
        {
            Debug.Log(">>>>부모없음");
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
            lineRenderer.transform.parent.SetParent(posControlPannel.GetChild(0), false);


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
            (clickedNode.parentName != null && skillTree.FindNode(skillTree.StartNode, clickedNode.parentName)?.isSold == false))
        {
            return;
        }

        playerScript.coin -= clickedNode.price;
        clickedNode.isSold = true;
        skillAffect.Invoke(clickedNode.name, 0);
        //invoke로 넣자
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
    public float xOffset;
    public bool isSold;
    public string parentName;  // parentName으로 대체
    public List<Node> children;

    public Node(string name, int price, float xSet)
    {
        this.name = name;
        this.price = price;
        xOffset = xSet;
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
        StartNode = new Node(startName, 0, 0);
        StartNode.parentName = null;
    }

    public Node AddSkill(string parentName, string skillName, int price, float xSet)
    {
        var parentNode = FindNode(StartNode, parentName);
        if (parentNode != null)
        {
            var newSkillNode = new Node(skillName, price, xSet);
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