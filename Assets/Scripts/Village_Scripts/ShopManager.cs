using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    #region Parameters

    [Header("Notification")]
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private GameObject notification;
    [SerializeField] private int timeNotification = 5;

    [Header("References")]
    [SerializeField] private string shopName;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject shopsUI;

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

    private int[] animalPensLevel;

    #endregion

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerInput = FindObjectOfType<PlayerInput>();
        listSlots = FindObjectOfType<List_Slots>();

        interaction = "Utiliser " + playerInput.InteractionAction.GetBindingDisplayString();

        interactionUI.SetActive(false);
        CloseShopUI();

        interactionUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            listSlots.UpdateMoney(1000);
        }

        HandleShopUI();
    }

    #region Handle Shop

    #region Handle Main Shop UI

    private void HandleShopUI()
    {
        if (shopUI != null)
            shopUI.SetActive(shopInUse);

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

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} pour fermer {shopName}";

        MinigameManager.AddOpenInventory(shopUI);
    }

    private void CloseShopUI()
    {
        shopInUse = false;

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} pour ouvrir {shopName}";

        MinigameManager.RemoveOpenInventory(shopUI);

        Clear();
    }

    public void UpdateInteraction(bool state)
    {
        interactionUI.SetActive(state);

        if (state)
            interactionUI.GetComponentInChildren<Text>().text = $"{interaction} pour ouvrir {shopName}";
    }

    #endregion

    public void ShowHome()
    {
        for (int i = 0; i < shopsConfiguration.Count; i++)
        {
            shopsConfiguration[i].objectsPanel.SetActive(false);
        }
    }

    public void ShowShopContent()
    {
        Show(GetCurrentShop());
    }

    #region Buy / Sell / Upgrade

    private void BuyItem(ItemToHandle itemToBuy, InfosUIRefs currentItemUI)
    {
        if (currentItemUI.AmountUI.text == String.Empty) return;

        int amount = int.Parse(currentItemUI.AmountUI.text);

        if (amount <= 0) return;

        if (playerController.PlayerInventory.InventoryIsFull())
        {
            ShowNotification($"L'inventaire est plein");

            return;
        }

        int totalPrice = amount * itemToBuy.price;

        if (playerController.Money >= totalPrice)
        {
            listSlots.UpdateMoney(playerController.Money - totalPrice);

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

    private void SellItem(ItemToHandle itemToSell, InfosUIRefs currentItemUI, ShopConfiguration shopConfiguration)
    {
        if (currentItemUI.AmountUI.text == String.Empty) return;

        int amount = int.Parse(currentItemUI.AmountUI.text);

        if (amount <= 0) return;

        int quantityInInventory = playerController.PlayerInventory.GetItemQuantity(itemToSell.item);

        if (amount <= quantityInInventory)
        {
            int totalPrice = amount * itemToSell.price;
            int quantityLeft = quantityInInventory - amount;

            listSlots.UpdateMoney(playerController.Money + totalPrice);

            playerController.PlayerInventory.RemoveItemQuantity(itemToSell.item, amount);

            currentItemUI.NameUI.text = $"{itemToSell.item.itemName} x{quantityLeft}";

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

    private void UpgradeAnimalPen(ShopConfiguration shopConfiguration, InfosUIRefs infosUIRefs, int price, string type, int index)
    {
        int currentLevel = animalPensLevel[index];

        int maxLevel = shopConfiguration.items.Count;

        if (currentLevel < maxLevel)
        {
            if (playerController.Money >= price)
            {
                listSlots.UpdateMoney(playerController.Money - price);

                currentLevel++;

                animalPensLevel[index] = currentLevel;

                UpdateAnimalPenUI(shopConfiguration, infosUIRefs, currentLevel, type, index);

                SaveAnimalPenLevels();

                ShowNotification($"Vous avez acheté l'amélioration Lv{currentLevel} pour l'enclos n°{index} pour {price} P");
            }
            else
            {
                ShowNotification($"Vous n'avez pas assez de pièces pour améliorer cet enclos");
            }
        }
        else
        {
            ShowNotification($"Vous avez achetez toutes les améliorations pour cet enclos");
        }
    }

    private void SaveAnimalPenLevels()
    {
        AnimalPen_Data data = (AnimalPen_Data)SaveSystem.Load(SaveSystem.SaveType.Save_AnimalPen);

        if (data == null) return;

        data.animalPenLevels = animalPensLevel;

        SaveSystem.Save(SaveSystem.SaveType.Save_AnimalPen, data);
    }

    #endregion

    #region Show Current Shop

    private void Show(ShopConfiguration currentShop)
    {
        switch (currentShop.shopConfiguration)
        {
            case ShopType.Seeds_Seller:
                HandleSeedSeller(currentShop);
                break;
            case ShopType.Recipes_Dealer:
                HandleRecipesDealer(currentShop);
                break;
            case ShopType.Items_Dealer:
                HandleItemsDealer(currentShop);
                break;
            case ShopType.Animal_Pen_Manager:
                HandleAnimalPenManager(currentShop);
                break;
            default:
                Debug.Log($"Error Show Shop Content");
                break;
        }
    }

    private void SetupListAccordingToRestrictions(ShopConfiguration currentShop, bool isRecipe)
    {
        if (listSlots == null || listSlots.Stuffs == null) return;

        for (int i = 0; i < listSlots.Stuffs.Length; i++)
        {
            if (!isRecipe)
            {
                if (listSlots.Stuffs[i].GetType() == typeof(Item))
                {
                    Item item = (Item)listSlots.Stuffs[i];

                    if (currentShop.itemsRestriction.Contains(item.itemType)) 
                        CreateItemToHandle(item, (int)item.itemValue, currentShop);
                }
            }
            else
            {
                if (listSlots.Stuffs[i].GetType() == typeof(Recipe))
                {
                    Recipe recipe = (Recipe)listSlots.Stuffs[i];

                    if (currentShop.itemsRestriction.Contains(ItemType.Recipe)) 
                        CreateRecipeToHandle(recipe, (int)recipe.basePrice, currentShop);
                }
            }
        }
    }

    private void HandleSeedSeller(ShopConfiguration currentShop)
    {
        if (currentShop.items == null) return;

        SetupListAccordingToRestrictions(currentShop, false);

        Clear();

        currentShop.objectsPanel.SetActive(true);

        currentShop.objectsParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < currentShop.items.Count; i++)
        {
            ShowObjectUI(currentShop, i);
        }
    }

    private void HandleRecipesDealer(ShopConfiguration currentShop)
    {
        if (currentShop.recipes == null) return;

        SetupListAccordingToRestrictions(currentShop, true);

        Clear();

        currentShop.objectsPanel.SetActive(true);

        currentShop.objectsParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < currentShop.recipes.Count; i++)
        {
            ShowObjectUI(currentShop, i);
        }
    }

    private void HandleItemsDealer(ShopConfiguration currentShop)
    {
        if (currentShop.items == null) return;

        currentShop.items.Clear();

        for (int i = 0; i < playerController.PlayerInventory.SearchItemsPossessed().Count; i++)
        {
            DraggableItem itemPossessed = playerController.PlayerInventory.SearchItemsPossessed()[i];

            if (currentShop.itemsRestriction.Contains(itemPossessed.Item.itemType))
            {
                CreateItemToHandle(playerController.PlayerInventory.SearchItemsPossessed()[i].Item, (int)playerController.PlayerInventory.SearchItemsPossessed()[i].Item.itemValue, currentShop);
            }
        }

        Clear();

        currentShop.objectsPanel.SetActive(true);

        currentShop.objectsParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < currentShop.items.Count; i++)
        {
            ShowObjectUI(currentShop, i);
        }
    }

    private void HandleAnimalPenManager(ShopConfiguration currentShop)
    {
        AnimalPen_Data data = (AnimalPen_Data)SaveSystem.Load(SaveSystem.SaveType.Save_AnimalPen);

        if (data == null) return;

        Clear();

        currentShop.objectsPanel.SetActive(true);

        currentShop.objectsParent.GetComponent<Swipe>().ScrollPos = 1;

        animalPensLevel = data.animalPenLevels;

        for (int i = 0; i < data.nbAnimalPen; i++)
        {
            ShowAnimalsPenUI(currentShop, data.animalPenLevels[i], data.animalPenTypes[i], i);
        }
    }

    private void ShowObjectUI(ShopConfiguration shopConfiguration, int index)
    {
        InfosUIRefs infosUIRefs = Instantiate(shopConfiguration.objectsInfosUI, shopConfiguration.objectsParent).GetComponent<InfosUIRefs>();

        if (infosUIRefs == null || shopConfiguration == null) return;

        object objectToHandle = null;
        Sprite objectSprite = null;
        string objectName = null;
        float objectPrice = -1;

        if (!shopConfiguration.isRecipe && shopConfiguration.items.Count == 0) return;
        if (shopConfiguration.isRecipe && shopConfiguration.recipes.Count == 0) return;

        if (!shopConfiguration.isRecipe)
        {
            ItemToHandle itemToHandle = shopConfiguration.items[index];
            objectToHandle = itemToHandle;

            objectSprite = itemToHandle.item.itemSprite;

            if (!shopConfiguration.isForSelling) objectName = itemToHandle.item.itemName;
            else objectName = $"{itemToHandle.item.itemName} x{playerController.PlayerInventory.GetItemQuantity(itemToHandle.item)}";

            objectPrice = itemToHandle.price;
        }
        else
        {
            RecipeToHandle recipeToHandle = shopConfiguration.recipes[index];
            objectToHandle = recipeToHandle;

            objectSprite = recipeToHandle.item.recipeSprite;
            objectName = recipeToHandle.item.recipeName;
            objectPrice = recipeToHandle.price;
        }

        infosUIRefs.ImageUI.sprite = objectSprite;
        infosUIRefs.NameUI.text = objectName;
        infosUIRefs.PriceUI.text = $"{objectPrice} P";

        if (!shopConfiguration.isRecipe)
        {
            infosUIRefs.AmountUI.text = $"1";

            infosUIRefs.OperationUI.onClick.RemoveAllListeners();

            if (!shopConfiguration.isForSelling)
            {
                infosUIRefs.OperationUI.onClick.AddListener(delegate { BuyItem((ItemToHandle)objectToHandle, infosUIRefs); });
            }
            else
            {
                infosUIRefs.OperationUI.onClick.AddListener(delegate { SellItem((ItemToHandle)objectToHandle, infosUIRefs, shopConfiguration); });
            }

            infosUIRefs.AmountButtonUp.onClick.RemoveAllListeners();
            infosUIRefs.AmountButtonDown.onClick.RemoveAllListeners();

            infosUIRefs.AmountButtonUp.onClick.AddListener(delegate { AddValue(infosUIRefs); });
            infosUIRefs.AmountButtonDown.onClick.AddListener(delegate { RemoveValue(infosUIRefs); });
        }
        else
        {
            infosUIRefs.OperationUI.onClick.RemoveAllListeners();

            infosUIRefs.OperationUI.onClick.AddListener(delegate { BuyRecipe((RecipeToHandle)objectToHandle); });
        }
    }

    private void ShowAnimalsPenUI(ShopConfiguration shopConfiguration, int animalPenLevel, string animalPenType, int index)
    {
        InfosUIRefs infosUIRefs = Instantiate(shopConfiguration.objectsInfosUI, shopConfiguration.objectsParent).GetComponent<InfosUIRefs>();

        if (MinigameManager.AnimalPenIndexToUpgrade.Contains(index))
        {
            for (int i = 0; i < MinigameManager.AnimalPenIndexToUpgrade.Count; i++)
            {
                if (MinigameManager.AnimalPenIndexToUpgrade[i] == index)
                {
                    animalPenLevel++;
                    UpdateAnimalPenUI(shopConfiguration, infosUIRefs, animalPenLevel++, animalPenType, index);
                }
            }
        }
        else
        {
            UpdateAnimalPenUI(shopConfiguration, infosUIRefs, animalPenLevel, animalPenType, index);
        }
    }

    private void UpdateAnimalPenUI(ShopConfiguration shopConfiguration, InfosUIRefs infosUIRefs, int level, string type, int index)
    {
        if (level == 3)
        {
            infosUIRefs.NameUI.text = $"Enclos Lv.{level} (max) pour {type}";
            infosUIRefs.PriceUI.text = $"0 P";
            infosUIRefs.OperationUI.onClick.RemoveAllListeners();
            infosUIRefs.OperationUI.interactable = false;
        }
        else
        {
            int price = shopConfiguration.items[level - 1].price;

            infosUIRefs.NameUI.text = $"Enclos Lv.{level} -> Lv.{level + 1} pour {type}";
            infosUIRefs.PriceUI.text = $"{price} P";
            infosUIRefs.OperationUI.onClick.RemoveAllListeners();
            infosUIRefs.OperationUI.onClick.AddListener(delegate { UpgradeAnimalPen(shopConfiguration, infosUIRefs, price, type, index); });
        }
    }

    public void AddValue(InfosUIRefs itemRefs)
    {
        if (itemRefs.AmountUI.text == String.Empty) itemRefs.AmountUI.text = $"0";

        int currentValue = int.Parse(itemRefs.AmountUI.text);

        currentValue++;

        itemRefs.AmountUI.text = $"{currentValue}";
    }

    public void RemoveValue(InfosUIRefs itemRefs)
    {
        if (itemRefs.AmountUI.text == String.Empty) itemRefs.AmountUI.text = $"0";

        int currentValue = int.Parse(itemRefs.AmountUI.text);

        currentValue--;

        if (currentValue < 0) currentValue = 0;

        itemRefs.AmountUI.text = $"{currentValue}";
    }

    #endregion

    #region Item Creation

    private void CreateItemUI(ItemToHandle itemToBuy, int quantity)
    {
        playerController.PlayerInventory.AddItemToInventory(itemToBuy.item, quantity);
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

    private RecipeToHandle CreateRecipeToHandle(Recipe recipe, int price, ShopConfiguration currentShop)
    {
        RecipeToHandle recipeToHandle = null;

        bool alreadyAdded = false;

        for (int i = 0; i < currentShop.recipes.Count; i++)
        {
            if (currentShop.recipes[i].item == recipe)
            {
                alreadyAdded = true;
            }
        }

        if (!alreadyAdded)
        {
            recipeToHandle = new RecipeToHandle();

            recipeToHandle.item = recipe;
            recipeToHandle.price = price;

            currentShop.recipes.Add(recipeToHandle);
        }

        return recipeToHandle;
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
        if (notification != null) Destroy(notification);

        notification = Instantiate(notificationPrefab, shopsUI.transform);

        notification.transform.SetAsFirstSibling();

        notification.GetComponentInChildren<Text>().text = textToShow;

        Destroy(notification, timeNotification);
    }

    #endregion
}