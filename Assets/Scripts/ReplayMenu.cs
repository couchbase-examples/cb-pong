using UnityEngine;
using TMPro;
public class ReplayMenu : MonoBehaviour
{
    public Transform template;
    public Transform container;

    private DatabaseManager dbm;

    public Transform mainMenu;

    public void BackButton(){
        mainMenu.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void Awake(){
        template.gameObject.SetActive(false);
    }

    private void Start(){
        dbm = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
        var res = dbm.GetLatestTenReplays();

        float templateHeight = 32f;

        int i=0;

        foreach (var r in res){
            Transform temp = Instantiate(template, container);
            RectTransform tempRT = temp.GetComponent<RectTransform>();
            tempRT.anchoredPosition = new Vector2(0, -templateHeight*i);
            temp.Find("game_id").GetComponent<TextMeshProUGUI>().text = r.GetString("id");
            temp.Find("game_timestamp").GetComponent<TextMeshProUGUI>().text = r.GetDate("timestamp").ToString(@"dd\:hh\:mm\:ss");
            temp.Find("game_score").GetComponent<TextMeshProUGUI>().text = r.GetInt("p1Score") + " - " + r.GetInt("p2Score");
            temp.gameObject.SetActive(true);
            i++;
        }
    }
}
