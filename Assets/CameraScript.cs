using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	private AndroidPluginCallback plugin;

	public bool GetMouseDown(int type) {
		if (plugin == null)
			return false;

		return plugin.mouseDown[type];
	}

	class AndroidPluginCallback : AndroidJavaProxy
	{
		private int ACTION_UP = 0x00000001;
		private int ACTION_DOWN = 0x00000000;
		private int BUTTON_PRIMARY = 0x00000001;
		private int BUTTON_SECONDARY = 0x00000002;

		public float mouseDx = 0, mouseDy = 0;
		public bool[] mouseDown = new bool[]{false, false};

		private static int SOURCE_MOUSE_RELATIVE = 131076;

		public AndroidPluginCallback() : base("android.view.View$OnCapturedPointerListener") { 
			Debug.Log ("Intantiated plugin");
		}

		public bool IsMoving() {
			return mouseDx != 0 || mouseDy != 0;
		}

		private bool getState(AndroidJavaObject motionEvent, int type) {
			int state = motionEvent.Call<int> ("getButtonState");
			return (state & type) != 0;
		}

		public bool onCapturedPointer(AndroidJavaObject view, AndroidJavaObject motionEvent) {
			mouseDown[0] = getState (motionEvent, BUTTON_PRIMARY);
			mouseDown[1] = getState (motionEvent, BUTTON_SECONDARY);

			mouseDx += motionEvent.Call<float> ("getRawX");
			mouseDy += motionEvent.Call<float> ("getRawY");
			return false;
		}

		public void empty() {
			mouseDx = 0;
			mouseDy = 0;
		}
	}

	private float initialYAngle = 0f;
	private Gyroscope gyro;
	private Quaternion gyroAttitude;
	private bool gyroSupport; 

	// sensitivity vars
	public float mouseSpeed = 1f;

	private Camera camera;

	AndroidJavaObject GetUnity() {
		if (Application.isEditor)
			return null;

		Debug.Log ("Get Unity");
		AndroidJNI.AttachCurrentThread();
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		AndroidJavaObject jp = jo.Get<AndroidJavaObject> ("mUnityPlayer");
		return jp;
	}

	void SetScreenBrightness() {
		if (Application.isEditor)
			return;
		
		AndroidJNI.AttachCurrentThread();
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 

		AndroidJavaObject window = jo.Call<AndroidJavaObject>("getWindow");
		AndroidJavaObject prms = window.Call<AndroidJavaObject>("getAttributes");
		prms.Set ("screenBrightness", 1f);
		window.Call ("setAttributes", prms);
	}

	void AttachPlugin() {
		if (Application.isEditor)
			return;

		//SetScreenBrightness ();

		Debug.Log ("Attach plugin");
		AndroidJNI.AttachCurrentThread();
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		AndroidJavaObject jd = jo.Call<AndroidJavaObject>("getCurrentFocus");

		plugin = new AndroidPluginCallback ();
		jd.Call ("setOnCapturedPointerListener", plugin);
	}

	private void LockMouse() {
		if (Application.isEditor)
			return;

		Debug.Log ("Lock cursor");
		GetUnity().Call("requestPointerCapture");
		AttachPlugin ();
	}

	private void UnlockMouse() {
		if (Application.isEditor)
			return;

		Debug.Log ("Unlock cursor");
		GetUnity().Call("releasePointerCapture"); 
	}

	void Start()
	{
		// Portrait set
		Screen.orientation = ScreenOrientation.Portrait;

		camera = GetComponentInChildren<Camera>();

		gyroSupport = SystemInfo.supportsGyroscope;

		if (gyroSupport) {
			gyro = Input.gyro;
			gyro.enabled = true;
			camera.transform.Rotate (new Vector3(0,0,180));
		} else {
			print ("NO GYRO");
		}
	}

	void OnApplicationPause(bool state) {
		if (state) {
			UnlockMouse ();
		} else {
			LockMouse ();
		}
	}

	void Update()
	{
		// Before emptying mouse movement
		if (gyroSupport) {
			ApplyGyroRotation ();
		}
		ApplyMouseMovement ();
	}

	void ApplyMouseMovement() {
		if (plugin == null)
			return;

		if (GetMouseDown (1)) {
			transform.position -= transform.forward * plugin.mouseDy * mouseSpeed;
			transform.position += transform.right * plugin.mouseDx * mouseSpeed;
		} else {
			transform.position -= transform.forward * -plugin.mouseDy * mouseSpeed;
			transform.position += transform.right * -plugin.mouseDx * mouseSpeed;
		}
			

		plugin.empty ();
	}

	Quaternion rotation = new Quaternion(Quaternion.identity.x, 
		Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w);
	int movingCount = 0;
	void ApplyGyroRotation()
	{
		if (plugin.IsMoving ()) {
			movingCount = 30;
		}

		if (movingCount == 0) {
			gyroAttitude = Input.gyro.attitude;
			return;
		}

		movingCount--;

		Quaternion diff = gyroAttitude * Quaternion.Inverse(Input.gyro.attitude);
		if (GetMouseDown (1)) {
			rotation *= Quaternion.Inverse(diff);
		} else {
			rotation *= diff;
		}

		Vector3 euler = new Vector3 (0, rotation.eulerAngles.z, 0);
		Quaternion quat = Quaternion.Euler (euler);
		transform.localRotation = quat;

		// save the current gyro rotation for the next update
		gyroAttitude = Input.gyro.attitude;
	}
}
