using UnityEngine;
using System.Globalization;
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    public Animator animator;
    public bool IsMoving { get; private set; }
    Vector2 touch;
    CharacterController controller;
    void Awake(){controller=GetComponent<CharacterController>();}
    public void SetMove(string value){
        var parts=value.Split(',');
        if(parts.Length==2 && float.TryParse(parts[0],NumberStyles.Float,CultureInfo.InvariantCulture,out var x) && float.TryParse(parts[1],NumberStyles.Float,CultureInfo.InvariantCulture,out var y) && !float.IsNaN(x) && !float.IsNaN(y) && !float.IsInfinity(x) && !float.IsInfinity(y)) touch=Vector2.ClampMagnitude(new Vector2(x,y),1);
    }
    void Update(){
        Vector2 input=touch.sqrMagnitude>.001f?touch:new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        input=Vector2.ClampMagnitude(input,1);var delta=new Vector3(input.x,0,input.y)*5f*Time.deltaTime;
        var before=transform.position;
        var target=before+delta;target.x=Mathf.Clamp(target.x,-13.5f,13.5f);target.z=Mathf.Clamp(target.z,-13.5f,13.5f);
        controller.Move(target-before+Vector3.down*3f*Time.deltaTime);
        var moved=transform.position-before;moved.y=0;IsMoving=moved.sqrMagnitude>.000001f;
        if(IsMoving) transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(moved),Time.deltaTime*14);
        animator.SetBool("IsMoving",IsMoving);
    }
    void OnApplicationFocus(bool focus){if(!focus)touch=Vector2.zero;}
}
