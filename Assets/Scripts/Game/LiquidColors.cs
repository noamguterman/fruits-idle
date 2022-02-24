using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidColors : MonoBehaviour
{
    public static LiquidColors Instance;
    public Color[] fruitColors;
    public string[] FruitName = new string[15]
    {
        "Apple",
        "Pineapple",
        "Banana",
        "Mango",
        "Strawberry",
        "Pomegranate",
        "Mini Watermelon",
        "Kiwi",
        "Orange",
        "Coconut",
        "Pear",
        "Carrot",
        "Gold Apple",
        "Gold Pineapple",
        "Diamond Berry"
    };

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public Color GetLiquidColor(string pieceName)
    {
        //for(int i = 0; i < FruitName.Length; i++)
        //{
        //    if (pieceName.Contains(FruitName[i]))
        //    {
        //        return fruitColors[i];
        //    }
        //}
        int colIdx = 0;

        if (pieceName.Contains("Apple"))
            colIdx = 6;
        else if (pieceName.Contains("Gold Pineapple") || pieceName.Contains("Mini Pineapple"))
            colIdx = 5;
        else if (pieceName.Contains("Pineapple") || pieceName.Contains("Rich Pineapple"))
            colIdx = 2;
        else if (pieceName.Contains("Banana"))
            colIdx = 3;
        else if (pieceName.Contains("Mango"))
            colIdx = 4;
        else if (pieceName.Contains("Pear"))
            colIdx = 6;
        else if (pieceName.Contains("Kiwi"))
            colIdx = 7;
        else if (pieceName.Contains("Watermelon"))
            colIdx = 6;
        else if (pieceName.Contains("Pomegranate"))
            colIdx = 1;
        else if (pieceName.Contains("Carrot"))
            colIdx = 0;
        else if (pieceName.Contains("Coconut"))
            colIdx = 5;
        else if (pieceName.Contains("Strawberry"))
            colIdx = 0;
        else if (pieceName.Contains("Orange"))
            colIdx = 2;
        else if (pieceName.Contains("Gold Apple"))
            colIdx = 4;
        else if (pieceName.Contains("Diamond Berry"))
            colIdx = 1;
        

        return fruitColors[colIdx];
    }
}
