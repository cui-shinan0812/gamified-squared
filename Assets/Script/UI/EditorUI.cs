using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class EditorUI : MonoBehaviour   
{
    [Header("Buttons")]
    [SerializeField] private Button getArrayFromFileButton;
    [SerializeField] private Button turnGreenButton;
    [SerializeField] private Button turnBlueButton;
    [SerializeField] private Button turnRedButton;
    [SerializeField] private Button emptyButton;
    [SerializeField] private Button getArrayButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button previousFrameButton;
    [SerializeField] private Button nextFrameButton;
    [SerializeField] private Button testButton;
    // [SerializeField] private Button applyTimeSettingButton;
    // [SerializeField] private Button liveStagePref1Button;
    // [SerializeField] private Button liveStagePref2Button;
    // [SerializeField] private Button liveStagePref3Button;
    [SerializeField] private Button testReadMetaDataButton;

    [Header("Drop Downs")] 
    [SerializeField] private TMP_Dropdown framePerSecDropdown;
    
    [Header("Toggle Groups")]
    [SerializeField] private ToggleGroup dynamicToggleGroup;
    
    [Header("Game Objects")]
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject dynamicGridSubPanel;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI framePagesText;
    // [SerializeField] private TextMeshProUGUI timeLineText;
    // [SerializeField] private TextMeshProUGUI fileName1Text;
    // [SerializeField] private TextMeshProUGUI fileName2Text;
    // [SerializeField] private TextMeshProUGUI fileName3Text;
    [SerializeField] private TextMeshProUGUI mapNameValueText;
    [SerializeField] private TextMeshProUGUI mapCategoryValueText;
    [SerializeField] private TextMeshProUGUI mapDifficultyValueText;

    [Header("Sliders")]
    [SerializeField] private Slider timeLineSlider;

    private Toggle[] toggles;
    
    [DllImport("testingLibrary")]
    private static extern int getA();
    [DllImport("testingLibrary")]
    private static extern void setA(int new_a);
    
    private int previousSliderValue;
    // private int framePerSec;
    private int settingMinutes;
    private int settingSeconds;
    private int gameplayTimeInSeconds;
    
    private void Start()
    {
        framePerSecDropdown.value = 0;
        settingSeconds = 0;
        settingMinutes = 1;
        // framePerSec = 1;
        gameplayTimeInSeconds = settingMinutes * 60 + settingSeconds;
        // timeLineText.text = "00 : 00";
        framePagesText.text = "0 / 0";
        previousSliderValue = 0;
        testReadMetaDataButton.onClick.AddListener(() =>
        {
            MetaDataManager.Instance.ExtractData(out var map3DArray, out var mapName, out var mapCategory, out var mapDifficulty);

            if (mapName == null || mapName.Length <= 0)
            {
                mapNameValueText.text = "Undefined";
            }
            else
            {
                mapNameValueText.text = mapName;
            }
            
            if (mapCategory == null || mapCategory.Length <= 0)
            {
                mapCategoryValueText.text = "Undefined";
            }
            else
            {
                mapCategoryValueText.text = mapCategory;
            }
            
            mapDifficultyValueText.text = "" + mapDifficulty;

            if (map3DArray.Length > 0)
            {
                DestroyAllChildren(dynamicGridSubPanel);
                PixelManager.Instance.ExtractPixelByLocalVariable(map3DArray);
                setGridPanel();
                if (toggles.Length > 0)
                {
                    PixelManager.Instance.UpdateAllPixelsDynamic(toggles);
                }
                framePagesText.text = "Key Frame: " + (PixelManager.Instance.GetEditingFrameIndex() + 1) +" / " + PixelManager.Instance.GetMaxFrame();
                timeLineSlider.value = PixelManager.Instance.GetEditingFrameIndex();
                timeLineSlider.maxValue = PixelManager.Instance.GetMaxFrame()-1;   
            }
        });
        timeLineSlider.onValueChanged.AddListener((value) =>
        {
            if (PixelManager.Instance.GetMaxFrame() > 0)
            {
                Debug.Log("value: " + value);
                Debug.Log("previousSliderValue: " + previousSliderValue);
                PixelManager.Instance.MoveToTheFrame((int)value);
                previousSliderValue = (int)value;
                DestroyAllChildren(dynamicGridSubPanel);
                setGridPanel();
                if (toggles.Length > 0)
                {
                    PixelManager.Instance.UpdateAllPixelsDynamic(toggles);
                }
                framePagesText.text = "Key Frames: " + (PixelManager.Instance.GetEditingFrameIndex() + 1) +" / " + PixelManager.Instance.GetMaxFrame();
                timeLineSlider.value = PixelManager.Instance.GetEditingFrameIndex();
            }
            else
            {
                timeLineSlider.value = 0;
            }
        });
        testButton.onClick.AddListener(() =>
        {
            FileManager.Instance.ReadTxtFile();
        });
        nextFrameButton.onClick.AddListener(() =>
        {
            if (PixelManager.Instance.GetMaxFrame() > 0)
            {
                PixelManager.Instance.MoveToNextFrame();
                DestroyAllChildren(dynamicGridSubPanel);
                setGridPanel();
                if (toggles.Length > 0)
                {
                    PixelManager.Instance.UpdateAllPixelsDynamic(toggles);
                }
                framePagesText.text = "Frame: " + (PixelManager.Instance.GetEditingFrameIndex() + 1) +" / " + PixelManager.Instance.GetMaxFrame();
                timeLineSlider.value = PixelManager.Instance.GetEditingFrameIndex();
            }
        });
        previousFrameButton.onClick.AddListener(() =>
        {
            if (PixelManager.Instance.GetMaxFrame() > 0)
            {
                PixelManager.Instance.MoveToPreviousFrame();
                DestroyAllChildren(dynamicGridSubPanel);
                setGridPanel();
                if (toggles.Length > 0)
                {
                    PixelManager.Instance.UpdateAllPixelsDynamic(toggles);
                }
                framePagesText.text = "Frame: " + (PixelManager.Instance.GetEditingFrameIndex() + 1) +" / " + PixelManager.Instance.GetMaxFrame();
                timeLineSlider.value = PixelManager.Instance.GetEditingFrameIndex();
            }
        });
        resetButton.onClick.AddListener(() =>
        {
            PixelManager.Instance.ResetEditedArray();
            DestroyAllChildren(dynamicGridSubPanel);
            setGridPanel();
            if (toggles.Length > 0)
            {
                PixelManager.Instance.UpdateAllPixelsDynamic(toggles);
            }
        });
        saveButton.onClick.AddListener(() =>
        {
            PixelManager.Instance.SaveEditedArray();
        });
        getArrayFromFileButton.onClick.AddListener(() =>
        {
            // Debug.Log(getA());
            // setA(10);
            // Debug.Log(getA());
            DestroyAllChildren(dynamicGridSubPanel);
            PixelManager.Instance.ExtractPixelByFile();
            setGridPanel();
            if (toggles.Length > 0)
            {
                PixelManager.Instance.UpdateAllPixelsDynamic(toggles);
            }
            framePagesText.text = "Key Frame: " + (PixelManager.Instance.GetEditingFrameIndex() + 1) +" / " + PixelManager.Instance.GetMaxFrame();
            timeLineSlider.value = PixelManager.Instance.GetEditingFrameIndex();
            timeLineSlider.maxValue = PixelManager.Instance.GetMaxFrame()-1;
        });
        getArrayButton.onClick.AddListener(() =>
        {
            DestroyAllChildren(dynamicGridSubPanel);
            PixelManager.Instance.ExtractPixel();
            setGridPanel();
            if (toggles.Length > 0)
            {
                PixelManager.Instance.UpdateAllPixelsDynamic(toggles);
            }
        });
        
        turnGreenButton.onClick.AddListener(() =>
        {
            PixelManager.Instance.SetColorOfOneGrid("GREEN", toggles);
        });
        
        turnRedButton.onClick.AddListener(() =>
        {
            PixelManager.Instance.SetColorOfOneGrid("RED", toggles);
        });
        
        turnBlueButton.onClick.AddListener(() =>
        {
            PixelManager.Instance.SetColorOfOneGrid("BLUE", toggles);
        });
        
        emptyButton.onClick.AddListener(() =>
        {
            PixelManager.Instance.SetColorOfOneGrid("EMPTY", toggles);
        });
        
        returnButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetTimeLineText()
    {
        string minuteText = $"{settingMinutes: 00}";
        string secondText = $"{settingSeconds: 00}";

        Debug.Log(minuteText + " : " + secondText);
        // timeLineText.text = minuteText + " : " + secondText;
    }
    
    private static void DestroyAllChildren(GameObject go)
    {
        foreach (Transform transform in go. transform)
        {
            Destroy(transform. gameObject);
        }
    }
    
    private void setGridPanel()
    {
        int width = PixelManager.Instance.GetWidth();
        int height = PixelManager.Instance.GetHeight();
        
        // dynamicGridSubPanel.GetComponent<FlexibleGridLayout>().

        toggles = new Toggle[width * height];
        
        for (int i = 0; i < width * height; i++)
        {
            toggles[i] = Instantiate(gridPrefab, dynamicGridSubPanel.transform.position, dynamicGridSubPanel.transform.rotation,
                dynamicGridSubPanel.transform).GetComponent<Toggle>();

            toggles[i].group = dynamicToggleGroup.GetComponent<ToggleGroup>();
            
            PixelManager.Instance.AddButtonListener(toggles[i], i);

        }   
    }

    private void NotifyReceiver()
    {
        
    }
}
