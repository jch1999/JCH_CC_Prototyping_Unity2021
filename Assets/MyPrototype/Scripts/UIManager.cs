using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PanelID
{
    ItemGetPanel=1,
    ExplainPanel=2,
}

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject CenterPoint;

    [Tooltip("Panels")]
    [SerializeField]
    GameObject ItemGetPanel;
    [SerializeField]
    GameObject ExplainPanel;

    [Space(10)]

    [Tooltip("Text")]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI explainScoreText;
    [SerializeField]
    private GameObject explainText;

    // public void UIPanelActive(PanelID id)
    // {
    //     CenterPoint.SetActive(false);
    //     if(id==PanelID.ItemGetPanel)
    //     {
    //         ItemGetPanel.SetActive(true);
    //     }
    //     else if(id==PanelID.ExplainPanel)
    //     {
    //         ExplainPanel.SetActive(true);
    //     }
    // }
    public void UIPanelActive(int id)
    {
        CenterPoint.SetActive(false);
        if(id==(int)PanelID.ItemGetPanel)
        {
            ItemGetPanel.SetActive(true);
        }
        else if(id==(int)PanelID.ExplainPanel)
        {
            ExplainPanel.SetActive(true);
            explainText.SetActive(false);
        }
    }
    public void UIPanelDeactive(PanelID id)
    {
        CenterPoint.SetActive(true);
        if(id==PanelID.ItemGetPanel)
        {
            ItemGetPanel.SetActive(false);
        }
        else if(id==PanelID.ExplainPanel)
        {
            ExplainPanel.SetActive(false);
            explainText.SetActive(true);
        }
    }

    public void UpdateScoreText(int nowScore,int MaxScore)
    {
        scoreText.text=$"{nowScore}/{MaxScore}";
        explainScoreText.text=$"You gather {nowScore}/{MaxScore}";
    }
}
