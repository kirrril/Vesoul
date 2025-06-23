using System;
using UnityEngine;

public class YouWinSceneManager : MonoBehaviour
{
    [SerializeField]
    private CityDatabase cityDataBase;

    [SerializeField]
    private UIyouWinScene uiYouWinScene;

    private Texture2D arrivalCityImage;

    public event Action<Texture2D, string> getCityView;


    void Start()
    {
        getCityView?.Invoke(GetArrivalCityImage(), Player.Instance.lastVisitedCity.ToUpper());
    }

    private Texture2D GetArrivalCityImage()
    {
        foreach (CityData city in cityDataBase.cities)
        {
            if (city.cityName.ToUpper() == Player.Instance.lastVisitedCity.ToUpper())
            {
                return arrivalCityImage = city.cityImage;
            }
        }
        return null;
    }
}
