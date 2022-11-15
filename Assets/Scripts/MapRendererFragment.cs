﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(SpriteRenderer))]
public class MapRendererFragment : MonoBehaviour {

    private SpriteRenderer _spriteRenderer;
    private float _width, _height;
    public Bounds Bounds => _spriteRenderer.bounds;

    private void Start() {
        if(!_spriteRenderer)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        if(!_spriteRenderer)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if(!_spriteRenderer)
            Debug.LogError("Could not find any sprite renderer for MapRenderer " + name + ".");
    }

	public void Init(float width, float height) {
        this._width = width;
        this._height = height;
        if(!_spriteRenderer)
            Start();
    }

	private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        var r = GetComponent<SpriteRenderer>();
        //Gizmos.DrawWireCube(r.bounds.center, r.bounds.size);
        Gizmos.DrawSphere(r.bounds.min, 0.05f);
        Gizmos.DrawSphere(r.bounds.max, 0.05f);
    }

	private IEnumerator LoadSprite(string url, int x, int y, int zoom) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.LogError("Could not fetch tecture from [" + url + "] : " + www.error);
        } else {
            Debug.Log("Image successfully fetched from [" + url + "].");
            Texture2D texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
            texture.filterMode = FilterMode.Point;
            _spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, _width, _height), new Vector2(.5f, .5f));
            // save it in the buffer
            TilesBuffer.Instance.Put(zoom, x, y, _spriteRenderer.sprite);
        }
    }

    public void UpdateMap(UrlFetcher fetcher, int x, int y, int zoom) {

        TextureQuery res = TilesBuffer.Instance.TryGet(zoom, x, y);
        if(res.found) {
            _spriteRenderer.sprite = res.texture;
        } else {
            string url = fetcher.CreateUrlTile(x, y, zoom);
            StartCoroutine(LoadSprite(url, x, y, zoom));
        }
    }
}