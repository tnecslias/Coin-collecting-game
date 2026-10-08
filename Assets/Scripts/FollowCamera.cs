using UnityEngine;
public class FollowCamera:MonoBehaviour {
 public Transform target;
 void LateUpdate(){transform.position=target.position+new Vector3(-1.8f,14,1);transform.rotation=Quaternion.Euler(78,0,0);}
}
