using UnityEngine;
using System.Runtime.InteropServices;
using System.Globalization;

public class CoinGame : MonoBehaviour {
    public Transform player;
    public Material coinMaterial;
    public int Score { get; private set; }
    int remaining, total;
    [DllImport("__Internal")] static extern void ReportCoins(int score, int remaining, int total);
    void Start() { RegenerateCoins(); }
    public void SetMove(string value) { player.GetComponent<PlayerMovement>().SetMove(value); }
    public void RegenerateCoins() {
        foreach(var coin in GameObject.FindGameObjectsWithTag("Coin")) { coin.SetActive(false); Destroy(coin); }
        total = remaining = Random.Range(10, 21);
        var positions = new System.Collections.Generic.List<Vector3>();
        for(int i=0;i<total;i++) {
            Vector3 p = Vector3.zero;
            for(int attempt=0;attempt<100;attempt++) {
                var offset=Random.insideUnitCircle*8;
                p=player.position+new Vector3(offset.x,0,offset.y); p.y=.7f;
                p.x=Mathf.Clamp(p.x,-13,13);p.z=Mathf.Clamp(p.z,-13,13);
                if(Vector3.Distance(p,player.position)>1.5f && !positions.Exists(v=>Vector3.Distance(v,p)<1.1f)) break;
            }
            positions.Add(p);
            var go = new GameObject("Coin");go.tag="Coin";go.transform.position=p;
            var trigger=go.AddComponent<SphereCollider>();trigger.radius=.55f;trigger.isTrigger=true;
            var visual=GameObject.CreatePrimitive(PrimitiveType.Cylinder);visual.name="Gold";
            Destroy(visual.GetComponent<Collider>());visual.transform.SetParent(go.transform,false);
            visual.transform.localRotation=Quaternion.identity;visual.transform.localScale=new Vector3(.65f,.09f,.65f);
            visual.GetComponent<Renderer>().sharedMaterial=coinMaterial;
            go.AddComponent<CoinPickup>().game=this;
        }
        Notify();
    }
    public void Collect() { Score++;remaining--;Notify(); }
    void Notify() {
#if UNITY_WEBGL && !UNITY_EDITOR
        ReportCoins(Score,remaining,total);
#endif
        Debug.Log($"COINS score={Score} remaining={remaining} total={total}");
    }
}
