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

        interactionUI.SetActive(false);
        CloseShopUI();

        interactionUI.SetActive(false);
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
        int amount = int.Parse(currentItemUI.AmountUI.text);

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

    private void SellItem(ItemToHandle itemToSell, InfosUIRefs currentItemUI, ShopConfiguration shopConfiguration)
    {
        int amount = int.Parse(currentItemUI.AmountUI.text);

        if (amount == 0) return;

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

    private void UpgradeAnimalPen(ShopConfiguration shopConfiguration, InfosUIRefs infosUIRefs, int price, int level, string type, int index)
    {
        if (shopConfiguration.items.Count > level)
        {
            if (playerController.Money >= price)
            {
                listSlots.UpdateMoney(playerController.Money - price);

                MinigameManager.AnimalPenIndexToUpgrade.Add(index);

                level++;

                UpdateAnimalPenUI(shopConfiguration, infosUIRefs, level, type, index);

                ShowNotification($"Vous avez acheté l'amélioration Lv{level} pour l'enclos n°{index} pour {price} P");
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

    private void HandleSeedSeller(ShopConfiguration currentShop)
    {
        if (currentShop.items == null) return;

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

        if (currentShop.items.Count > 0) currentShop.items.Clear();

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

        for (int i = 0; i < data.nbAnimalPen; i++)
        {
            ShowAnimalsPenUI(currentShop, data.animalPenLevels[i], data.animalPenTypes[i], i);
        }
    }

    private void ShowObjectUI(ShopConfiguration shopConfiguration, int index)
    {
        InfosUIRefs infosUIRefs = Instantiate(shopConfiguration.objectsInfosUI, shopConfiguration.objectsParent).GetComponent<InfosUIRefs>();

        Sprite objectSprite;
        string objectName;
        float objectPrice;

        if (!shopConfiguration.isRecipe)
        {
            ItemToHandle itemToHandle = shopConfiguration.items[index];

            objectSprite = itemToHandle.item.itemSprite;

            if (!shopConfiguration.isForSelling) objectName = itemToHandle.item.itemName;
            else objectName = $"{itemToHandle.item.itemName} x{playerController.PlayerInventory.GetItemQuantity(itemToHandle.item)}";

            objectPrice = itemToHandle.price;        
        }
        else
        {
            RecipeToHandle recipeToHandle = shopConfiguration.recipes[index]; ;

            objectSprite = recipeToHandle.item.recipeSprite;
            objectName = recipeToHandle.item.recipeName;
            objectPrice = recipeToHandle.price;
        }

        infosUIRefs.ImageUI.sprite = objectSprite;
        infosUIRefs.NameUI.text = objectName;
        infosUIRefs.PriceUI.text = $"{objectPrice} P";

        if (!shopConfiguration.isRecipe)
        {
            infosUIRefs.AmountUI.text = $"0";

            if (!shopConfiguration.isForSelling)
                infosUIRefs.OperationUI.onClick.AddListener(delegate { BuyItem(shopConfiguration.items[index], infosUIRefs); });
            else
                infosUIRefs.OperationUI.onClick.AddListener(delegate { SellItem(shopConfiguration.items[index], infosUIRefs, shopConfiguration); });

            infosUIRefs.AmountButtonUp.onClick.AddListener(delegate { AddValue(infosUIRefs); });
            infosUIRefs.AmountButtonDown.onClick.AddListener(delegate { RemoveValue(infosUIRefs); });
        }
        else
        {
            infosUIRefs.OperationUI.onClick.AddListener(delegate { BuyRecipe(shopConfiguration.recipes[index]); });
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
        int price = shopConfiguration.items[level - 1].price;

        infosUIRefs.NameUI.text = $"Enclos Lv.{level} pour {type}";
        infosUIRefs.PriceUI.text = $"{price} P";
        infosUIRefs.OperationUI.onClick.AddListener(delegate { UpgradeAnimalPen(shopConfiguration, infosUIRefs, price, level, type, index); });
    }

    public void AddValue(InfosUIRefs itemRefs)
    {
        int currentValue = int.Parse(itemRefs.AmountUI.text);

        currentValue++;

        itemRefs.AmountUI.text = $"{currentValue}";
    }

    public void RemoveValue(InfosUIRefs itemRefs)
    {
        int currentValue = int.Parse(itemRefs.AmountUI.text);

        currentValue--;

        if (currentValue < 0) currentValue = 0;

        itemRefs.AmountUI.text = $"{currentValue}";
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