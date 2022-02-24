using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Game.Audio;
using UI.ListItems;
using Game.Pieces;
using Game;
using Utilities;
using Sirenix.OdinInspector;

public class VenderController : MonoBehaviour
{
    public SpriteRenderer[] juice_sprites;
    public GameObject[] level_bgs;
    public GameObject[] sliders;
    private bool[] slotStates = new bool[4];
    public Transform[] botte_SpwanPos;

    public Bottle[] bottlePrefab;

    public Color[] liquidColors;

    [SerializeField]
    [Required]
    [SceneObjectsOnly]
    private MoneyManager moneyManager;

    public Material currentMaterial;

    [SerializeField]
    [Required]
    private SoundSettings soundSettings;

    void Start()
    {
        //for (int i = 0; i < juice_sprites.Length; i++)
        //{
        //    juice_sprites[i].color = new Color(0.5f, 0.5f, 0.5f);
        //}
        for(int i = 0; i < slotStates.Length; i++)
        {
            slotStates[i] = false;
        }
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].transform.localScale = new Vector3(0, 1, 1);
        }

        SetLevels(GameData.CurrentLevel);
    }

    private void OnEnable()
    {
        GameData.LevelChanged += OnLevelChanged;
    }

    private void OnDisable()
    {
        GameData.LevelChanged -= OnLevelChanged;
    }

    private void OnLevelChanged(int level)
    {
        Debug.Log("VenderController : OnLevelChanged = " + level);
        SetLevels(level);
    }

    private void SetLevels(int level)
    {
        if (level < 40)
            return;
        ChangeJuiceSprites((level - 40) / 5);
        ChangeLevelBG((level - 40) / 5);
    }

    private void ChangeJuiceSprites(int showIndex)
    {
        return;
        showIndex = Mathf.Min(showIndex, juice_sprites.Length - 1);
        for(int i = 0; i < juice_sprites.Length; i++)
        {
            if(showIndex >= i)
            {
                juice_sprites[i].color = new Color(1, 1, 1);
            }
            else
            {
                juice_sprites[i].color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    private void ChangeLevelBG(int showIndex)
    {
        return;
        showIndex = Mathf.Min(showIndex, level_bgs.Length - 1);
        for (int i = 0; i < level_bgs.Length; i++)
        {
            if (showIndex >= i)
            {
                level_bgs[i].SetActive(false);
            }
            else
            {
                level_bgs[i].SetActive(true);
            }
        }
    }

    public void ReceiveLiquidFromTube(float money, float duration)
    {
        int slotNum = GetEmptySlot();
        if (slotNum == -1)
            return;

        StartCoroutine(MakeBottle(money, duration, slotNum));
    }

    private int GetEmptySlot()
    {
        int slotNum;
        for(int i = 0; i < 50; i++)
        {
            slotNum = Random.Range(0, 4);

            if(slotStates[slotNum] == false)
            {
                return slotNum;
            }
        }
        return -1;
    }

    IEnumerator MakeBottle(float money, float duration, int outputPos)
    {
        yield return new WaitForSeconds(1);

        slotStates[outputPos] = true;
        duration = Mathf.Min(0.3f, duration);
        sliders[outputPos].transform.DOScaleX(1, duration);

        yield return new WaitForSeconds(duration);

        sliders[outputPos].transform.localScale = new Vector3(0, 1, 1);
        slotStates[outputPos] = false;

        Bottle bottle = bottlePrefab[UnityEngine.Random.Range(0,4)].PullOrCreate<Bottle>();

        AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.BottleSpawn);
        if (audioSource != null)
        {
            audioSource.Play(false);
        }

        moneyManager.Subscribe(bottle);
        int idx = 0;

        //idx = UnityEngine.Random.Range(0, Mathf.Min((GameData.CurrentLevel - 40) / 5, juice_sprites.Length - 1));
        idx = UnityEngine.Random.Range(0, 4);
        bottle.Initialize(money, liquidColors[idx]);
        bottle.Position = botte_SpwanPos[outputPos].position;
        bottle.transform.localEulerAngles = new Vector3(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90));
        //bottle.Rotation = Quaternion.identity;
    }
}
