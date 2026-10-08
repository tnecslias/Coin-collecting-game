using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Animations;
using UnityEditor.Build.Reporting;
using System.Linq;
using System.IO;
public static class BuildGame {
 const string Root="Assets/UnityChan/SD Unity-chan 3D Model Data/";
 [MenuItem("Kadai05/Build WebGL")]
 public static void Build(){
  Directory.CreateDirectory("Assets/Generated");
  var tags=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
  var list=tags.FindProperty("tags");if(!Enumerable.Range(0,list.arraySize).Any(i=>list.GetArrayElementAtIndex(i).stringValue=="Coin")){list.InsertArrayElementAtIndex(list.arraySize);list.GetArrayElementAtIndex(list.arraySize-1).stringValue="Coin";tags.ApplyModifiedProperties();}
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  var ground=Mat("Grass",new Color(.94f,.96f,.95f));var gold=Mat("Gold",new Color(1,.86f,.03f));
  ground.shader=Shader.Find("CoinGarden/WhiteGround");gold.SetFloat("_Metallic",.15f);gold.SetFloat("_Glossiness",.6f);
  var floor=GameObject.CreatePrimitive(PrimitiveType.Cube);floor.name="Garden";floor.transform.position=new Vector3(0,-.3f,0);floor.transform.localScale=new Vector3(200,.6f,200);floor.GetComponent<Renderer>().sharedMaterial=ground;
  var player=new GameObject("Player");player.tag="Player";
  var cc=player.AddComponent<CharacterController>();cc.height=1.5f;cc.center=new Vector3(0,.75f,0);cc.radius=.35f;
  var rb=player.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
  var model=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"Models/SD_unitychan_humanoid.fbx"),player.transform);model.name="UnityChan";model.transform.localScale=Vector3.one*2.2f;player.transform.rotation=Quaternion.Euler(0,180,0);
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Textures/UnityChan/utc_all2_light.png");
  var skin=Mat("UnityChan",Color.white);skin.mainTexture=texture;
  foreach(var renderer in model.GetComponentsInChildren<Renderer>()) renderer.sharedMaterials=renderer.sharedMaterials.Select(m=>skin).ToArray();
  var anim=model.GetComponent<Animator>();if(!anim)anim=model.AddComponent<Animator>();anim.applyRootMotion=false;
  var clips=AssetDatabase.LoadAllAssetsAtPath(Root+"Animators/SD_unitychan_motion_humanoid.fbx").OfType<AnimationClip>().ToArray();
  var idle=clips.First(c=>c.name=="Standing@loop");var run=clips.First(c=>c.name=="Running@loop");
  AssetDatabase.DeleteAsset("Assets/Generated/Movement.controller");var ac=AnimatorController.CreateAnimatorControllerAtPath("Assets/Generated/Movement.controller");ac.AddParameter("IsMoving",AnimatorControllerParameterType.Bool);
  var sm=ac.layers[0].stateMachine;var a=sm.AddState("Idle");a.motion=idle;var b=sm.AddState("Run");b.motion=run;sm.defaultState=a;
  var ab=a.AddTransition(b);ab.hasExitTime=false;ab.duration=.12f;ab.AddCondition(AnimatorConditionMode.If,0,"IsMoving");
  var ba=b.AddTransition(a);ba.hasExitTime=false;ba.duration=.12f;ba.AddCondition(AnimatorConditionMode.IfNot,0,"IsMoving");anim.runtimeAnimatorController=ac;
  player.AddComponent<PlayerMovement>().animator=anim;
  var game=new GameObject("GameManager").AddComponent<CoinGame>();game.player=player.transform;game.coinMaterial=gold;
  var camera=new GameObject("Main Camera");camera.tag="MainCamera";var cam=camera.AddComponent<Camera>();cam.orthographic=true;cam.orthographicSize=6;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.94f,.96f,.95f);camera.AddComponent<FollowCamera>().target=player.transform;
  camera.transform.position=new Vector3(-1.8f,14,1);camera.transform.rotation=Quaternion.Euler(78,0,0);
  var light=new GameObject("Sun").AddComponent<Light>();light.type=LightType.Directional;light.intensity=.85f;light.transform.rotation=Quaternion.Euler(50,-30,0);light.shadows=LightShadows.Soft;light.shadowStrength=.5f;
  RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.28f,.30f,.34f);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),"Assets/Generated/CoinGarden.unity");
  PlayerSettings.companyName="Honoka";PlayerSettings.productName="Coin Garden";PlayerSettings.WebGL.template="PROJECT:CoinGarden";PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;PlayerSettings.WebGL.memorySize=128;PlayerSettings.WebGL.exceptionSupport=WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
  PlayerSettings.SetScriptingBackend(BuildTargetGroup.WebGL,ScriptingImplementation.IL2CPP);
  AssetDatabase.SaveAssets();
  var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Generated/CoinGarden.unity"},locationPathName="docs",target=BuildTarget.WebGL,options=BuildOptions.None});
  if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("WebGL build failed: "+report.summary.result);
  File.WriteAllText("docs/.nojekyll", "");
  Debug.Log("KADAI05_BUILD_SUCCESS");
 }
 static Material Mat(string name,Color color){var m=new Material(Shader.Find("Standard"));m.color=color;var path="Assets/Generated/"+name+".mat";AssetDatabase.DeleteAsset(path);AssetDatabase.CreateAsset(m,path);return m;}
}
