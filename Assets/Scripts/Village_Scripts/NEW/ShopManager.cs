using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    #region Parameters

    [Header("References")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject notification;
    [SerializeField] private int timeNotification = 5;

    private string interaction;

    public GameObject InteractionUI
    {
        get { return interactionUI; }
        set { interactionUI = value; }
    }

    [Header("Shop States")]
    [SerializeField] private bool canUseShop = false;
    [SerializeField] private bool shopInUse = false;

    public bool CanUseShop
    {
        get { return canUseShop; }
        set { canUseShop = value; }
    }

    [Serializable]
    public class ShopConfiguration
    {
        [Header("References")]
        public GameObject objectsPanel;
        public GameObject objectsInfosUI;
        public Transform objectsParent;

        [Header("Shop Properties")]
        public ShopType shopConfiguration;
        public bool isRecipe;
        public bool isForSelling;
        public List<ItemType> itemsRestriction;

        [Header("Shop Objects Lists")]
        public List<ItemToHandle> items;
        public List<RecipeToHandle> recipes;
    }

    [Header("Shop Configuration")]
    [SerializeField] private GameObject shopUI;
    [SerializeField] private List<ShopConfiguration> shopsConfiguration;

    private PlayerController playerController;
    private PlayerInput playerInput;
    private List_Slots listSlots;

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

    #endregion

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerInput = FindObjectOfType<PlayerInput>();
        listSlots = FindObjectOfType<List_Slots>();

        interaction = "Utiliser " + playerInput.InteractionAction.GetBindingDisplayString();

        notification.SetActive(false);
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

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} pour fermer l'interface";

        MinigameManager.AddOpenInventory(shopUI);
    }

    private void CloseShopUI()
    {
        shopInUse = false;

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} pour ouvrir l'interface";

        MinigameManager.RemoveOpenInventory(shopUI);

        Clear();
    }

    #endregion

    public void ShowShopContent()
    {
        Show(GetCurrentShop());
    }

    #region Buy / Sell

    private void BuyItem(ItemToHandle itemToBuy, InputField inputField)
    {
        int amount = int.Parse(inputField.text);

        if (amount == 0) return;

        int totalPrice = amount * itemToBuy.price;

        if (playerController.Money >= totalPrice)
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

            ShowNotification($"Vous avez acheté x{amount} '{itemToBuy.item.itemName}' pour {totalPrice} pièces");
        }
        else
        {
            ShowNotification($"Vous n'avez pas assez de pièces pour acheter cet item");
        }
    }

    private void SellItem(ItemToHandle itemToSell, InputField inputField, Transform currentItemUI, ShopConfiguration shopConfiguration)
    {
        int amount = int.Parse(inputField.text);

        if (amount == 0) return;

        int quantityInInventory = playerController.PlayerInventory.GetItemQuantity(itemToSell.item);

        if (amount <= quantityInInventory)
        {
            int totalPrice = amount * itemToSell.price;
            int quantityLeft = quantityInInventory - amount;

            listSlots.UpdateMoney(playerController.Money + totalPrice);

            playerController.PlayerInventory.RemoveItemQuantity(itemToSell.item, amount);

            currentItemUI.GetChild(1).GetComponent<Text>().text = $"{itemToSell.item.itemName} x{quantityLeft}";

            ShowNotification($"Vous avez vendu x{amount} '{itemToSell.item.itemName}' pour {totalPrice} pièces");

            if (quantityLeft <= 0)
            {
                shopConfiguration.items.Remove(itemToSell);

                Destroy(currentItemUI.gameObject);
            }
        }
        else
        {
            ShowNotification($"La quantité saisie est trop élevée");
        }
    }

    private void BuyRecipe(RecipeToHandle recipeToBuy)
    {
        if (playerController.Money >= recipeToBuy.price)
        {
            if (!playerController.PlayerRecipesInventory.CheckIfHasRecipe(recipeToBuy.item))
            {
                playerController.PlayerRecipesInventory.AddRecipeToInventory(recipeToBuy.item);

                listSlots.UpdateMoney(playerController.Money - recipeToBuy.price);

                ShowNotification($"Vous avez acheté la recette '{recipeToBuy.item.recipeName}' pour {recipeToBuy.price} pièces");
            }
            else
            {
                ShowNotification($"Vous possedez déjà cette recette");
            }
        }
        else
        {
            ShowNotification($"Vous n'avez pas assez de pièces pour acheter cette recette");
        }
    }

    #endregion

    #region Show Current Shop

    private void Show(ShopConfiguration currentShop)
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
                DraggableItem itemPossessed = playerController.PlayerInventory.SearchItemsPossessed()[i];

                if (currentShop.itemsRestriction.Contains(itemPossessed.Item.itemType))
                {
                    CreateItemToHandle(playerController.PlayerInventory.SearchItemsPossessed()[i].Item, (int)playerController.PlayerInventory.SearchItemsPossessed()[i].Item.itemValue, currentShop);
                }
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

    private void ShowObjectUI(Transform parent, GameObject objectInfoUI, object objectToHandle, bool isRecipe, bool isForSelling, ShopConfiguration shopConfiguration)
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
                item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { SellItem((ItemToHandle)objectToHandle, inputField, item, shopConfiguration); });
            }
        }
        else
        {
            item.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { BuyRecipe((RecipeToHandle)objectToHandle); });
        }
    }

    #endregion

    #region Item Creation

    private void CreateItemUI(ItemToHandle itemToBuy, int quantity)
    {
        GameObject itemUI = listSlots.PlayerSlotsParent.GetComponent<PlayerInventory>().CreateItemUI();

        itemUI.GetComponent<DraggableItem>().QuantityStacked = quantity;

        itemUI.GetComponent<DraggableItem>().Item = itemToBuy.item;

        listSlots.UpdateMoney(playerController.Money - itemToBuy.price);
    }

    private ItemToHandle CreateItemToHandle(Item item, int price, ShopConfiguration currentShop)
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

    #endregion

    private ShopConfiguration GetCurrentShop()
    {
        GameObject currentSelectedObject = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < shopsConfiguration.Count; i++)
        {
            if (currentSelectedObject.name.Contains(shopsConfiguration[i].shopConfiguration.ToString()))
            {
                return shopsConfiguration[i];
            }
        }

        return null;
    }

    private void Clear()
    {
        for (int i = 0; i < shopsConfiguration.Count; i++)
        {
            if (shopsConfiguration[i].objectsPanel != null)
            {
                shopsConfiguration[i].objectsPanel.SetActive(false);

                for (int j = 0; j < shopsConfiguration[i].objectsParent.childCount; j++)
                {
                    Destroy(shopsConfiguration[i].objectsParent.GetChild(j).gameObject);
                }
            }
        }
    }

    private void ShowNotification(string textToShow)
    {
        StopCoroutine(ShowingNotification(textToShow));
        StartCoroutine(ShowingNotification(textToShow));
    }

    private IEnumerator ShowingNotification(string textToShow)
    {
        notification.GetComponentInChildren<Text>().text = textToShow;

        notification.SetActive(true);

        yield return new WaitForSeconds(timeNotification);

        notification.SetActive(false);
    }

    #endregion
}