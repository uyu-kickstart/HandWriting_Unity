using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class RecognizeCharacter : BasePaintImage {
	private Texture2D recognizedTexture;

	public override void Start () {
		initialize ();
		recognizedTexture = texture;
	}

	public override void Update () {
		
	}

	public void Recognize_1 () {
		int right = 0, left = 0, top = 0, bottom = 0;
		int t = 0;
		bool flag = false;
		while (t < imageSize.x) {
			int u = 0;
			while (u < imageSize.y) {
				if (texture.GetPixel (t, u) == strokeColor) {
					left = t;
					flag = true;
				}
				++u;
			}
			if (flag) break;
			++t;
		}
		t = 0;
		flag = false;
		while (t < imageSize.x) {
			int u = 0;
			while (u < imageSize.y) {
				if (texture.GetPixel (u, t) == strokeColor) {
					bottom = t;
					flag = true;
				}
				++u;
			}
			if (flag) break;
			++t;
		}
		t = (int)imageSize.x;
		flag = false;
		while (t > 0) {
			int u = 0;
			while (u < imageSize.y) {
				if (texture.GetPixel (t, u) == strokeColor) {
					right = t;
					flag = true;
				}
				++u;
			}
			if (flag) break;
			--t;
		}
		t = (int)imageSize.y;
		flag = false;
		while (t > 0) {
			int u = 0;
			while (u < imageSize.y) {
				if (texture.GetPixel (u, t) == strokeColor) {
					top = t;
					flag = true;
				}
				++u;
			}
			if (flag) break;
			--t;
		}
		Debug.Log (new Vector4 (top, bottom, right, left));
		Debug.Log (right - left);
		Debug.Log (top - bottom);
		recognizedTexture = new Texture2D (right - left, top - bottom);
	}
}
