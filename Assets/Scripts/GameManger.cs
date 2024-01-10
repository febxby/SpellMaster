using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[System.Serializable]
public class AssetLoadData
{
    public string assetLabel;
    public System.Action<object> callback;
    public object[] dataArray;
}
public class LabelName
{
    public const string Spell = "Spell";
    public const string Wand = "Wand";
    public const string PlayerWands = "PlayerWands";
    public const string BossWands = "BossWands";
    public const string MobsWands = "MobsWands";
    public const string Mobs = "Mobs";
    public const string Boss = "Boss";
}
public struct SaveData
{
}
public class GameManger : MonoSingleton<GameManger>
{
    // Start is called before the first frame update
    IOCContainer container;
    [SerializeField] PlayerModel playerModel;
    [SerializeField] GameObject defaultWand;
    public List<Spell> allSpells = new();
    public List<GameObject> playerWands = new();
    public List<GameObject> bossWands = new();
    public List<GameObject> mobsWands = new();

    public List<GameObject> allEnemies = new();
    public List<GameObject> allBoss = new();
    Dictionary<string, List<object>> resources = new();
    public int spellCount => allSpells.Count;
    public int wandCount => playerWands.Count;
    AsyncOperationHandle<IList<GameObject>> wandHandle;
    AsyncOperation sceneLoadHandle;
    // 进度条UI
    List<GameObject> activePoolObjects = new List<GameObject>();

    public GameObject damageText;
    public Button startGame;
    public Button enterLab;
    public Button exitGame;
    public Image blackScreen;
    public Text loadingIcon;

    public float duration = 1f;
    private Tweener breatheTween;
    public GameObject loadingPage;
    GameObject page;
    private DamageText currentDamageText;
    public LevelController levelController;
    public GameObject templateWand;

    // // 计算总体进度的方法
    // float CalculateTotalProgress(float currentProgress, float newProgress)
    // {
    //     // 这里可以根据实际情况调整进度计算方式
    //     return currentProgress + (newProgress / assetLoadDataArray.Length);
    // }

    // // 更新进度条的方法
    // void UpdateProgressBar(float progress)
    // {
    //     // 确保进度条不越界
    //     progress = Mathf.Clamp01(progress);

    //     // 更新UI的进度条
    //     if (progressBar != null)
    //     {
    //         progressBar.value = progress;
    //     }
    // }
    private void Awake()
    {
        // SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive).completed += (e) =>
        {
            // blackScreen = GameObject.Find("BlackScreen").GetComponent<Image>();
            // loadingIcon = GameObject.Find("LoadingIcon").GetComponent<Text>();
            // blackScreen.gameObject.SetActive(false);
            // loadingIcon.gameObject.SetActive(false);
        };
        DontDestroyOnLoad(gameObject);
        // DontDestroyOnLoad(blackScreen.gameObject);
        // DontDestroyOnLoad(loadingIcon.gameObject);
        // blackScreen.gameObject.SetActive(false);
        // loadingIcon.gameObject.SetActive(false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PickUpable"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Wand"));


        container = IOCContainer.Instance;
        container.Register<PlayerModel>(playerModel);
        //仓库初始化
        Addressables.LoadAssetsAsync<GameObject>(LabelName.Mobs,
        addressable =>
        {
            allEnemies.Add(addressable);

        }, true).WaitForCompletion();

        Addressables.LoadAssetsAsync<GameObject>(LabelName.Boss,
        addressable =>
        {
            allBoss.Add(addressable);
        }, true).WaitForCompletion();

        //加载法术和法杖
        Addressables.LoadAssetsAsync<Spell>(LabelName.Spell,
        addressable =>
        {
            allSpells.Add(addressable);
        }, true).WaitForCompletion();

        Addressables.LoadAssetsAsync<GameObject>(LabelName.BossWands,
        addressable =>
        {
            bossWands.Add(addressable);
        }, true).WaitForCompletion();

        Addressables.LoadAssetsAsync<GameObject>(LabelName.MobsWands,
        addressable =>
        {
            mobsWands.Add(addressable);
        }, true).WaitForCompletion();

        wandHandle = Addressables.LoadAssetsAsync<GameObject>("PlayerWands",
        addressable =>
        {
            playerWands.Add(addressable);
        }, true);
        defaultWand = Addressables.LoadAssetAsync<GameObject>("DefaultWand").WaitForCompletion();

        wandHandle.Completed += OnWandLoadComplete;
        ObjectPoolFactory.Instance.Init<Spell>(10);
        ObjectPoolFactory.Instance.Init<Divide>(10);
        ObjectPoolFactory.Instance.Init<MultiCast>(10);
        ObjectPoolFactory.Instance.Init<Formation>(10);
        ObjectPoolFactory.Instance.Init<DivideModifier>(10);
    }
    public void LoadScene(string sceneName)
    {
        playerModel.Init();
        sceneLoadHandle = SceneManager.LoadSceneAsync(sceneName);
        sceneLoadHandle.completed += OnSceneLoadComplete;
    }
    // public void LoadSaveScene()
    // {
    //     // LoadData();
    //     sceneLoadHandle = SceneManager.LoadSceneAsync("Demo");
    //     sceneLoadHandle.completed += (e) =>
    //     {
    //         LoadData();
    //         levelController.Init(true);
    //         // Wand wand = playerModel.GetWand(0);
    //         // MEventSystem.Instance.Send<AddWand>(new AddWand() { wand = wand });
    //     };
    // }
    private void OnWandLoadComplete(AsyncOperationHandle<IList<GameObject>> handle)
    {
        startGame.gameObject.SetActive(true);
        enterLab.gameObject.SetActive(true);
        exitGame.gameObject.SetActive(true);
    }

    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     // if (isSpellLoadComplete && isWandLoadComplete)
    //     // {
    //     //     isSpellLoadComplete = false;
    //     //     isWandLoadComplete = false;
    //     //     //加载场景
    //     //     demoHandle = SceneManager.LoadSceneAsync("Demo");
    //     //     demoHandle.completed += OnSceneLoadComplete;
    //     // }
    // }
    void OnSceneLoadComplete(AsyncOperation handle)
    {
        //初始化法杖和法术
        // float num = Random.Range(0f, allSpells.Count);
        // inventoryModel.Add<Spell>(allSpells[(int)num]);
        // num = Random.Range(0f, allWands.Count);
        Wand wand = Instantiate(defaultWand).GetComponent<Wand>();
        MEventSystem.Instance.Send<AddWand>(new AddWand() { wand = wand });
        // levelController.Init(false);
    }
    private void OnDestroy()
    {
        Addressables.Release(wandHandle);
        playerModel.Clear();
    }
    public void LoadData()
    {
        PlayerModel data = SaveSystem.LoadFromJson<PlayerModel>("Player", playerModel);
        List<WandData> gameObjects = SaveSystem.LoadListFromJson<WandData>("Wands");
        if (data != null || playerModel.wands != null)
        {
            playerModel.Coin = data.Coin;
            playerModel.MaxHealth = data.MaxHealth;
            playerModel.CurrentHealth = data.CurrentHealth;
            playerModel.spells = data.spells;
            playerModel.nullSpellIndices = data.nullSpellIndices;
            playerModel.nullWandIndices = data.nullWandIndices;
            foreach (var wandData in gameObjects)
            {
                if (wandData.wandName != "")
                {
                    var wand = GameObjectPool.Instance.GetObject(templateWand).GetComponent<Wand>();
                    wand.SetWandData(wandData);
                    MEventSystem.Instance.Send<AddWand>(new AddWand() { wand = wand });
                }
            }

        }
    }
    public void DamageText(Vector3 position, int damage)
    {
        if (currentDamageText == null)
        {
            var damageTextObject = GameObjectPool.Instance.GetObject(damageText);
            damageTextObject.SetPositionAndRotation(position, Quaternion.identity);
            currentDamageText = damageTextObject.GetComponent<DamageText>();
            currentDamageText.Init(damage, () =>
            {
                currentDamageText = null;
            });
        }
        else
        {
            currentDamageText.UpdateDamage(damage);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 获取法术或者法杖
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public T Get<T>(int index) where T : class
    {
        var type = typeof(T);
        if (type == typeof(Spell))
        {
            if (index >= allSpells.Count || index < 0)
            {
                return null;
            }
            return allSpells[index] as T;
        }
        else if (type == typeof(Wand))
        {
            if (index >= playerWands.Count || index < 0)
            {
                return null;
            }
            return Instantiate(playerWands[index]).GetComponent<Wand>() as T;
        }
        return allSpells[index] as T;
    }
    public void EnterLoading()
    {
        page = GameObjectPool.Instance.GetObject(loadingPage);
    }
    public void ExitLoading()
    {
        GameObjectPool.Instance.PushObject(page);
    }
}
