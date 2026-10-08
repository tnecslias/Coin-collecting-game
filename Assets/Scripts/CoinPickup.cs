using UnityEngine;
public class CoinPickup : MonoBehaviour {
    public CoinGame game; bool collected; float baseY;
    void Start(){baseY=transform.position.y;}
    void Update(){transform.Rotate(0,100*Time.deltaTime,0);var p=transform.position;p.y=baseY+Mathf.Sin(Time.time*3+baseY)*.1f;transform.position=p;}
    void OnTriggerEnter(Collider other){if(collected || !other.CompareTag("Player"))return;collected=true;game.Collect();gameObject.SetActive(false);Destroy(gameObject);}
}
