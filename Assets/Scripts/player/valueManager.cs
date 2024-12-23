using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class valueManager : MonoBehaviour
{
    public Scrollbar healthScrollbar;

    public Slider energySlider;

    private float[] ScrollSizes = { 0f, 0.164f, 0.346f, 0.5f, 0.65f, 0.832f, 1f };
    private float[] SliderSizes = { 0f, 0.9f, 1.9f, 2.9f, 4f, 4.9f, 6f, 7f, 8.1f, 9.1f, 10f };

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("Hp"))
        {
            healthScrollbar = GameObject.FindGameObjectWithTag("Hp").GetComponent<Scrollbar>();
        }
        if (GameObject.FindGameObjectWithTag("Energy"))
        {
            energySlider = GameObject.FindGameObjectWithTag("Energy").GetComponent<Slider>();
        }

        if ((int)playerControlSystem.Instance.healthPoint <= 0)
        {
            healthScrollbar.size = 0;
        }
        else
        {
            healthScrollbar.size = ScrollSizes[(int)playerControlSystem.Instance.healthPoint * 2];
        }

        if ((int)playerControlSystem.Instance.energyPoint <= 0)
        {
            energySlider.value = 0;
        }
        else
        {
            energySlider.value = SliderSizes[(int)playerControlSystem.Instance.energyPoint];
        }
    }
}