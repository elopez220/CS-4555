// Balloon.cs - Fixed to work with new GameManager
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Balloon : MonoBehaviour
{
    [Header("Balloon Settings")]
    public int currencyValue = 10;
    public int ownerPlayerNumber = 1; // NEW: Which player shot this balloon
    public GameObject popEffect;
    public AudioClip popSound;

    private AudioSource audioSource;
    private bool isPopped = false;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) // FIXED: Was using = instead of ==
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PopBalloon()
    {
        if (isPopped)
        {
            return;
        }

        isPopped = true;
        
        if (popEffect != null)
        {
            Instantiate(popEffect, transform.position, Quaternion.identity);
        }
        
        AwardCurrency();
        Destroy(gameObject, 0.1f);
    }

    void AwardCurrency()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            // FIXED: Now uses new signature with player number
            gm.AddCurrency(ownerPlayerNumber, currencyValue);
        }
    }
    
    // Called when a dart hits - set which player gets credit
    public void SetOwner(int playerNum)
    {
        ownerPlayerNumber = playerNum;
    }
}