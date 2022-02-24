using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddLevel()
    {
        GameData.SetLevel(GameData.CurrentLevel + 5);
    }

    public void AddMoney()
    {
        GameData.SetMoney(GameData.GetMoney(MoneyType.Money) + 1000000000, MoneyType.Money, true);
    }
}
