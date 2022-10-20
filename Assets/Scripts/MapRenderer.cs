using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapRenderer : MonoBehaviour {

    private SpriteRenderer spriteRenderer;

    public float width, height;

    private void Start() {
        if(!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if(!spriteRenderer)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if(!spriteRenderer)
            Debug.LogError("Could not find any sprite renderer for MapRenderer " + name + ".");
    }

	public void Init(float width, float height) {
        this.width = width;
        this.height = height;
        if(!spriteRenderer)
            Start();
    }

	private IEnumerator LoadSprite(string url) {
        WWW www = new(url);
        yield return www;
        Debug.Log("Image successfully fetched from ["+url+"].");
        spriteRenderer.sprite = Sprite.Create(www.texture, new Rect(0, 0, width, height), new Vector2());
    }

    public void UpdateMap(UrlFetcher fetcher, int x, int y, int zoom) {
        string url = fetcher.CreateUrlTile(x, y, zoom);
        StartCoroutine(LoadSprite(url));
    }
}