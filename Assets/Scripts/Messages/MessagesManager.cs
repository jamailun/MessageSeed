﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using GoShared;

public class MessagesManager : MonoBehaviour {

	public static MessagesManager Instance { get; private set; }

	[SerializeField] private UnityEvent unauthorizedEvent;
	[SerializeField] private NewMessagesEvent newMessagesEvent;

	private readonly List<Message> messages = new();
	private Coordinates lastCoordinates;

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void UpdatePosition(Coordinates coordinates) {
		lastCoordinates = coordinates;
	}

	private bool callingServer = false;
	public void UpdateMessages(Coordinates coordinates) {
		if(callingServer)
			return;
		callingServer = true;
		StartCoroutine(CR_UpdateMessagesRequest());
	}

	private IEnumerator CR_UpdateMessagesRequest() {
		using(var www = RemoteApiManager.Instance.CreateAuthGetRequest("/messages/")) {
			yield return www.SendWebRequest();
			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + ". URL=[" + www.url + "] Request feedback: [" + www.downloadHandler?.text + "]");
				if(www.responseCode == 401) {
					// UNAUTHORIZED
					unauthorizedEvent?.Invoke();
				} else {
					AlertUI.OpenAlert("Could get messages: " + www.downloadHandler?.text);
				}
			} else {
				string json = www.downloadHandler.text;

				// Unity CANNOT handle [{}...]. It needs to be wrapped as {list:[{}...]}
				string jsonWrapped = CSharpExtension.WrapJsonToClass(json, "list");
				var headers = JsonUtility.FromJson<MessagesHeaderList>(jsonWrapped);

				Debug.Log("Successfully fetched " + headers.list.Length + " messages.");

				foreach(var header in headers.list) {
					messages.Add(new Message(header));
				}

				newMessagesEvent?.Invoke(messages);
			}
			callingServer = false;
		}
	}

	public void WriteMessage(string title, string content, CSharpExtension.Consumable<string> errorCallback, CSharpExtension.Runnable callback) {
		StartCoroutine(CR_SendCreateMessage(title, content, errorCallback, callback));
	}

	private IEnumerator CR_SendCreateMessage(string title, string content, CSharpExtension.Consumable<string> errorCallback, CSharpExtension.Runnable successCallback) {
		var dataSent = new MessageCreateRequest(title, content, lastCoordinates);
		string postData = JsonUtility.ToJson(dataSent);
		byte[] postDataRaw = System.Text.Encoding.UTF8.GetBytes(postData);

		using(UnityWebRequest www = RemoteApiManager.Instance.CreatePostRequest("/database/message/create/", postDataRaw, true)) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
				if(www.responseCode == 401) {
					// UNAUTHORIZED
					unauthorizedEvent?.Invoke();
				} else {
					errorCallback?.Invoke(www.error + " : " + www.downloadHandler?.text);
				}
			} else {
				Debug.Log("new message posted successfully !");
				successCallback?.Invoke();

				var header = JsonUtility.FromJson<MessageHeader>(www.downloadHandler.text);
				var body = JsonUtility.FromJson<MessageComplete>(www.downloadHandler.text);

				Message msg = new(header);
				msg.Complete(body);
				messages.Add(msg);

				Debug.Log("created new message " +msg);
				newMessagesEvent?.Invoke(CSharpExtension.AsList(msg));
			}
		}
	}

	public void LikeMessage(MessageDisplay owner) {
		StartCoroutine(CR_LikeMessage(owner));
	}

	private IEnumerator CR_LikeMessage(MessageDisplay owner) {
		Debug.Log("Sending like.");
		using(UnityWebRequest www = RemoteApiManager.Instance.CreateAuthPutRequest("/database/message/" + owner.Message.MessageId + "/like")) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError("LIKE:"+www.error + " : " + www.downloadHandler?.text);
				owner.EndOfLikeProcess();
				if(www.responseCode == 401) {
					// UNAUTHORIZED
					unauthorizedEvent?.Invoke();
				} else {
					AlertUI.OpenAlert("Could list this message: " + www.downloadHandler?.text);
				}
			} else {
				Debug.Log("Liked message successfully !");
				owner.Message.SetAsLiked();
				owner.EndOfLikeProcess();
			}
		}
	}

	[System.Serializable]
	private struct MessageCreateRequest {
		public string title;
		public string message;
		public double latitude;
		public double longitude;

		public MessageCreateRequest(string title, string content, Coordinates coordinates) {
			this.title = title;
			this.message = content;
			this.longitude = System.Math.Round(coordinates.longitude, 14);
			this.latitude = System.Math.Round(coordinates.latitude, 14);
		}
	}


	[System.Serializable]
	public class NewMessagesEvent : UnityEvent<List<Message>> { }

}