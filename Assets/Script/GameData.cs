using System.Collections;

using System.Collections.Generic;

using UnityEngine;



[CreateAssetMenu(menuName = "MyScriptable/Create GameData")]

public class GameData: ScriptableObject
{
    //獲得したチーズの数
    public int CheeseCount;
    //捕まった回数
    public int iscaughtTimes;
    //追尾を振り切った回数
    public int escapedTimes;
    //壁にぶつかった回数
    public int clashTimes;
}