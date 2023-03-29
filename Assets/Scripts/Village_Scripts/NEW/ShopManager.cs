using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public bool isForSelling;
        public List<ItemToHandle> items;
        public List<RecipeToHandle> recipes;
    }

    [SerializeField] private GameObject shopUI;
    [SerializeField] private List<ShopRole> shopsRole;

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
    public class ItemToHandle
    {
        public Item item;
        public int price;
    }

    [Serializable]
    public class RecipeToHandle
    {
        public Recipe item;
        public int price;
    }

    public enum ShopType
    { 
        Seeds_Seller,
        Recipes_Dealer,
        Items_Dealer,
        Animal_Pen_Manager
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

    #region Handle Main Shop UI

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

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} to close shopsRole";

        MinigameManager.AddOpenInventory(shopUI);
    }

    private void CloseShopUI()
    {
        shopInUse = false;

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} to open shopsRole";

        MinigameManager.RemoveOpenInventory(shopUI);

        Clear();
    }

    #endregion

    public void ShowShopContent()
    {
        Show(GetCurrentShop());
    }

    private void BuyItem(ItemToHandle itemToBuy, InputField inputField)
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

    private void SellItem(ItemToHandle itemToSell, InputField inputField, Transform currentItemUI, ShopRole shopRole)
    {
        int amount = int.Parse(inputField.text);

        if (amount == 0) return;

        if (amount <= playerController.PlayerInventory.GetItemQuantity(itemToSell.item))
        {
            if (playerController.Money >= (amount * itemToSell.price))
            {
                int totalPrice = amount * itemToSell.price;
                int quantityLeft = playerController.PlayerInventory.GetItemQuantity(itemToSell.item) - amount;

                listSlots.UpdateMoney(playerController.Money + totalPrice);

                playerController.PlayerInventory.RemoveItemQuantity(itemToSell.item, amount);

                currentItemUI.GetChild(1).GetComponent<Text>().text = $"{itemToSell.item.itemName} x{quantityLeft}";

                if (quantityLeft <= 0)
                {
                    shopRole.items.Remove(itemToSell);

                    Destroy(currentItemUI.gameObject);
                }
            }
        }
    }

    private void BuyRecipe(RecipeToHandle recipeToBuy)
    {
        if (playerController.Money >= recipeToBuy.price)
        {
            listSlots.PRInventory.AddRecipeToInventory(recipeToBuy.item);

            listSlots.UpdateMoney(playerController.Money - recipeToBuy.price);
        }
    }

    private void Show(ShopRole currentShop)
    {
        IList shopList;
        
        if (!currentShop.isForSelling)
        {
            if (!currentShop.isRecipe) shopList = currentShop.items;
            else shopList = currentShop.recipes;
        }
        else
        {
            if (currentShop.items.Count > 0) currentShop.items.Clear();

            for (int i = 0; i < playerController.PlayerInventory.SearchItemsPossessed().Count; i++)
            {
                CreateItemToHandle(playerController.PlayerInventory.SearchItemsPossessed()[i].Item, (int)playerController.PlayerInventory.SearchItemsPossessed()[i].Item.itemValue, currentShop);
            }

            shopList = currentShop.items;
        }

        if (shopList == null) return;

        Clear();

        currentShop.objectsPanel.SetActive(true);

        currentShop.objectsParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < shopList.Count; i++)
        {
            ShowObjectUI(currentShop.objectsParent, currentShop.objectsInfosUI, shopList[i], currentShop.isRecipe, currentShop.isForSelling, currentShop);
        }
    }

    private ItemToHandle CreateItemToHandle(Item item, int price, ShopRole currentShop)
    {
        ItemToHandle itemToHandle = null;

        bool alreadyAdded = false;

        for (int i = 0; i < currentShop.items.Count; i++)
        {
            if (currentShop.items[i].item == item)
            {
                alreadyAdded = true;
            }
        }

        if (!alreadyAdded)
        {
            itemToHandle = new ItemToHandle();

            itemToHandle.item = item;
            itemToHandle.price = price;

            currentShop.items.Add(itemToHandle);
        }

        return itemToHandle;
    }

    private void ShowObjectUI(Transform parent, GameObject objectInfoUI, object objectToHandle, bool isRecipe, bool isForSelling, ShopRole shopRole)
    {
        Transform item = Instantiate(objectInfoUI, parent).transform;

        Sprite objectSprite;
        string objectName;
        float objectPrice;

        if (!isRecipe)
        {
            ItemToHandle itemToHandle = (ItemToHandle)objectToHandle;

            objectSprite = itemToHandle.item.itemSprite;

            if (!isForSelling) objectName = itemToHandle.item.itemName;
            else objectName = $"{itemToHandle.item.itemName} x{playerController.PlayerInventory.GetItemQuantity(itemToHandle.item)}";

            objectPrice = itemToHandle.price;        
        }
        else
        {
            RecipeToHandle recipeToHandle = (RecipeToHandle)objectToHandle;

            objectSprite = recipeToHandle.item.recipeSprite;
            objectName = recipeToHandle.item.recipeName;
            objectPrice = recipeToHandle.price;
        }

        item.GetChild(0).GetComponent<Image>().sprite = objectSprite;
        item.GetChild(1).GetComponent<Text>().text = objectName;
        item.GetChild(2).GetComponent<Text>().text = $"{objectPrice} P";

        if (!isRecipe)
        {
            InputField inputField = item.GetChild(3).GetComponent<InputField>();
            inputField.text = $"0";

            if (!isForSelling)
            {
                item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { BuyItem((ItemToHandle)objectToHandle, inputField); });
            }
            else
            {
                item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { SellItem((ItemToHandle)objectToHandle, inputField, item, shopRole); });
            }
        }
        else
        {
            item.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BuyRecipe((RecipeToHandle)objectToHandle); });
        }
    }

    private void Clear()
    {
        for (int i = 0; i < shopsRole.Count; i++)
        {
            if (shopsRole[i].objectsPanel != null)
            {
                shopsRole[i].objectsPanel.SetActive(false);

                for (int j = 0; j < shopsRole[i].objectsParent.childCount; j++)
                {
                    Destroy(shopsRole[i].objectsParent.GetChild(j).gameObject);
                }
            }
        }
    }

    private void CreateItemUI(ItemToHandle itemToBuy, int quantity)
    {
        GameObject itemUI = listSlots.PlayerSlotsParent.GetComponent<PlayerInventory>().CreateItemUI();

        itemUI.GetComponent<DraggableItem>().QuantityStacked = quantity;

        itemUI.GetComponent<DraggableItem>().Item = itemToBuy.item;

        listSlots.UpdateMoney(playerController.Money - itemToBuy.price);
    }

    private ShopRole GetCurrentShop()
    {
        GameObject currentSelectedObject = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < shopsRole.Count; i++)
        {
            if (currentSelectedObject.name.Contains(shopsRole[i].shopRole.ToString()))
            {
                return shopsRole[i];
            }
        }

        return null;
    }

    #endregion
}