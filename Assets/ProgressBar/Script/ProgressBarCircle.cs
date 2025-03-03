using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressBarCircle : MonoBehaviour
{
    [Header("Title Setting")]
    public string Title;
    public Color TitleColor;
    public Font TitleFont;

    [Header("Bar Setting")]
    public Color BarColor;
    public Color BarBackGroundColor;
    public Color MaskColor;
    public Sprite BarBackGroundSprite;
    [Range(1f, 100f)]
    public int Alert = 20;
    public Color BarAlertColor;

    [Header("Sound Alert")]
    public AudioClip sound;
    public bool repeat = false;
    public float RepearRate = 1f;

    private Image bar, barBackground, Mask;
    private float nextPlay;
    private AudioSource audiosource;
    private Text txtTitle;
    private float barValue;
    private bool isProgressing = false; // Prevents multiple triggers

    public float BarValue
    {
        get { return barValue; }
        set
        {
            value = Mathf.Clamp(value, 0, 100);
            barValue = value;
            UpdateValue(barValue);
        }
    }

    private void Awake()
    {
        txtTitle = transform.Find("Text").GetComponent<Text>();
        barBackground = transform.Find("BarBackgroundCircle").GetComponent<Image>();
        bar = transform.Find("BarCircle").GetComponent<Image>();
        audiosource = GetComponent<AudioSource>();
        Mask = transform.Find("Mask").GetComponent<Image>();
    }

    private void Start()
    {
        txtTitle.text = Title;
        txtTitle.color = TitleColor;
        txtTitle.font = TitleFont;

        bar.color = BarColor;
        Mask.color = MaskColor;
        barBackground.color = BarBackGroundColor;
        barBackground.sprite = BarBackGroundSprite;

        // 🔹 Automatically start the progress when scene starts
        StartCoroutine(StartProgress());
    }

    void UpdateValue(float val)
    {
        bar.fillAmount = -(val / 100) + 1f;
        txtTitle.text = Title + " " + val + "%";

        if (Alert >= val)
        {
            barBackground.color = BarAlertColor;
        }
        else
        {
            barBackground.color = BarBackGroundColor;
        }
    }

    IEnumerator StartProgress()
    {
        float progress = 0;
        isProgressing = true;

        while (progress <= 100)
        {
            BarValue = progress;
            progress += Time.deltaTime * 10; // Adjust speed as needed
            yield return null;
        }

        isProgressing = false;
    }

    // 🔹 OPTIONAL: Trigger progress with a collider
    private void OnTriggerEnter(Collider other)
    {
        if (!isProgressing) // Ensure progress starts only once
        {
            StartCoroutine(StartProgress());
        }
    }
}
