using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    [SerializeField] private ToggleGroup dynamicToggleGroup;

    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject dynamicGridSubPanel;

    private Toggle[] toggles;
    
    [DllImport("testingLibrary")]
    private static extern int getA();

    private static extern void setA(int new_a);
    
    // Start is called before the first frame update
    void Start()
    {
        // b.onClick.AddListener(() =>
        // {
        //     Debug.Log(getA());
        //     setA(10);
        //     Debug.Log(getA());
        // });
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
