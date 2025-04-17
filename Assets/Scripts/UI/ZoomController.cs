using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomController : MonoBehaviour
{
    public Image image;
    private GameObject Player;
    private float Energy;
    private float maxEnergy;
    private float _lerpSpeed = 3f;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        maxEnergy = Player.GetComponent<Player>().maxEnergy;
    }

    void Update()
    {
        if (image != null)
        {
            Energy = Player.GetComponent<Player>().Energy;
            image.fillAmount = Mathf.Lerp(image.fillAmount, Energy / maxEnergy, Time.deltaTime * _lerpSpeed);
        }

    }
}
