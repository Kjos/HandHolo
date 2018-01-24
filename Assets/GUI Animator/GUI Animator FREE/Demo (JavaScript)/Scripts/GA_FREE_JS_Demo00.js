// GUI Animator FREE
// Version: 1.1.5
// Compatilble: Unity 5.5.1 or higher, see more info in Readme.txt file.
//
// Developer:							Gold Experience Team (https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:4162)
//
// Unity Asset Store:					https://www.assetstore.unity3d.com/en/#!/content/58843
// See Full version:					https://www.assetstore.unity3d.com/en/#!/content/28709
//
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com


// ######################################################################
// GA_FREE_Demo00 class
// This class loads "GA FREE JS - Demo00 (960x600px)" scene.
// ######################################################################

class GA_FREE_JS_Demo00 extends MonoBehaviour {
	
	// ########################################
	// MonoBehaviour Functions
	// ########################################
	
	// Awake is called when the script instance is being loaded.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html
    function Awake () {
        if(enabled)
        {
            // Set GUIAnimSystemFREE.Instance.m_AutoAnimation to false in Awake() will let you control all GUI Animator elements in the scene via scripts.
			GUIAnimSystemFREE.Instance.m_AutoAnimation = false;
        }
    }

	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
    function Start () {
		StartCoroutine(ShowText(1.0));
    }
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
    function Update () {
		
    }
	
	// ########################################
	// Delay Functions
	// ########################################
    
    function ShowText (Delay:float) : IEnumerator
    {
    
		// Find game object names "Panel (Middle Center)"
		var go:GameObject = GameObject.Find("Panel (Middle Center)");

		// Play move-in animations
		if(go)
			GUIAnimSystemFREE.Instance.MoveIn(go.transform, true);

		// wait for 3 seconds
		yield WaitForSeconds(3);

		// Play move-out animations
		if(go)
			GUIAnimSystemFREE.Instance.MoveOut(go.transform, true);

		// Wait for a while
		yield WaitForSeconds(Delay/2);

		// Load next demo scene
		// Unity 5.3 or higher uses SceneManager.LoadScene instead of Application.LoadLevel,
		// see http://docs.unity3d.com/Manual/UpgradeGuide53.html
		// and http://docs.unity3d.com/530/Documentation/ScriptReference/SceneManagement.SceneManager.html
		SceneManagement.SceneManager.LoadScene("GA FREE JS - Demo01 (960x600px)");
    }
}