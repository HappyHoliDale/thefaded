// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;

// public class SkillPannelScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
// {
//     public SkillTree skillTree;
//     public GameObject skillInfoPannel;
//     private GameObject skillInfo;
//     private string skillName;

//     void Start()
//     {
//         foreach (Skill item in skillTree.skills)
//         {
//             if (item.SkillName == gameObject.name)
//             {
//                 // 정보 가져오기
//             }
//         }
//     }

//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         skillInfo = Instantiate(skillInfoPannel, transform);
//     }

//     public void OnPointerMove(PointerEventData eventData)
//     {
//         if (skillInfo != null)
//         {
//             skillInfo.transform.position = eventData.position;
//         }
//     }

//     public void OnPointerExit(PointerEventData eventData)
//     {
//         if (skillInfo != null)
//         {
//             Destroy(skillInfo);
//         }
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UIElements;
using TMPro;

public class SkillPannelScript : MonoBehaviour
{
    SkillTree skillTree;
    public TextMeshProUGUI skillInfoPannel;
    private TextMeshProUGUI skillInfo;

    void Start()
    {
        skillTree = GameObject.Find("SkillTree").GetComponent<SkillTree>();
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => skillTree.PnlClicked(gameObject));
        // foreach (Skill item in skillTree.skills)
        // {
        //     if (item.SkillName == gameObject.name)
        //     {
        //     }
        // }
    }

    void Update()
    {
        CheckHover();
    }
    void CheckHover()
    {
        Vector3 a = Input.mousePosition;
        Vector3 translate = new Vector3(100, -100);
        // Debug.Log(a + " || " + transform.position + " >>>>" + Vector3.Distance(a, transform.position) + "||>> " + gameObject.name);

        if (Vector3.Distance(a, transform.position) < 100)
        {

            if (skillInfo == null)
                skillInfo = Instantiate(skillInfoPannel, transform);
            skillInfo.transform.position = a + translate;
            skillInfo.text = gameObject.name;
            // Debug.Log("dd");
        }
        else
        {
            if (skillInfo != null)
                Destroy(skillInfo);
        }
    }
}
