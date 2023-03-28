using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionUI;

    [Serializable]
    public class ShopRole
    {
        public GameObject objectsPanel;
        public GameObject objectsInfosUI;
        public Transform objectsParent;
        public ShopType shopRole;
        public bool isRecipe;
        public List<ItemToBuy> items;
        public List<RecipeToBuy> recipes;
    }

    [SerializeField] private GameObject shopUI;
    [SerializeField] private List<ShopRole> shop;

    public GameObject InteractionUI
    {
        get { return interactionUI; }
        set { interactionUI = value; }
    }

    private PlayerController playerController;
    private PlayerInput playerInput;
    private List_Slots listSlots;

    private string interaction;

    [Header("Shop States")]
    [SerializeField] private bool canUseShop = false;
    [SerializeField] private bool shopInUse = false;

    public bool CanUseShop
    {
        get { return canUseShop; }
        set { canUseShop = value; }
    }

    [Serializable]
    public class ItemToBuy
    {
        public Item item;
        public int price;
    }

    [Serializable]
    public class RecipeToBuy
    {
        public Recipe item;
        public int price;
    }

    public enum ShopType
    { 
        FruitSeedsSeller,
        ItemDealer,
        AnimalPenManager
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerInput = FindObjectOfType<PlayerInput>();
        listSlots = FindObjectOfType<List_Slots>();

        interaction = "Use " + playerInput.InteractionAction.GetBindingDisplayString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            listSlots.UpdateMoney(1000);
        }

        HandleShopUI();
    }

    #region Shop UI

    private void HandleShopUI()
    {
        if (shopUI != null)
            shopUI.SetActive(shopInUse);

        if (!canUseShop)
        {
            CloseShopUI();
            return;
        }

        if (playerInput.InteractionAction.triggered && canUseShop)
        {
            if (!shopInUse)
            {
                OpenShopUI();
            }
            else
            {
                CloseShopUI();
            }
        }
    }

    private void OpenShopUI()
    {
        shopInUse = true;

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} to close shop";

        MinigameManager.AddOpenInventory(shopUI);
    }

    private void CloseShopUI()
    {
        shopInUse = false;

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} to open shop";

        MinigameManager.RemoveOpenInventory(shopUI);

        Clear();
    }

    #region Shop "Fruit Seeds Seller"

    public void ShowFruitSeedsUI()
    {
        int index = 0;
        Show(index);
    }

    #endregion


    #region Shop "Item Dealer"

    public void ShowItemsUI()
    {
        int index = 0;
        Show(index);
    }

    public void ShowRecipesUI()
    {
        int index = 1;
        Show(index);
    }

    private void BuyItem(ItemToBuy itemToBuy, InputField inputField)
    {
        int amount = int.Parse(inputField.text);

        if (amount == 0) return;

        if (playerController.Money >= (amount * itemToBuy.price))
        {
            if (amount <= itemToBuy.item.maxStackSize)
            {
                CreateItemUI(itemToBuy, amount);
            }
            else
            {
                int nbItemsToCreate = amount / itemToBuy.item.maxStackSize;

                for (int i = 0; i < nbItemsToCreate; i++)
                {
                    CreateItemUI(itemToBuy, itemToBuy.item.maxStackSize);
                }

                int lastQuantity = amount - (nbItemsToCreate * itemToBuy.item.maxStackSize);

                if (lastQuantity == 0) return;

                CreateItemUI(itemToBuy, lastQuantity);
            }
        }
    }

    private void BuyRecipe(RecipeToBuy recipeToBuy)
    {
        if (playerController.Money >= recipeToBuy.price)
        {
            listSlots.PRInventory.AddRecipeToInventory(recipeToBuy.item);

            listSlots.UpdateMoney(playerController.Money - recipeToBuy.price);
        }
    }

    #endregion

    #region Shop "Animal Pen Manager"

    private void HandleAnimalPenManager()
    {
        Debug.Log($"Animal Pen Manager");
    }

    #endregion

    private void Show(int index)
    {
        IList shopList;

        if (!shop[index].isRecipe) shopList = shop[index].items;
        else shopList = shop[index].recipes;

        if (shopList == null) return;

        Clear();

        shop[index].objectsPanel.SetActive(true);

        shop[index].objectsParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < shopList.Count; i++)
        {
            ShowObjectUI(shop[index].objectsParent, shop[index].objectsInfosUI , shopList[i], shop[index].isRecipe);
        }
    }

    private void ShowObjectUI(Transform parent, GameObject objectInfoUI, object objectToBuy, bool isRecipe)
    {
        Transform item = Instantiate(objectInfoUI, parent).transform;

        Sprite objectSprite;
        string objectName;
        float objectPrice;

        if (!isRecipe)
        {
            ItemToBuy itemToBuy = (ItemToBuy)objectToBuy;

            objectSprite = itemToBuy.item.itemSprite;
            objectName = itemToBuy.item.itemName;
            objectPrice = itemToBuy.price;
        }
        else
        {
            RecipeToBuy recipeToBuy = (RecipeToBuy)objectToBuy;

            objectSprite = recipeToBuy.item.recipeSprite;
            objectName = recipeToBuy.item.recipeName;
            objectPrice = recipeToBuy.price;
        }

        item.GetChild(0).GetComponent<Image>().sprite = objectSprite;
        item.GetChild(1).GetComponent<Text>().text = objectName;
        item.GetChild(2).GetComponent<Text>().text = $"{objectPrice} P";

        if (!isRecipe)
        {
            InputField inputField = item.GetChild(3).GetComponent<InputField>();
            inputField.text = $"{0}";

            item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { BuyItem((ItemToBuy)objectToBuy, inputField); });
        }
        else
        {
            item.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BuyRecipe((RecipeToBuy)objectToBuy); });
        }
    }

    private void Clear()
    {
        for (int i = 0; i < shop.Count; i++)
        {
            if (shop[i].objectsPanel != null)
            {
                shop[i].objectsPanel.SetActive(false);

                for (int j = 0; j < shop[i].objectsParent.childCount; j++)
                {
                    Destroy(shop[i].objectsParent.GetChild(j).gameObject);
                }
            }
        }
    }

    private void CreateItemUI(ItemToBuy itemToBuy, int quantity)
    {
        GameObject itemUI = listSlots.PlayerSlotsParent.GetComponent<PlayerInventory>().CreateItemUI();

        itemUI.GetComponent<DraggableItem>().quantityStacked = quantity;

        itemUI.GetComponent<DraggableItem>().Item = itemToBuy.item;

        listSlots.UpdateMoney(playerController.Money - itemToBuy.price);
    }

    #endregion
}