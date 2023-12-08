using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class GameManger : MonoSingleton<GameManger>
{
    // Start is called before the first frame update
    IOCContainer container;
    [SerializeField] InventoryModel inventoryModel;
    [SerializeField] List<Spell> allSpells = new();
    [SerializeField] List<GameObject> allWands = new();
    public int spellCount => allSpells.Count;
    public int wandCount => allWands.Count;
    AsyncOperationHandle<IList<Spell>> spellHandle;
    AsyncOperationHandle<IList<GameObject>> wandHandle;
    bool isSpellLoadComplete;
    bool isWandLoadComplete;
    AsyncOperation demoHandle;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PickUpable"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Wand"));

        container = IOCContainer.Instance;
        container.Register<InventoryModel>(inventoryModel);
        //仓库初始化
        inventoryModel.Init();
        //加载法术和法杖
        spellHandle = Addressables.LoadAssetsAsync<Spell>("Spell",
        addressable =>
        {
            allSpells.Add(addressable);
        }, true);
        spellHandle.Completed += OnSpellLoadComplete;
        wandHandle = Addressables.LoadAssetsAsync<GameObject>("Wand",
        addressable =>
        {
            allWands.Add(addressable);
        }, true);
        wandHandle.Completed += OnWandLoadComplete;
        ObjectPoolFactory.Instance.Init<Spell>(10);
        ObjectPoolFactory.Instance.Init<Divide>(10);
        ObjectPoolFactory.Instance.Init<MultiCast>(10);
        ObjectPoolFactory.Instance.Init<Formation>(10);
        ObjectPoolFactory.Instance.Init<DivideModifier>(10);
    }

    private void OnSpellLoadComplete(AsyncOperationHandle<IList<Spell>> handle)
    {

        isSpellLoadComplete = true;
    }

    private void OnWandLoadComplete(AsyncOperationHandle<IList<GameObject>> handle)
    {


        isWandLoadComplete = true;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isSpellLoadComplete && isWandLoadComplete)
        {
            isSpellLoadComplete = false;
            isWandLoadComplete = false;
            //加载场景
            demoHandle = SceneManager.LoadSceneAsync("Demo");
            demoHandle.completed += OnSceneLoadComplete;
        }
    }
    void OnSceneLoadComplete(AsyncOperation handle)
    {
        //初始化法杖和法术
        float num = Random.Range(0f, allSpells.Count);
        inventoryModel.Add<Spell>(allSpells[(int)num]);
        num = Random.Range(0f, allWands.Count);
        Wand wand = Instantiate(allWands[(int)num]).GetComponent<Wand>();
        MEventSystem.Instance.Send<AddWand>(new AddWand() { wand = wand });
    }
    private void OnDestroy()
    {
        Addressables.Release(spellHandle);
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
            if (index >= allWands.Count || index < 0)
            {
                return null;
            }
            return Instantiate(allWands[index]).GetComponent<Wand>() as T;
        }
        return allSpells[index] as T;
    }
}
