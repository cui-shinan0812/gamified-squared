using System.Collections;
using UnityEngine;

public class DisplayFrames : MonoBehaviour
{
    private int[][][] resultArray;
    private int framesPerSecond = 12;

    private void Start()
    {
        // Assume resultArray is already initialized
        // StartCoroutine(DisplayResultArray());
    }

    private IEnumerator DisplayResultArray()
    {
        for (int i = 0; i < resultArray.Length; i++)
        {
            Debug.Log(ArrayToString(resultArray[i]));
            yield return new WaitForSeconds(1f / framesPerSecond);
        }
    }

    private string ArrayToString(int[][] array)
    {
        string result = "";
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array[i].Length; j++)
            {
                result += array[i][j] + " ";
            }
            result += "\n";
        }
        return result;
    }
}