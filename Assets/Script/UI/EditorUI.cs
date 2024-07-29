using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour   
{
    [SerializeField] private Button b;

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

    [SerializeField] private ToggleGroup dynamicToggleGroup;

    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject dynamicGridSubPanel;

    [SerializeField] private TextMeshProUGUI framePagesText;

    private Toggle[] toggles;
    
    [DllImport("testingLibrary")]
    private static extern int getA();
    [DllImport("testingLibrary")]
    private static extern void setA(int new_a);
    
    // Start is called before the first frame update
    void Start()
    {
        testButton.onClick.AddListener(() =>
        {
            FileManager.Instance.ReadTxtFile();
        });
        nextFrameButton.onClick.AddListener(() =>
        {
            PixelManager.Instance.MoveToNextFrame();
            DestroyAllChildren(dynamicGridSubPanel);
            setGridPanel();
            if (toggles.Length > 0)
            {
                PixelManager.Instance.UpdateAllPixelsDynamic(toggles);
            }
            framePagesText.text = "Frame: " + (PixelManager.Instance.GetEditingFrameIndex() + 1) +" / " + PixelManager.Instance.GetMaxFrame();
        });
        previousFrameButton.onClick.AddListener(() =>
        {
            PixelManager.Instance.MoveToPreviousFrame();
            DestroyAllChildren(dynamicGridSubPanel);
            setGridPanel();
            if (toggles.Length > 0)
            {
                PixelManager.Instance.UpdateAllPixelsDynamic(toggles);
            }
            framePagesText.text = "Frame: " + (PixelManager.Instance.GetEditingFrameIndex() + 1) +" / " + PixelManager.Instance.GetMaxFrame();
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
        b.onClick.AddListener(() =>
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
            framePagesText.text = "Frame: " + (PixelManager.Instance.GetEditingFrameIndex() + 1) +" / " + PixelManager.Instance.GetMaxFrame();
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
