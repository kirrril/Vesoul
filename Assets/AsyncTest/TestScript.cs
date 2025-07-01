using System.Threading.Tasks;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    async void Start()
    {
        Debug.Log("Previous step");

        await LoopTrough();

        Debug.Log("Next step");
    }

    async Task LoopTrough()
    {
        Debug.Log("Start counter");

        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(1000);
            Debug.Log(i);
        }

        Debug.Log("Stop counter");
    }
}
