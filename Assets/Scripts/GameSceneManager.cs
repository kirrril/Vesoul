using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    private CityDatabase cityDataBase;

    [SerializeField]
    private UIgameScene uiGameScene;

    private string cityToGuess;

    public List<string> alreadyPlayed = new List<string>();

    private List<string> notYetGuessedCities = new List<string>();

    private int remainingFuel = 7;

    private string cityDisplayVersion = string.Empty;

    private string inputFirstLetter;

    public event Action<string> ShowCityToGuess;
    public event Action<List<string>> AlreadyPlayedUpdate;
    public event Action<int> FuelUpdate;
    public event Action<string> CommentUpdate;

    void Awake()
    {
        uiGameScene.PlayerMadeInput += DisplayCityToGuess;
        uiGameScene.PlayerMadeInput += UpdateFuelLevel;
        uiGameScene.PlayerMadeInput += UpdateComment;
        uiGameScene.PlayerMadeInput += UpdateAlreadyPlayed;
        uiGameScene.PlayerMadeInput += GameLogic;
    }

    void Start()
    {
        FindNotYetGuessed();
        ChooseCityToGuess();
        DisplayCityFirstTime();
        AlreadyPlayedUpdate?.Invoke(alreadyPlayed);
        CommentUpdate?.Invoke("ENTREZ UNE LETTRE\nOU LE NOM DE LA VILLE EN ENTIER");
        FuelUpdate?.Invoke(remainingFuel);

        // Debug.Log(Player.Instance.currentPlayer.playerEmail);
        Debug.Log(Player.Instance.currentPlayer.experience);

        foreach (string city in Player.Instance.currentPlayer.visitedCities)
        {
            Debug.Log(city);
        }
    }

    private void FindNotYetGuessed()
    {
        foreach (CityData city in cityDataBase.cities)
        {
            if (!Player.Instance.currentPlayer.visitedCities.Contains(city.cityName.ToUpper()))
            {
                notYetGuessedCities.Add(city.cityName);
            }
        }
    }

    private void ChooseCityToGuess()
    {
        int cityIndex = 400;

        while (cityIndex >= notYetGuessedCities.Count)
        {
            cityIndex = UnityEngine.Random.Range(0, 3);
        }

        cityToGuess = notYetGuessedCities[cityIndex];
    }

    private bool CheckInput(string input)
    {
        string pattern = @"^[\p{L}-']+$";
        return Regex.IsMatch(input, pattern);
    }

    private void DisplayCityToGuess(string input)
    {
        if (CheckInput(input))
        {
            cityDisplayVersion = string.Empty;

            if (input.ToUpper() != cityToGuess.ToUpper())
            {
                string inputFirstLetter = input.Substring(0, 1).ToUpper();

                foreach (char letter in cityToGuess.ToUpper())
                {
                    if (letter.ToString() != input.ToUpper() && !alreadyPlayed.Contains(letter.ToString().ToUpper()))
                    {
                        cityDisplayVersion += "_";
                    }
                    else if (letter.ToString() == input.ToUpper() || alreadyPlayed.Contains(letter.ToString().ToUpper()))
                    {
                        cityDisplayVersion += letter.ToString();
                    }
                }
            }
            else
            {
                cityDisplayVersion = cityToGuess.ToUpper();
            }

            ShowCityToGuess?.Invoke(cityDisplayVersion);
        }
        else
        {
            return;
        }
    }

    private void DisplayCityFirstTime()
    {
        cityDisplayVersion = string.Empty;

        foreach (char letter in cityToGuess.ToUpper())
        {
            cityDisplayVersion += "_";
        }
        ShowCityToGuess?.Invoke(cityDisplayVersion);
    }

    private void UpdateAlreadyPlayed(string input)
    {
        if (CheckInput(input))
        {
            if (input.ToUpper() == cityToGuess.ToUpper())
            {
                foreach (char letter in input)
                {
                    if (!alreadyPlayed.Contains(letter.ToString().ToUpper()))
                    {
                        alreadyPlayed.Add(letter.ToString().ToUpper());
                    }
                }
            }
            else
            {
                string inputFirstLetter = input.Substring(0, 1).ToUpper();

                if (!alreadyPlayed.Contains(inputFirstLetter))
                {
                    alreadyPlayed.Add(inputFirstLetter);
                }
            }

            AlreadyPlayedUpdate?.Invoke(alreadyPlayed);
        }
        else
        {
            return;
        }
    }

    private void UpdateFuelLevel(string input)
    {
        if (CheckInput(input))
        {
            string inputFirstLetter = input.Substring(0, 1).ToUpper();

            if (input.ToUpper() != cityToGuess.ToUpper() && !alreadyPlayed.Contains(inputFirstLetter))
            {
                remainingFuel--;
            }

            FuelUpdate?.Invoke(remainingFuel);
        }
        else
        {
            return;
        }
    }

    private void GameLogic(string input)
    {
        if (CheckInput(input))
        {
            if (cityDisplayVersion == cityToGuess.ToUpper() || input.ToUpper() == cityToGuess.ToUpper())
            {
                Player.Instance.AddVisitedCity(cityToGuess.ToUpper());
                SceneManager.LoadScene("YouWinScene");
            }
            else if (cityDisplayVersion != cityToGuess && remainingFuel < 1)
            {
                SceneManager.LoadScene("YouLoseScene");
            }
        }
        else
        {
            return;
        }
    }

    private void UpdateComment(string input)
    {
        string commentText = string.Empty;

        if (CheckInput(input))
        {
            if (input.ToUpper() == cityToGuess.ToUpper())
            {
                commentText = $"BRAVO ! VOUS AVEZ DEVINÉ LA VILLE !";
            }
            else
            {
                inputFirstLetter = input.Substring(0, 1).ToUpper();

                if (alreadyPlayed.Contains(inputFirstLetter))
                {
                    commentText = $"LA LETTRE {inputFirstLetter} A DEJA ETE JOUEE";
                }
                else
                {
                    if (cityToGuess.ToUpper().Contains(inputFirstLetter))
                    {
                        commentText = $"BRAVO ! VOUS AVEZ DEVINÉ LA LETTRE {inputFirstLetter} !";
                    }
                    else
                    {
                        commentText = $"ÉCHEC !\nIL N'Y A PAS DE {inputFirstLetter.ToUpper()}\nDANS LE NOM DE CETTE VILLE ...";
                    }
                }
            }
        }
        else
        {
            commentText = $"ENTRÉE INCORRECTE.\nTAPEZ UNE LETTRE,\nUN TRAIT D'UNION\nOU UN APOSTROPHE";
        }

        CommentUpdate?.Invoke(commentText);
    }
}
