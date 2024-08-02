using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestCppReturnValue : MonoBehaviour
{
    [DllImport("BoolArrayDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetBoolArray(out int rows, out int cols);

    void Start()
    {
        int rows, cols;
        IntPtr arrPtr = GetBoolArray(out rows, out cols);
        bool[][] arr = new bool[rows][];

        // Marshal the array of pointers (which point to the arrays of bools)
        IntPtr[] ptrArray = new IntPtr[rows];
        Marshal.Copy(arrPtr, ptrArray, 0, rows);

        for (int i = 0; i < rows; i++)
        {
            arr[i] = new bool[cols];
            // Now copy the bool values for each row
            IntPtr rowPtr = ptrArray[i];
            byte[] boolBytes = new byte[cols];
            Marshal.Copy(rowPtr, boolBytes, 0, cols);

            for (int j = 0; j < cols; j++)
            {
                arr[i][j] = boolBytes[j] != 0;
            }
        }

        // Print to check the values
        for (int i = 0; i < arr.Length; i++)
        {
            for (int j = 0; j < arr[i].Length; j++)
            {
                Debug.Log(arr[i][j] + " ");
            }
        }
    }
}