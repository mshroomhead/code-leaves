﻿using Core;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Frontend
{
    public class DragToScale : MonoBehaviour, IManipulationHandler
    {
        public GameObject Target;
        public float ScaleFactor = 0.5f;
        public float MinScale = 0.5f;
        public float MaxScale = 10;

        private float OriginalScale;

        private void Start()
        {
            if (Target == null) Target = gameObject;
        }

        public void OnManipulationStarted(ManipulationEventData eventData)
        {
            OriginalScale = Target.transform.localScale.x;
            InputManager.Instance.PushModalInputHandler(gameObject);
        }

        public void OnManipulationUpdated(ManipulationEventData eventData)
        {
            Scale(eventData.CumulativeDelta.x);
        }

        public void OnManipulationCompleted(ManipulationEventData eventData)
        {
            InputManager.Instance.PopModalInputHandler();
            InteractionManager.Instance.HandleFloorInteractionCompleted();
        }

        public void OnManipulationCanceled(ManipulationEventData eventData)
        {
            ApplyScale(OriginalScale);
        }

        private void Scale(float delta)
        {
            var scale = Mathf.Clamp(OriginalScale + delta * ScaleFactor, MinScale, MaxScale);
            ApplyScale(scale);
        }

        private void ApplyScale(float scale)
        {
            Target.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}