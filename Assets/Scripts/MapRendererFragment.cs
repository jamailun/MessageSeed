using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(SpriteRenderer))]
public class MapRendererFragment : MonoBehaviour {

    public static MapRendererFragment CENTER { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private float _width, _height;
    private int _i, _j;
    public int IndexI => _i;
    public int IndexJ => _j;
    public Bounds Bounds => _spriteRenderer.bounds;
    public bool IsLoading { get;  private set; }

    // this point is the Unity position for the corresponding worlds coordinates.
    public Vector2 TopLeft => new(Bounds.min.x, Bounds.max.y);

    private void Start() {
        if(!_spriteRenderer)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        if(!_spriteRenderer)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if(!_spriteRenderer)
            Debug.LogError("Could not find any sprite renderer for MapRenderer " + name + ".");
    }

	public void Init(float width, float height, int i, int j) {
        this._width = width;
        this._height = height;
        this._i = i;
        this._j = j;
        if(!_spriteRenderer)
            Start();
        if(i == 0 && j == 0) {
            CENTER = this;
		}
    }

	private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }

	private IEnumerator LoadSprite(string url, int x, int y, int zoom) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.LogError("Could not fetch tecture from [" + url + "] : " + www.error);
        } else {
            //Debug.Log("Image successfully fetched from [" + url + "].");
            Texture2D texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
            texture.filterMode = FilterMode.Point;
            _spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, _width, _height), new Vector2(.5f, .5f));
            // save it in the buffer
            TilesBuffer.Instance.Put(zoom, x, y, _spriteRenderer.sprite);
        }
        IsLoading = false;
    }

    private bool hasImage = false;
    private int lastX, lastY, lastZoom;
    [SerializeField] private Vector2 lastPosition;

    public Vector2 WorldPosition => lastPosition;

    public void UpdateMap(UrlFetcher fetcher, int x, int y, int zoom) {
        if(hasImage && lastX == x && lastY == y && lastZoom == zoom)
            return;

        TextureQuery res = TilesBuffer.Instance.TryGet(zoom, x, y);
        IsLoading = true;
        if(res.found) {
            _spriteRenderer.sprite = res.texture;
            IsLoading = false;
        } else {
            string url = fetcher.CreateUrlTile(x, y, zoom);
            StartCoroutine(LoadSprite(url, x, y, zoom));
        }
        hasImage = true;
        lastX = x;
        lastY = y;
        lastZoom = zoom;
        lastPosition = MapUtils.GetCoordinates(x, y, zoom);
        Debug.Log("(" + x + ", " + y + ", " + zoom + ") => " + lastPosition);
    }
}