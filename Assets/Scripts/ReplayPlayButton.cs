using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayPlayButton : MonoBehaviour
{
    private string _id;
    private DatabaseManager dbm;

    private void Start(){
        dbm = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
        _id = this.transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text;
    }
    public void ReplayPlayClick(){
        dbm.SetId(_id);
        SceneManager.LoadScene("ReplayPong");
    }
}
