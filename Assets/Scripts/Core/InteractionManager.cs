﻿using System.Linq;
using Data;
using Frontend;
using Frontend.Models;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using Utilities;

namespace Core
{
    public class InteractionManager : Singleton<InteractionManager>
    {
        private AppState appState;
        private UiElements uiElements;

        public void Start()
        {
            appState = ApplicationManager.Instance.AppState;
            uiElements = appState.UiElements;
        }

        public void HandleNodeClick(string id)
        {
            var selected = FindUiNode(id);
            if (selected == null) return;
            selected.IsSelected.Value ^= true;
        }

        public void HandleNodeFocusEnter(string id)
        {
            var focused = FindUiNode(id);
            if (focused == null) return;
            focused
                .Traverse(x => (x as UiInnerNode)?.Children)
                .ToList()
                .ForEach(x => x.IsFocused.Value = true);
            uiElements.GazeText.Text.Value = focused.Text.Value;
            uiElements.GazeText.IsActive.Value = true;
        }

        public void HandleNodeFocusExit(string id)
        {
            var focused = FindUiNode(id);
            if (focused == null) return;
            focused
                .Traverse(x => (x as UiInnerNode)?.Children)
                .ToList()
                .ForEach(x => x.IsFocused.Value = false);
            uiElements.GazeText.IsActive.Value = false;
            uiElements.GazeText.Text.Value = "";
        }

        public void HandleCircleVisualizationToggle()
        {
            appState.Forest.Value.VisualizeCircles.Value ^= true;
        }

        public void HandleEmptyClick()
        {
            if (GazeManager.Instance.HitObject != null &&
                !GazeManager.Instance.HitObject.transform.IsChildOf(SpatialMappingManager.Instance.transform)) return;
            HandleFloorClick();
        }

        private UiNode FindUiNode(string id)
        {
            return appState.Forest.Value?.Root?
                .Traverse(x => (x as UiInnerNode)?.Children)
                .FirstOrDefault(x => x.Id == id);
        }

        public void HandleProjectSelection(int index)
        {
            var fileName = appState.AvailableExampleProjects[index];
            var softwareRoot = StreamingAssetsService.Instance.DesirializeData<Package>(fileName);
            appState.AppData.Value = new AppData {Root = SoftwareArtefactToNodeMapper.Map(softwareRoot)};
            appState.UiElements.AppMenu.Page.Value = AppMenuPage.Settings;
            uiElements.AppMenu.BackAvailable.Value = true;
        }

        public void HandleFloorClick()
        {
            uiElements.ContexMenu.IsActive.Value ^= true;
        }

        public void HandleIsPlacingToggle()
        {
            uiElements.ContexMenu.IsActive.Value = false;
            uiElements.IsPlacing.Value ^= true;
        }

        public void HandleScaleMode()
        {
            uiElements.ContexMenu.IsActive.Value = false;
            uiElements.ForestManipulationMode.Value = ManipulationMode.Scale;
            uiElements.ManipulationIndicators.Mode.Value = ManipulationMode.Scale;
        }

        public void HandleRotateMode()
        {
            uiElements.ContexMenu.IsActive.Value = false;
            uiElements.ForestManipulationMode.Value = ManipulationMode.Rotate;
            uiElements.ManipulationIndicators.Mode.Value = ManipulationMode.Rotate;
        }

        public void HandleShowProjectMenu()
        {
            uiElements.ContexMenu.IsActive.Value = false;
            uiElements.AppMenu.IsActive.Value = true;
            uiElements.AppMenu.IsTagalong.Value = true;
        }

        public void HandleAppMenuTagalongToggle()
        {
            uiElements.AppMenu.IsTagalong.Value ^= true;
        }

        public void HandleAppMenuHandDrag()
        {
            uiElements.AppMenu.IsTagalong.Value = false;
        }
        
        public void HandleManipulationStarted()
        {
            uiElements.ManipulationIndicators.IsActive.Value = true;
        }
        
        public void HandleManipulationCompleted()
        {
            uiElements.ManipulationIndicators.IsActive.Value = false;
        }

        public void HandleAppMenuBack()
        {
            if (uiElements.AppMenu.Page.Value == AppMenuPage.Settings)
            {
                uiElements.AppMenu.Page.Value = AppMenuPage.ProjectSelection;
                uiElements.AppMenu.BackAvailable.Value = false;
            }
        }
    }
}