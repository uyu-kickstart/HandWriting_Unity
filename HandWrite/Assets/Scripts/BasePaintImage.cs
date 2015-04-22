using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent (typeof(RawImage))]
[RequireComponent (typeof(RectTransform))]
[RequireComponent (typeof(CanvasRenderer))]
public class BasePaintImage : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler {

	#region Members & Properties

	[SerializeField,Range (1, 50)]
	public int strokeWidth = 5;
	public Color strokeColor = Color.black;

	public bool drawing {
		protected set {
			drawing_ = value;
		}
		get {
			return drawing_;
		}
	}

	public Vector2 imageSize {
		set {
			useRectTransform.sizeDelta = value;
			useImageSize = value;
			SetDiffernce ();
		}
		get {
			return useImageSize;
		}
	}

	public Texture2D texture {
		get {
			return useTexture;
		}
	}

	private Texture2D useTexture;
	private Vector2 useImageSize;
	private bool drawing_;
	private Vector2 imagePos;
	private RectTransform useRectTransform;
	private RawImage rawImage;
	private Vector2 beforeTouchPos;

	#endregion

	public virtual void Start () {

	}

	public virtual void Update () {
		SetDiffernce ();
	}

	#region EventProcesses

	public void OnBeginDrag (PointerEventData eventData) {
		if (eventData.pointerEnter == gameObject) {
			drawing_ = true;
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
		if (eventData.pointerEnter == gameObject) {
			drawing_ = false;
			Vector2 touchPosition = new Vector2 (eventData.position.x - imagePos.x, eventData.position.y - imagePos.y);
			DrawLine (touchPosition, beforeTouchPos);
			beforeTouchPos = touchPosition;
		}
	}

	public void OnPointerEnter (PointerEventData eventData) {
		if (drawing_) {
			beforeTouchPos = new Vector2 (eventData.position.x - imagePos.x, eventData.position.y - imagePos.y);
		}
	}

	public void OnPointerExit (PointerEventData eventData) {
		if (drawing_) {
			Vector2 touchPosition = new Vector2 (eventData.position.x - imagePos.x, eventData.position.y - imagePos.y);
			DrawLine (touchPosition, beforeTouchPos);
		}
	}

	#endregion

	#region some of class functions

	public void initialize () {
		drawing_ = false;
		useRectTransform = GetComponent<RectTransform> ();
		useImageSize = new Vector2 (useRectTransform.rect.width, useRectTransform.rect.height);
		imagePos = Vector2.zero;
		beforeTouchPos = Vector2.zero;
		SetDiffernce ();
		useTexture = new Texture2D ((int)useImageSize.x, (int)useImageSize.y);
		useTexture.filterMode = FilterMode.Bilinear;
		rawImage = GetComponent<RawImage> ();
		WhiteImage ();
	}

	public void WhiteImage () {
		rawImage.color = Color.white;
		int i = 0;
		while (i < useImageSize.x) {
			int j = 0;
			while (j < useImageSize.y) {
				useTexture.SetPixel (i, j, Color.white);
				++j;
			}
			++i;
		}
		SetImage ();
	}

	public void ClearImage () {
		rawImage.color = Color.clear;
		int i = 0;
		while (i < useImageSize.x) {
			int j = 0;
			while (j < useImageSize.y) {
				useTexture.SetPixel (i, j, Color.white);
				++j;
			}
			++i;
		}
		SetImage ();
	}

	public void BlackImage () {
		rawImage.color = Color.black;
		int i = 0;
		while (i < useImageSize.x) {
			int j = 0;
			while (j < useImageSize.y) {
				useTexture.SetPixel (i, j, Color.white);
				++j;
			}
			++i;
		}
		SetImage ();
	}

	public void FillColorImage (Color c) {
		rawImage.color = c;
		int i = 0;
		while (i < useImageSize.x) {
			int j = 0;
			while (j < useImageSize.y) {
				useTexture.SetPixel (i, j, Color.white);
				++j;
			}
			++i;
		}
		SetImage ();
	}

	public void SetDiffernce () {
		imagePos.x = useRectTransform.position.x - useImageSize.x / 2f;
		imagePos.y = useRectTransform.position.y - useImageSize.y / 2f;
	}

	public void SetImage () {
		useTexture.Apply ();
		rawImage.texture = useTexture;
	}

	#endregion

	#region DrawCircle Methods

	//this method use Bresenham's line algorithm
	void DrawFillCircle (Vector2 center, int radius) {
		int cx, cy, d, x, y;
		x = (int)center.x;
		y = (int)center.y;
		while (radius > 0 && x + radius < imageSize.x && x - radius > 0 && y + radius < imageSize.y && y - radius > 0) {
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
		SetImage ();
	}

	void DrawFillCircle (float x_, float y_, int radius) {
		int cx, cy, d, x, y;
		x = (int)x_;
		y = (int)y_;
		while (radius > 0 && x + radius < imageSize.x && x - radius > 0 && y + radius < imageSize.y && y - radius > 0) {
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
		SetImage ();
	}

	void DrawFillCircle (int x, int y, int radius) {
		int cx, cy, d;
		while (radius > 0 && x + radius < imageSize.x && x - radius > 0 && y + radius < imageSize.y && y - radius > 0) {
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
		SetImage ();
	}

	#endregion

	#region DrawLine Methods

	//this method use Bresenham's line algorithm
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
				DrawFillCircle (x1, y1, strokeWidth);
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
				DrawFillCircle (x1, y1, strokeWidth);
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
				DrawFillCircle (x1, y1, strokeWidth);
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
				DrawFillCircle (x1, y1, strokeWidth);
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
				DrawFillCircle (x1, y1, strokeWidth);
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
				DrawFillCircle (x1, y1, strokeWidth);
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

	#region DrawRect

	//ここ今のところいらない
	void DrawRectAngle (int x, int y, int width, int height) {
		if (x > useImageSize.x) x = (int)useImageSize.x;
		else if (x < 0) x = 0;
		if (y > useImageSize.y) y = (int)useImageSize.y;
		else if (y < 0) y = 0;
		if (width > useImageSize.x) width = (int)useImageSize.x;
		else if (width < 0) width = 0;
		if (height > useImageSize.y) height = (int)useImageSize.y;
		else if (height < 0) height = 0;
		for (int i = 0; i < width; ++i) {
			useTexture.SetPixel (x + i, y, strokeColor);
			useTexture.SetPixel (x + i, y + height, strokeColor);
		}
		for (int i = 0; i < height; ++i) {
			useTexture.SetPixel (x, y + i, strokeColor);
			useTexture.SetPixel (x + width, y + i, strokeColor);
		}
	}

	#endregion

}
