﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Frontend;
using HoloToolkit.Unity;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

namespace Core
{
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        public UiNode Root;
        public Forest Forest;
        public AppState AppState;

        private readonly AppState InitialAppState = new AppState
        {
            FloorInteractionMode = new ReactiveProperty<FloorInteractionMode>(FloorInteractionMode.TapToMenu),
            AppData = null
        };

        private const string TreeDataFile = "AirCbsStructure.json";

        private readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        protected override void Awake()
        {
            AppState = InitialAppState;

            var path = Path.Combine(Application.streamingAssetsPath, TreeDataFile);

            var softwareRoot = DesirializeData<Package>(path);

            var node = SoftwareArtefactToNodeMapper.Map(softwareRoot);

            Forest = new Forest
            {
                Trees = (node as InnerNode)?.Children.Select(AppToUiMapper.Map).ToList() ??
                        new List<UiNode> {AppToUiMapper.Map(node)}
            };

            base.Awake();

//        SerializeData(Root);
        }

        private void Start()
        {
            var count = Forest.Trees.Count;
            for (var i = 0; i < count; i++)
            {
                TreeBuilder.Instance.GenerateTreeStructure(Forest.Trees[i],
                    new Vector2((i - count / 2) * 3, (i - count / 2) * 3));
            }
        }

        private void SerializeData(object obj, string path)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, JsonSerializerSettings);
            File.WriteAllText(path, json);
        }

        private T DesirializeData<T>(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("Connot load tree data, for there is no such file.");
            }
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }
    }
}