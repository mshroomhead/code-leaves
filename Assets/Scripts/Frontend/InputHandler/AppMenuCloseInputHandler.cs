﻿using Core;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Frontend.InputHandler
{
	public class AppMenuCloseInputHandler : MonoBehaviour, IInputClickHandler
	{
		public ApplicationManager AppManager;
		
		public void OnInputClicked(InputClickedEventData eventData)
		{
			AppManager.AppState.UiElements.AppMenu.IsActive.Value = false;
		}
	}
}
