using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CityDatabase", menuName = "Game/City Database", order = 1)]
public class CityDatabase : ScriptableObject
{
    public List<CityData> cities = new List<CityData>();

}


[System.Serializable]
public class CityData
{
    public string cityName;
    public Texture2D cityImage;
    public bool alreadyVisited;
}
