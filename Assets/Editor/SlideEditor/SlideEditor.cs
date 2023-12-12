using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SlideEditor : EditorWindow
{
    private SceneDirector _sceneDirector;

    private string _loadedDataSceneName;
    private IntegerField slideIndex;

    private LocalisationData _localization;
    private BackgroundData _backgrounds;
    private EntityHistoryData _entityHistory;
    private BgSongData _bgSongs;
    private AudioEffectData _audioEffects;
    private ParticleSystemData _particleSystems;

    private readonly List<PropertyField> _properties = new();
    private readonly Dictionary<string, Toggle> _propertyToggles = new();
    private readonly List<Label> _propertyLabels = new();
    private readonly List<Button> _propertyButtons = new();
    private readonly Dictionary<string, TextField> _pathFields = new();

    private TextField soPackageName;
    private TextField soCustomFileName;
    private Toggle soAutoAssignToSceneDirector;
    private Toggle addBlankSlide;
    private readonly Dictionary<string, string> _assetPaths = new() {
        { "LOC_ASSET_DIR", "Assets/Scripts/Canvas/Dialogue/SO/" },
        { "BG_ASSET_DIR", "Assets/Scripts/Canvas/Backgrounds/SO/" },
        { "ENTITY_ASSET_DIR", "Assets/Scripts/Canvas/Entity/SO/" },
        { "BG_SONG_ASSET_DIR", "Assets/Scripts/Audio/SO/BgSongs/" },
        { "AUDIO_EFFECT_ASSET_DIR", "Assets/Scripts/Audio/SO/AudioEffects/" },
        { "PARTICLE_ASSET_DIR", "Assets/Scripts/Canvas/Particle Systems/SO/" }
    };

    [MenuItem("Window/Klein/SlideEditor")]
    public static void ShowExample()
    {
        SlideEditor wnd = GetWindow<SlideEditor>();
        wnd.titleContent = new GUIContent("SlideEditor");
    }

    public void CreateGUI()
    {
        _sceneDirector = GameObject.Find("SceneDirector").GetComponent<SceneDirector>();
        LoadData();

        #region ROOT
        VisualElement window = rootVisualElement;
        ScrollView root = new(ScrollViewMode.Vertical);
        window.Add(root);

        root.Add(new Label($"\n<b>Klein VN Slide Editor: loaded data from {_loadedDataSceneName}</b>\n"));
        #endregion

        #region CREATE SLIDE DATA
        root.Add(new Label("<b>Asset gen options</b>"));
        soPackageName = new("Package name")
        {
            value = $""
        };
        soPackageName.RegisterValueChangedCallback(UpdatePaths);
        root.Add(soPackageName);

        soCustomFileName = new("Custom file name")
        {
            value = $""
        };
        soCustomFileName.RegisterValueChangedCallback(UpdatePaths);
        root.Add(soCustomFileName);

        root.Add(new Label("<b>Asset paths</b>"));
        root.Add(CreatePathFields("Localisation", "LOC_ASSET_DIR", _assetPaths["LOC_ASSET_DIR"]));
        root.Add(CreatePathFields("Backgrounds", "BG_ASSET_DIR", _assetPaths["BG_ASSET_DIR"]));
        root.Add(CreatePathFields("Entities", "ENTITY_ASSET_DIR", _assetPaths["ENTITY_ASSET_DIR"]));
        root.Add(CreatePathFields("Bg songs", "BG_SONG_ASSET_DIR", _assetPaths["BG_SONG_ASSET_DIR"]));
        root.Add(CreatePathFields("Audio effects", "AUDIO_EFFECT_ASSET_DIR", _assetPaths["AUDIO_EFFECT_ASSET_DIR"]));
        root.Add(CreatePathFields("Particle systems", "PARTICLE_ASSET_DIR", _assetPaths["PARTICLE_ASSET_DIR"]));

        soAutoAssignToSceneDirector = new("Automatically assign generated SOs to the SceneDirector");
        soAutoAssignToSceneDirector.value = true;
        root.Add(soAutoAssignToSceneDirector);

        Button buttonCreateSlideSO = new()
        {
            name = "buttonCreateSlideSO",
            text = "Generate slide SOs"
        };
        buttonCreateSlideSO.clicked += () => CreateSlideSOs(_sceneDirector, root);
        if (_localization != null)
            buttonCreateSlideSO.SetEnabled(false);
        root.Add(buttonCreateSlideSO);
        #endregion

        #region LOAD SLIDE
        slideIndex = new("Slide index");
        root.Add(slideIndex);

        Button buttonLoadSlide = new()
        {
            name = "buttonLoadSlide",
            text = "Load slide at index"
        };
        buttonLoadSlide.clicked += () => Redraw(root);
        root.Add(buttonLoadSlide);

        Button buttonPrintIndexes = new()
        {
            name = "buttonPrintIndexes",
            text = "Print indexes"
        };
        buttonPrintIndexes.clicked += () =>
            Debug.Log($"loc: {slideIndex.value}\n" +
                (_backgrounds != null ? $"bg: {FindIndex(_backgrounds.indexes, slideIndex.value)}\n" : "bg not found") +
                (_audioEffects != null ? $"ae: {FindIndex(_audioEffects.indexes, slideIndex.value)}\n" : "ae not found") +
                (_bgSongs != null ? $"song: {FindIndex(_bgSongs.indexes, slideIndex.value)}\n" : "bgSongs not found") +
                (_entityHistory != null ? $"entities: {FindIndex(_entityHistory.indexes, slideIndex.value)}\n" : "entities not found") +
                (_particleSystems != null ? $"ps: {FindIndex(_particleSystems.indexes, slideIndex.value)}\n" : "particle systems not found"));
        root.Add(buttonPrintIndexes);
        #endregion

        #region SWITCH SLIDES
        Button buttonBack = new()
        {
            name = "buttonBack",
            text = "< Last Slide"
        };
        buttonBack.clicked += () => {
            slideIndex.value = Math.Clamp(slideIndex.value - 1, 0, _localization.dialogue.Count - 1);
            Redraw(root);
        };

        Button buttonNext = new()
        {
            name = "buttonNext",
            text = "Next Slide >"
        };
        buttonNext.clicked += () => {
            slideIndex.value = Math.Clamp(slideIndex.value + 1, 0, _localization.dialogue.Count - 1);
            Redraw(root);
        };
        root.Add(buttonBack);
        root.Add(buttonNext);
        #endregion

        #region CREATE SLIDE
        root.Add(new Label("\n<b>Add slide</b>"));

        addBlankSlide = new("add blank slide with all data");
        root.Add(addBlankSlide);

        Button buttonAddSlide = new()
        {
            name = "buttonAddSlide",
            text = "Add slide"
        };
        if (_localization == null)
            buttonAddSlide.SetEnabled(false);
        buttonAddSlide.clicked += () => CreateSlide(root);
        root.Add(buttonAddSlide);

        Button buttonAddSlideAtIndex = new()
        {
            name = "buttonAddSlideAtindex",
            text = "Insert new slide at index"
        };
        if (_localization == null)
            buttonAddSlideAtIndex.SetEnabled(false);
        buttonAddSlideAtIndex.clicked += () => InsertSlide(root);
        root.Add(buttonAddSlideAtIndex);

        #endregion

        DrawSlideData(root);
    }

    private void InsertSlide(VisualElement root)
    {
        int clampedIndex = Math.Clamp(slideIndex.value, 0, _localization.dialogue.Count);
        if (clampedIndex == _localization.dialogue.Count)
        {
            CreateSlide(root);
            return;
        }

        _localization.dialogue.Insert(clampedIndex, new());
        _localization.dialogueSpeed.Insert(clampedIndex, 0.01f);

        if (_backgrounds != null && (_propertyToggles["BG"].value || addBlankSlide.value))
        {
            // blank prefillData for better visibility of new element
            /*BackgroundDataType prefillData = _backgrounds.textures.Count > 0
                ? _backgrounds.textures[Math.Clamp(prevSlideIndex, 0, _backgrounds.textures.Count - 1)]
                : null;*/
            _backgrounds.textures.Insert(clampedIndex, new(null/*prefillData*/));
            int indexPos = FindIndex(_backgrounds.indexes, clampedIndex).indexPos;
            _backgrounds.indexes.Insert(indexPos, clampedIndex);
            for (int i = indexPos + 1; i < _backgrounds.indexes.Count; i++)
                _backgrounds.indexes[i]++;
        }

        if (_audioEffects != null && (addBlankSlide.value || _propertyToggles["AE"].value))
        {
            List<AudioEffectDataTypeCollection> audioEffectsList = _audioEffects.effects.ToList();
            audioEffectsList.Insert(clampedIndex, new(null));
            _audioEffects.effects = audioEffectsList.ToArray();
            int indexPos = FindIndex(_audioEffects.indexes, clampedIndex).indexPos;
            _audioEffects.indexes.Insert(indexPos, clampedIndex);
            for (int i = indexPos + 1; i < _audioEffects.indexes.Count; i++)
                _audioEffects.indexes[i]++;
        }

        if (_bgSongs != null && (addBlankSlide.value || _propertyToggles["BGS"].value))
        {
            _bgSongs.songs.Insert(clampedIndex, new(null));
            int indexPos = FindIndex(_bgSongs.indexes, clampedIndex).indexPos;
            _bgSongs.indexes.Insert(indexPos, clampedIndex);
            for (int i = indexPos + 1; i < _bgSongs.indexes.Count; i++)
                _bgSongs.indexes[i]++;
        }

        if (_entityHistory != null && (addBlankSlide.value || _propertyToggles["ENTITY"].value))
        {
            _entityHistory.history.Insert(clampedIndex, new(null));
            int indexPos = FindIndex(_entityHistory.indexes, clampedIndex).indexPos;
            _entityHistory.indexes.Insert(indexPos, clampedIndex);
            for (int i = indexPos + 1; i < _entityHistory.indexes.Count; i++)
                _entityHistory.indexes[i]++;
        }

        if (_particleSystems != null && (addBlankSlide.value || _propertyToggles["PS"].value))
        {
            _particleSystems.particleHistory.Insert(clampedIndex, new(null));
            int indexPos = FindIndex(_particleSystems.indexes, clampedIndex).indexPos;
            _particleSystems.indexes.Insert(indexPos, clampedIndex);
            for (int i = indexPos + 1; i < _particleSystems.indexes.Count; i++)
                _particleSystems.indexes[i]++;
        }

        Redraw(root);
    }

    private void CreateSlide(VisualElement root)
    {
        int newSlideIndex = _localization.dialogue.Count > 0 ? ++slideIndex.value : 0;
        int prevSlideIndex = Math.Clamp(newSlideIndex - 1, 0, int.MaxValue);

        _localization.dialogue.Add(new());
        _localization.dialogueSpeed.Add(0.01f);

        if (_backgrounds != null && (addBlankSlide.value || _propertyToggles["BG"].value))
        {
            BackgroundDataType prefillData = _backgrounds.textures.Count > 0
                ? _backgrounds.textures[Math.Clamp(prevSlideIndex, 0, _backgrounds.textures.Count - 1)]
                : null;
            _backgrounds.textures.Add(new(addBlankSlide.value ? null : prefillData));
            _backgrounds.indexes.Add(newSlideIndex);
        }

        if (_audioEffects != null && (addBlankSlide.value || _propertyToggles["AE"].value))
        {
            List<AudioEffectDataTypeCollection> audioEffectsList = _audioEffects.effects.ToList();
            AudioEffectDataTypeCollection prefillData = _audioEffects.effects.Length > 0
                ? _audioEffects.effects[Math.Clamp(prevSlideIndex, 0, _audioEffects.effects.Length - 1)]
                : null;
            audioEffectsList.Add(new(addBlankSlide.value ? null : prefillData));
            _audioEffects.effects = audioEffectsList.ToArray();
            _audioEffects.indexes.Add(newSlideIndex);
        }

        if (_bgSongs != null && (addBlankSlide.value || _propertyToggles["BGS"].value))
        {
            BgSongDataType prefillData = _bgSongs.songs.Count > 0
                ? _bgSongs.songs[Math.Clamp(prevSlideIndex, 0, _bgSongs.songs.Count - 1)]
                : null;
            _bgSongs.songs.Add(new(addBlankSlide.value ? null : prefillData));
            _bgSongs.indexes.Add(newSlideIndex);
        }

        if (_entityHistory != null && (addBlankSlide.value || _propertyToggles["ENTITY"].value))
        {
            EntityHistoryDataType prefillData = _entityHistory.history.Count > 0
                ? _entityHistory.history[Math.Clamp(prevSlideIndex, 0, _entityHistory.history.Count - 1)]
                : null;
            _entityHistory.history.Add(new(addBlankSlide.value ? null : prefillData));
            _entityHistory.indexes.Add(newSlideIndex);
        }

        if (_particleSystems != null && (addBlankSlide.value || _propertyToggles["PS"].value))
        {
            ParticleSystemDataTypeCollection prefillData = _particleSystems.particleHistory.Count > 0
                ? _particleSystems.particleHistory[Math.Clamp(prevSlideIndex, 0, _particleSystems.particleHistory.Count - 1)]
                : null;
            _particleSystems.particleHistory.Add(new(addBlankSlide.value ? null : prefillData));
            _particleSystems.indexes.Add(newSlideIndex);
        }

        Redraw(root);
    }

    private void CreateSlideSOs(SceneDirector _sceneDirector, VisualElement root)
    {
        if (!EditorUtility.DisplayDialog("Slide SO Assets", "This will create multiple ScriptableObjects as Assets. Continue?", "Yes", "No"))
            return;

        CreateAssetFolders(_pathFields["LOC_ASSET_DIR"].value.Split('/')[..^1]);
        LocalisationData loc = CreateInstance<LocalisationData>();
        AssetDatabase.CreateAsset(loc, _pathFields["LOC_ASSET_DIR"].value);

        CreateAssetFolders(_pathFields["BG_ASSET_DIR"].value.Split('/')[..^1]);
        BackgroundData bg = CreateInstance<BackgroundData>();
        AssetDatabase.CreateAsset(bg, _pathFields["BG_ASSET_DIR"].value);

        CreateAssetFolders(_pathFields["ENTITY_ASSET_DIR"].value.Split('/')[..^1]);
        EntityHistoryData entities = CreateInstance<EntityHistoryData>();
        AssetDatabase.CreateAsset(entities, _pathFields["ENTITY_ASSET_DIR"].value);

        CreateAssetFolders(_pathFields["BG_SONG_ASSET_DIR"].value.Split('/')[..^1]);
        BgSongData bgSongs = CreateInstance<BgSongData>();
        AssetDatabase.CreateAsset(bgSongs, _pathFields["BG_SONG_ASSET_DIR"].value);

        CreateAssetFolders(_pathFields["AUDIO_EFFECT_ASSET_DIR"].value.Split('/')[..^1]);
        AudioEffectData audioEffects = CreateInstance<AudioEffectData>();
        AssetDatabase.CreateAsset(audioEffects, _pathFields["AUDIO_EFFECT_ASSET_DIR"].value);

        CreateAssetFolders(_pathFields["PARTICLE_ASSET_DIR"].value.Split('/')[..^1]);
        ParticleSystemData particleSystems = CreateInstance<ParticleSystemData>();
        AssetDatabase.CreateAsset(particleSystems, _pathFields["PARTICLE_ASSET_DIR"].value);

        if (soAutoAssignToSceneDirector.value && _sceneDirector != null)
        {
            _sceneDirector.localization = loc;
            _sceneDirector.backgrounds = bg;
            _sceneDirector.entityHistory = entities;
            _sceneDirector.bgSongs = bgSongs;
            _sceneDirector.audioEffects = audioEffects;
            _sceneDirector.particleSystems = particleSystems;
        }

        addBlankSlide.value = true;
        CreateSlide(root);
        addBlankSlide.value = false;
    }

    private void CreateAssetFolders(string[] pathSplit)
    {
        // check each folder for validity, create new ones if it doesn't exist
        string parentFolder = "";

        foreach (string folder in pathSplit)
        {
            string newFolder = (parentFolder.Equals("") ? "" : parentFolder) + folder;
            if (!AssetDatabase.IsValidFolder(newFolder))
            {
                Directory.CreateDirectory(newFolder);
                // this does not work for some reason
                //AssetDatabase.CreateFolder(parentFolder, folder);

                Debug.Log($"Created new directory {newFolder}");
            }

            parentFolder += $"{folder}/";
        }
    }

    private void UpdatePaths(ChangeEvent<string> evt)
    {
        foreach (string label in _pathFields.Keys)
        {
            string packageName = soPackageName.value.Equals("") ? "" : $"{soPackageName.value}/";
            string fileName = soCustomFileName.value.Equals("") ? _pathFields[label].value.Split('/').Last() : $"{soCustomFileName.value}.asset";
            _pathFields[label].value = $"{_assetPaths[label]}{packageName}{fileName}";
        }
    }

    private void DrawSlideData(VisualElement root)
    {
        SerializedObject so;
        
        #region localisation
        if (_localization != null && _localization.dialogue.Count > 0)
        {
            root.Add(CreateLabel("\n<b>Localisation</b>"));
            so = new(_localization);
            root.Add(CreateProperty(so, $"dialogue.Array.data[{slideIndex.value}]", "Localisation"));
            root.Add(CreateProperty(so, $"dialogueSpeed.Array.data[{slideIndex.value}]", "Localisation Speed"));
        }
        else
            Debug.LogError("Slide Editor could not find LocalisationData or slide");
        #endregion

        int indexAt;
        #region background
        if (_backgrounds != null && _backgrounds.textures.Count > 0)
        {
            so = new(_backgrounds);
            indexAt = FindIndex(_backgrounds.indexes, slideIndex.value).indexPos;

            root.Add(CreateLabel($"\n<b>Background</b>, at index {indexAt}"));
            root.Add(CreateToggle($"include on new slide", "BG"));

            root.Add(CreateProperty(so, $"textures.Array.data[{indexAt}]", "Background"));
        }
        else
            Debug.LogError("Slide Editor could not find BackgroundData or slide");
        #endregion

        #region audioEffect
        if (_audioEffects != null && _audioEffects.effects.Length > 0)
        {
            so = new(_audioEffects);
            indexAt = FindIndex(_audioEffects.indexes, slideIndex.value).indexPos;

            root.Add(CreateLabel($"\n<b>Audio Effects</b>, at index {indexAt}"));
            root.Add(CreateToggle($"include on new slide", "AE"));

            root.Add(CreateProperty(so, $"effects.Array.data[{indexAt}]", "Audio Effects"));
        }
        else
            Debug.LogError("Slide Editor could not find AudioEffectsData or slide");
        #endregion

        #region bgSong
        if (_bgSongs != null && _bgSongs.songs.Count > 0)
        {
            so = new(_bgSongs);
            indexAt = FindIndex(_bgSongs.indexes, slideIndex.value).indexPos;

            root.Add(CreateLabel($"\n<b>Bg Song</b>, at index {indexAt}"));
            root.Add(CreateToggle($"include on new slide", "BGS"));

            root.Add(CreateProperty(so, $"songs.Array.data[{indexAt}]", "Bg Song"));
        }
        else
            Debug.LogError("Slide Editor could not find BgSongsData or slide");
        #endregion

        #region entities
        if (_entityHistory != null && _entityHistory.history.Count > 0)
        {
            so = new(_entityHistory);
            indexAt = FindIndex(_entityHistory.indexes, slideIndex.value).indexPos;

            root.Add(CreateLabel($"\n<b>Entities</b>, at index {indexAt}"));
            root.Add(CreateToggle($"include on new slide", "ENTITY"));

            root.Add(CreateButton("Copy previous slide", "entityCopySlideButton", () =>
            {
                int indexPos = FindIndex(_entityHistory.indexes, slideIndex.value).indexPos;
                int prevIndex = indexPos == 0 ? -1 : indexPos - 1;
                if (prevIndex == -1)
                    return;

                foreach (EntityDataType edt in _entityHistory.history[prevIndex].entities)
                    _entityHistory.history[indexPos].entities.Add(new(edt));
            }, slideIndex.value > 0));

            root.Add(CreateProperty(so, $"history.Array.data[{indexAt}]", "Entities"));
        }
        else
            Debug.LogError("Slide Editor could not find EntityHistoryData or slide");
        #endregion

        #region particleSystems
        if (_particleSystems != null && _particleSystems.particleHistory.Count > 0)
        {
            so = new(_particleSystems);
            indexAt = FindIndex(_particleSystems.indexes, slideIndex.value).indexPos;

            root.Add(CreateLabel($"\n<b>Particle Systems</b>, at index {indexAt}"));
            root.Add(CreateToggle($"include on new slide", "PS"));

            root.Add(CreateProperty(so, $"particleHistory.Array.data[{indexAt}]", "Particle Systems"));
        }
        else
            Debug.LogError("Slide Editor could not find ParticleSystemData or slide");
        #endregion
    }

    private void Redraw(VisualElement root)
    {
        foreach (PropertyField field in _properties)
            root.Remove(field);
        foreach (Label label in _propertyLabels)
            root.Remove(label);
        foreach (Toggle toggle in _propertyToggles.Values)
            root.Remove(toggle);
        foreach (Button button in _propertyButtons)
            root.Remove(button);

        _properties.Clear();
        _propertyLabels.Clear();
        _propertyToggles.Clear();
        _propertyButtons.Clear();

        DrawSlideData(root);
    }

    private void LoadData()
    {
        if (_sceneDirector == null)
        {
            Debug.LogError("SlideEditor could not find SceneDirector");
            return;
        }

        _loadedDataSceneName = SceneManager.GetActiveScene().name;
        _localization = _sceneDirector.localization;
        _backgrounds = _sceneDirector.backgrounds;
        _audioEffects = _sceneDirector.audioEffects;
        _bgSongs = _sceneDirector.bgSongs;
        _entityHistory = _sceneDirector.entityHistory;
        _particleSystems = _sceneDirector.particleSystems;
    }

    private PropertyField CreateProperty(SerializedObject so, string propertyPath, string label = "")
    {
        PropertyField field = new(so.FindProperty(propertyPath), label);
        field.Bind(so);
        _properties.Add(field);
        return field;
    }

    private Button CreateButton(string text, string name, Action clicked, bool enabled = true)
    {
        Button button = new()
        {
            name = name,
            text = text
        };
        button.SetEnabled(enabled);
        button.clicked += clicked;
        _propertyButtons.Add(button);
        return button;
    }

    private Toggle CreateToggle(string text, string key, bool enabled = false)
    {
        Toggle toggle = new(text);
        toggle.value = enabled;
        _propertyToggles.Add(key, toggle);
        return toggle;
    }

    private Label CreateLabel(string text)
    {
        Label label = new (text);
        _propertyLabels.Add(label);
        return label;
    }

    private TextField CreatePathFields(string label, string assetDirKey, string assetDir)
    {
        string packageName = soPackageName.value.Equals("") ? "" : $"{soPackageName.value}/";
        TextField assetPath = new(label)
        {
            value = $"{assetDir}{packageName}{SceneManager.GetActiveScene().name}.asset"
        };
        _pathFields.Add(assetDirKey, assetPath);
        return assetPath;
    }

    private (int spawnedAtIndex, int indexPos) FindIndex(List<int> indexes, int slideIndex)
    {
        return SceneDirector.FindIndex(indexes, slideIndex);
    }
}
