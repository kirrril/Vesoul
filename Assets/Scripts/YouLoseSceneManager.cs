using UnityEngine;
using System;

public class YouLoseSceneManager : MonoBehaviour
{
    [SerializeField]
    private CityDatabase cityDataBase;

    [SerializeField]
    private UIyouLoseScene uiYouLoseScene;
    private Texture2D nowhereImage;

    public event Action<Texture2D> getNowhereView;

    void Start()
    {
        getNowhereView?.Invoke(GetNowhereImage());
    }

    private Texture2D GetNowhereImage()
    {
        foreach (CityData city in cityDataBase.cities)
        {
            if (city.cityName == "Nullepart")
            {
                return nowhereImage = city.cityImage;
            }
        }
        return null;
    }
}
