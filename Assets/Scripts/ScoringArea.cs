using UnityEngine;
using UnityEngine.EventSystems;

public class ScoringArea : MonoBehaviour
{
    public EventTrigger.TriggerEvent scoreTrigger;

    private void OnCollisionEnter2D(Collision2D collision){
        BaseBall ball = collision.gameObject.GetComponent<BaseBall>();
        if(ball != null){
            BaseEventData eventData = new BaseEventData(EventSystem.current);
            this.scoreTrigger.Invoke(eventData);
        }
    }
}
