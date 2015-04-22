using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintImage: MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler {

	#region Ikareta Menba- Wo Syoukai Suruze!!

	public int strokeWidth = 5;
	public Color strokeColor = Color.black;
	[System.NonSerialized]
	public bool drawing;
	[System.NonSerialized]
	public RawImage rawImage;
	private Texture2D useTexture;
	private RectTransform rectTransform;
	private Vector2 imageSize;
	private Vector2 imagePos;
	private Vector2 beforeTouchPos;

	#endregion

	void Start () {
		drawing = false;
		rectTransform = GetComponent<RectTransform> ();
		imageSize = new Vector2 (rectTransform.rect.width, rectTransform.rect.height);
		imagePos = Vector2.zero;
		beforeTouchPos = Vector2.zero;
		SetDiffernce ();
		useTexture = new Texture2D ((int)imageSize.x, (int)imageSize.y);
		useTexture.filterMode = FilterMode.Bilinear;
		rawImage = GetComponent<RawImage> ();
		ClearImage ();
	}

	#region DragProcessing

	public void OnBeginDrag (PointerEventData eventData) {
		drawing = true;
		if (eventData.pointerEnter == gameObject) {
			Vector2 touchPosition = new Vector2 (eventData.position.x - imagePos.x, eventData.position.y - imagePos.y);
			beforeTouchPos = touchPosition;
			DrawLine (touchPosition, beforeTouchPos);
		}
	}

	public void OnDrag (PointerEventData eventData) {
		if (eventData.pointerEnter == gameObject) {
			Vector2 touchPosition = new Vector2 (eventData.position.x - imagePos.x, eventData.position.y - imagePos.y);
			DrawLine (touchPosition, beforeTouchPos);
			beforeTouchPos = touchPosition;
		}
	}

	public void OnEndDrag (PointerEventData eventData) {
		drawing = false;
		if (eventData.pointerEnter == gameObject) {
			Vector2 touchPosition = new Vector2 (eventData.position.x - imagePos.x, eventData.position.y - imagePos.y);
			DrawLine (touchPosition, beforeTouchPos);
			beforeTouchPos = touchPosition;
		}
	}

	#endregion

	#region ButtonsFuctions

	public void ClearImage () {
		int i = 0;
		while (i < imageSize.x) {
			int j = 0;
			while (j < imageSize.y) {
				useTexture.SetPixel (i, j, Color.white);
				++j;
			}
			++i;
		}
		useTexture.Apply ();
		rawImage.texture = useTexture;
	}

	public void RecognizeImage () {
		int i = 0;
		while (i < imageSize.x) {
			useTexture.SetPixel (i, 32, Color.black);
			++i;
		}
		useTexture.Apply ();
		rawImage.texture = useTexture;
	}

	#endregion

	public void SetDiffernce () {
		imagePos.x = rectTransform.position.x - imageSize.x / 2f;
		imagePos.y = rectTransform.position.y - imageSize.y / 2f;
	}

	#region DrawCircle Methods

	void DrawCircle (Vector2 center, int radius) {
		int cx, cy, d, x, y;
		x = (int)center.x;
		y = (int)center.y;
		while (radius > 0) {
			cy = radius;
			d = 3 - 2 * radius;
			useTexture.SetPixel (x + radius, y, strokeColor);
			useTexture.SetPixel (x - radius, y, strokeColor);
			useTexture.SetPixel (x, y + radius, strokeColor);
			useTexture.SetPixel (x, y - radius, strokeColor);
			for (cx = 0; cx <= cy; cx++) {
				if (d < 0) {
					d += 6 + 4 * cx;
				} else {
					d += 10 + 4 * cx - 4 * cy--;
				}
				useTexture.SetPixel (cy + x, cx + y, strokeColor);
				useTexture.SetPixel (cx + x, cy + y, strokeColor);
				useTexture.SetPixel (-cx + x, cy + y, strokeColor);
				useTexture.SetPixel (-cy + x, cx + y, strokeColor);
				useTexture.SetPixel (-cy + x, -cx + y, strokeColor);
				useTexture.SetPixel (-cx + x, -cy + y, strokeColor);
				useTexture.SetPixel (cx + x, -cy + y, strokeColor);
				useTexture.SetPixel (cy + x, -cx + y, strokeColor);
			}
			radius--;
		}
		useTexture.Apply ();
	}

	void DrawCircle (int x, int y, int radius) {
		int cx, cy, d;
		while (radius > 0) {
			cy = radius;
			d = 3 - 2 * radius;
			useTexture.SetPixel (x + radius, y, strokeColor);
			useTexture.SetPixel (x - radius, y, strokeColor);
			useTexture.SetPixel (x, y + radius, strokeColor);
			useTexture.SetPixel (x, y - radius, strokeColor);
			for (cx = 0; cx <= cy; cx++) {
				if (d < 0) {
					d += 6 + 4 * cx;
				} else {
					d += 10 + 4 * cx - 4 * cy--;
				}
				useTexture.SetPixel (cy + x, cx + y, strokeColor);
				useTexture.SetPixel (cx + x, cy + y, strokeColor);
				useTexture.SetPixel (-cx + x, cy + y, strokeColor);
				useTexture.SetPixel (-cy + x, cx + y, strokeColor);
				useTexture.SetPixel (-cy + x, -cx + y, strokeColor);
				useTexture.SetPixel (-cx + x, -cy + y, strokeColor);
				useTexture.SetPixel (cx + x, -cy + y, strokeColor);
				useTexture.SetPixel (cy + x, -cx + y, strokeColor);
			}
			radius--;
		}
		useTexture.Apply ();
	}

	#endregion

	#region DrawLine Methods

	void DrawLine (Vector2 vec1, Vector2 vec2) {
		int dx, dy, sx, sy, e, a, a1, x1, y1, x2, y2;
		x1 = (int)vec1.x;
		y1 = (int)vec1.y;
		x2 = (int)vec2.x;
		y2 = (int)vec2.y;
		if (x1 <= x2) {
			dx = x2 - x1;
			sx = 1;
		} else {
			dx = x1 - x2;
			sx = -1;
		}
		if (y1 <= y2) {
			dy = y2 - y1;
			sy = 1;
		} else {
			dy = y1 - y2;
			sy = -1;
		}
		if (dx >= dy) {
			a = 2 * dy;
			a1 = a - 2 * dx;
			e = a - dx;
			while (x1 != x2) {
				DrawCircle (x1, y1, strokeWidth);
				if (e >= 0) {
					y1 += sy;
					e += a1;
				} else {
					e += a;
				}
				x1 += sx;
			}
		} else {
			a = 2 * dx;
			a1 = a - 2 * dy;
			e = a - dx;
			while (y1 != y2) {
				DrawCircle (x1, y1, strokeWidth);
				if (e >= 0) {
					x1 += sx;
					e += a1;
				} else {
					e += a;
				}
				y1 += sy;
			}
		}
	}

	void DrawLine (int x1, int y1, int x2, int y2) {
		int dx, dy, sx, sy, e, a, a1;
		if (x1 <= x2) {
			dx = x2 - x1;
			sx = 1;
		} else {
			dx = x1 - x2;
			sx = -1;
		}
		if (y1 <= y2) {
			dy = y2 - y1;
			sy = 1;
		} else {
			dy = y1 - y2;
			sy = -1;
		}
		if (dx >= dy) {
			a = 2 * dy;
			a1 = a - 2 * dx;
			e = a - dx;
			while (x1 != x2) {
				DrawCircle (x1, y1, strokeWidth);
				if (e >= 0) {
					y1 += sy;
					e += a1;
				} else {
					e += a;
				}
				x1 += sx;
			}
		} else {
			a = 2 * dx;
			a1 = a - 2 * dy;
			e = a - dx;
			while (y1 != y2) {
				DrawCircle (x1, y1, strokeWidth);
				if (e >= 0) {
					x1 += sx;
					e += a1;
				} else {
					e += a;
				}
				y1 += sy;
			}
		}
	}

	void DrawLine (float x1_, float y1_, float x2_, float y2_) {
		int dx, dy, sx, sy, e, a, a1, x1, y1, x2, y2;
		x1 = (int)x1_;
		y1 = (int)y1_;
		x2 = (int)x2_;
		y2 = (int)y2_;
		if (x1 <= x2) {
			dx = x2 - x1;
			sx = 1;
		} else {
			dx = x1 - x2;
			sx = -1;
		}
		if (y1 <= y2) {
			dy = y2 - y1;
			sy = 1;
		} else {
			dy = y1 - y2;
			sy = -1;
		}
		if (dx >= dy) {
			a = 2 * dy;
			a1 = a - 2 * dx;
			e = a - dx;
			while (x1 != x2) {
				DrawCircle (x1, y1, strokeWidth);
				if (e >= 0) {
					y1 += sy;
					e += a1;
				} else {
					e += a;
				}
				x1 += sx;
			}
		} else {
			a = 2 * dx;
			a1 = a - 2 * dy;
			e = a - dx;
			while (y1 != y2) {
				DrawCircle (x1, y1, strokeWidth);
				if (e >= 0) {
					x1 += sx;
					e += a1;
				} else {
					e += a;
				}
				y1 += sy;
			}
		}
	}

	#endregion

}
