using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CameraAccess : MonoBehaviour {

	private bool camAvailable = false;
	private WebCamDevice[] devices;
	private WebCamTexture currentCam;
	private Texture defaultBackground;

	private int cameraHeight;
	private int cameraWidth;
	private RectTransform camBackground;
	private RectTransform camMask;
	public RawImage camRenderer;
	public bool frontFacing;

	public string imgPath = "/Pictures/Jankenpon/";
	public string photoName = "test.png";

	void Start() {
		defaultBackground = camRenderer.texture;
		if (camRenderer == null) {
			Debug.LogError("There is no RawImage reference to the camera renderer!");
			return;
		}
		camMask = (RectTransform)camRenderer.rectTransform.parent;
		camBackground = (RectTransform)camMask.parent;

		// Get the dimesions of the camBackground and set it to the camRenderer, as the
		// first one occupies the whole screen
		Vector2 size = new Vector2(camBackground.rect.width, camBackground.rect.height);
		camRenderer.rectTransform.sizeDelta = size;


		imgPath = Application.persistentDataPath + "/";
		GetDevices();
	}

	void Update() {
		if(currentCam == null || !currentCam.isPlaying)
			return;
		
		//float ratio = currentCam.width / (float)currentCam.height;
		//fitter.aspectRatio = ratio;

		float scaleY = currentCam.videoVerticallyMirrored ? -1f : 1f;
		camRenderer.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

		int orient = -currentCam.videoRotationAngle;
		camRenderer.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
	}

	private void GetDevices() {
		devices = WebCamTexture.devices;

		if (devices.Length == 0){
			Debug.Log("There is no cam device available.");
			return;
		}

		Debug.Log(devices.Length + " camera devices were found.");
	}

	private Vector2 MinSquareVector(Vector2 vector) {
		if(vector.x > vector.y)
			vector.x = vector.y;
		else
			vector.y = vector.x;
		
		return vector;
	}

	public void GetFirstCam() {
		if (devices.Length > 0) {
			Vector2 size = MinSquareVector(camRenderer.rectTransform.sizeDelta);
			currentCam = new WebCamTexture(devices[0].name, (int)size.x, (int)size.y, 15);
			print("Using the camera " + devices[0].name);
			print("BEFORE req Cam width = " + currentCam.requestedWidth + ". Cam height = " + currentCam.requestedHeight);
			print("BEFORE normal Cam width = " + currentCam.width + ". Cam height = " + currentCam.height);
			camAvailable = true;
		}
	}

	public void GetBackCamera() {
		foreach (WebCamDevice device in devices) {
			Debug.Log("Found a new device called " + device.name);
			if (!device.isFrontFacing) {
				print("This cam isnt FrontFacing (so, I suppose its the back camera).");
				currentCam = new WebCamTexture(device.name, cameraWidth, cameraHeight, 15);
				print("Cam width = " + currentCam.width + ". Cam height = " + currentCam.height);
				camAvailable = true;
			}
		}
		
		if (currentCam == null) {
			Debug.Log("No back camera found.");
			return;
		}
	}

	public void StartCamera() {
		if (camAvailable) {
			currentCam.Play();
			camRenderer.texture = currentCam;
			print("PLAY req Cam width = " + currentCam.requestedWidth + ". Cam height = " + currentCam.requestedHeight);
			print("PLAY normal Cam width = " + currentCam.width + ". Cam height = " + currentCam.height);

			Vector2 size = new Vector2(currentCam.width, currentCam.height);
			camRenderer.rectTransform.sizeDelta = size;
			camMask.sizeDelta = MinSquareVector(size);
		}
	}

	public void StopCamera() {
		if (camAvailable && currentCam.isPlaying) {
			currentCam.Stop();
			camRenderer.texture = defaultBackground;
		}
	}

	public void TakeShot() {
		if (currentCam != null && currentCam.isPlaying) {
			Debug.Log("Taking a shot");
			StartCoroutine(SaveCamTexture());
		} else {
			Debug.Log("Cant take a photo right now. Either the camera is not playing or there is no current cam.");
		}
	}

	public IEnumerator SaveCamTexture() {
		yield return new WaitForEndOfFrame();

		Debug.Log("Saving the camera Texture.");
		Texture2D photo = new Texture2D(currentCam.width, currentCam.height);
		print("Texture width = " + photo.width + ". Texture height = " + photo.height);
		photo.SetPixels(currentCam.GetPixels());
		photo.Apply();

		SavePNG(photo, imgPath, photoName);

		StopCamera();
	}

	public void SavePNG(Texture2D photo, string path, string fileName) {
		byte[] bytes = photo.EncodeToPNG();
		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}
		Debug.Log("Saving a photo at \"" + path + fileName + "\"");
		File.WriteAllBytes(path + fileName, bytes);
	}
}
