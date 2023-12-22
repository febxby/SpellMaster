using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
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
public class GameManger : MonoSingleton<GameManger>
{
    // Start is called before the first frame update
    IOCContainer container;
    [SerializeField] InventoryModel inventoryModel;
    [SerializeField] GameObject initialWand;
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
    public UnityEngine.UI.Slider progressBar;



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

        DontDestroyOnLoad(gameObject);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PickUpable"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Wand"));


        container = IOCContainer.Instance;
        container.Register<InventoryModel>(inventoryModel);
        //仓库初始化
        inventoryModel.Init();
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


        wandHandle.Completed += OnWandLoadComplete;
        ObjectPoolFactory.Instance.Init<Spell>(10);
        ObjectPoolFactory.Instance.Init<Divide>(10);
        ObjectPoolFactory.Instance.Init<MultiCast>(10);
        ObjectPoolFactory.Instance.Init<Formation>(10);
        ObjectPoolFactory.Instance.Init<DivideModifier>(10);
    }

    private void OnWandLoadComplete(AsyncOperationHandle<IList<GameObject>> handle)
    {
        sceneLoadHandle = SceneManager.LoadSceneAsync("Demo");
        sceneLoadHandle.completed += OnSceneLoadComplete;
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
        Wand wand = Instantiate(initialWand).GetComponent<Wand>();
        MEventSystem.Instance.Send<AddWand>(new AddWand() { wand = wand });
    }
    private void OnDestroy()
    {
        Addressables.Release(wandHandle);
        inventoryModel.Clear();
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
}
