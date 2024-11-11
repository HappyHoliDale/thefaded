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
using TMPro;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class SkillPannelScript : MonoBehaviour
{
    SkillTree skillTree;
    public GameObject skillInfoPannel;
    // name price desc
    private GameObject skillInfo = null;
    Node thisSkill;
    private int skillPrice;
    private string skillDescription;

    void Start()
    {
        skillTree = GameObject.Find("SkillTree").GetComponent<SkillTree>();
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => skillTree.PnlClicked(this.gameObject));
        thisSkill = skillTree.skillTree.FindNode(skillTree.skillTree.StartNode, gameObject.name);
        skillPrice = thisSkill.price;
        skillDescription = skillTree.skillSulMyong[gameObject.name];
    }
    public void OnHover()
    {
        Debug.Log("hovering");
        Vector3 translate = new Vector3(350, -300);
        if (skillInfo == null)
        {
            skillInfo = Instantiate(skillInfoPannel, transform);
            skillInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gameObject.name;
            skillInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = skillPrice.ToString();
            skillInfo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = skillDescription;
            skillInfo.transform.SetAsLastSibling();
        }
        skillInfo.transform.localPosition = translate;

    }

    public void OnHoverOut()
    {
        Debug.Log("onExit");
        if (skillInfo != null)
            Destroy(skillInfo.gameObject);
    }
}
