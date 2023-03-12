using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private List<GameObject> shopsUI;

    private playerController playerController;
    private PlayerInput playerInput;
    private List_Slots listSlots;

    private GameObject currentShopUI = null;
    private string interaction;

    [Header("Shop states")]
    [SerializeField] private bool shopSetup = false;
    [SerializeField] private bool canUseShop = false;
    [SerializeField] private bool shopInUse = false;
    [SerializeField] private ShopRole shopRole;

    public bool CanUseShop
    {
        get { return canUseShop; }
        set { canUseShop = value; }
    }

    [Header("Shop Items Dealer")]
    [SerializeField] private List<ItemToBuy> itemsToBuy;
    [SerializeField] private List<RecipeToBuy> recipesToBuy;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private GameObject recipesPanel;
    [SerializeField] private Transform recipesParent;
    [SerializeField] private GameObject itemToBuyUI;

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

    public enum ShopRole
    { 
        FruitSeller,
        ItemDealer,
        AnimalPenManager
    }

    void Start()
    {
        playerController = FindObjectOfType<playerController>();
        listSlots = FindObjectOfType<List_Slots>();
        playerInput = GetComponent<PlayerInput>();

        interaction = "Use " + playerInput.actions["Intercation_Environnements"].GetBindingDisplayString();
    }

    void Update()
    {
        HandleShopUse();

        HandleShopUI();
    }

    #region Shop Use

    private void HandleShopUse()
    {
        if (canUseShop)
        {
            interactionUI.SetActive(true);
        }
        else
        {
            interactionUI.SetActive(false);
        }
    }

    #endregion

    #region Shop UI

    private void HandleShopUI()
    {
        if (currentShopUI != null)
            currentShopUI.SetActive(shopInUse);

        if (!canUseShop)
        {
            CloseShopUI();
            return;
        }

        if (playerInput.actions["Intercation_Environnements"].triggered && canUseShop)
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

    private GameObject GetCurrentShopUI()
    {
        GameObject shopUI = null;

        for (int i = 0; i < shopsUI.Count; i++)
        {
            if (shopsUI[i].name.Contains(shopRole.ToString()))
            {
                shopUI = shopsUI[i];
            }
        }

        return shopUI;
    }

    private void OpenShopUI()
    {
        currentShopUI = GetCurrentShopUI();

        shopInUse = true;

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} to close Shop";

        MinigameManager.AddOpenInventory(currentShopUI);
    }

    private void CloseShopUI()
    {
        shopInUse = false;

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} to open Shop";

        MinigameManager.RemoveOpenInventory(currentShopUI);

        Clear();
    }

    #endregion

    #region Shop Content

    #region Shop "Fruit Seller"

    private void HandleFruitSeller()
    {
        Debug.Log($"Fruit Seller");
    }

    #endregion

    #region Shop "Item Dealer"

    public void ShowItems()
    {
        if (itemsToBuy == null) return;

        Clear();

        itemsPanel.SetActive(true);
        recipesPanel.SetActive(false);

        Debug.Log($"Show Items");

        itemsParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < itemsToBuy.Count; i++)
        {
            Transform item = Instantiate(itemToBuyUI, itemsParent).transform;

            ItemToBuy itemToBuy = itemsToBuy[i];

            item.GetChild(0).GetComponent<Image>().sprite = itemToBuy.item.itemSprite;
            item.GetChild(1).GetComponent<Text>().text = itemToBuy.item.itemName;
            item.GetChild(2).GetComponent<Text>().text = $"{itemToBuy.price} P";

            InputField inputField = item.GetChild(3).GetComponent<InputField>();
            inputField.text = $"{0}";
            item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { BuyItem(itemToBuy, inputField); });
        }
    }

    public void ShowRecipes()
    {
        if (recipesToBuy == null) return;

        Clear();

        itemsPanel.SetActive(false);
        recipesPanel.SetActive(true);

        Debug.Log($"Show Recipes");

        recipesParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < recipesToBuy.Count; i++)
        {
            Transform item = Instantiate(itemToBuyUI, recipesParent).transform;

            RecipeToBuy recipeToBuy = recipesToBuy[i];

            item.GetChild(0).GetComponent<Image>().sprite = recipeToBuy.item.recipeSprite;
            item.GetChild(1).GetComponent<Text>().text = recipeToBuy.item.recipeName;
            item.GetChild(2).GetComponent<Text>().text = $"{recipeToBuy.price} P";

            InputField inputField = item.GetChild(3).GetComponent<InputField>();
            inputField.text = $"{0}";
            item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { BuyRecipe(recipeToBuy, inputField); });
        }
    }

    private void BuyItem(ItemToBuy itemToBuy, InputField inputField)
    {
        int amount = int.Parse(inputField.text);

        if (amount == 0) return;

        Debug.Log($"Player want to buy {itemToBuy.item.itemName} x{amount} for {amount} * {itemToBuy.price}");

        if (playerController.money >= (amount * itemToBuy.price))
        {
            Debug.Log($"Player can buy item(s) : {playerController.money} >= {amount * itemToBuy.price}");
            GameObject itemUI = listSlots.PlayerSlotsParent.GetComponent<PlayerInventoryUI>().CreateItemUI();

            itemUI.GetComponent<DraggableItem>().quantityStacked = amount;

            itemUI.GetComponent<DraggableItem>().Item = itemToBuy.item;

            listSlots.UpdateMoney(playerController.money - (amount * itemToBuy.price));
        }
        else
        {
            Debug.Log($"Player can NOT buy item(s)");
        }
    }

    private void BuyRecipe(RecipeToBuy recipeToBuy, InputField inputField)
    {
        int amount = int.Parse(inputField.text);

        if (amount == 0) return;

        Debug.Log($"Player want to buy {recipeToBuy.item.recipeName} x{inputField.text} for {inputField.text} * {recipeToBuy.price}");

        if (playerController.money >= (amount * recipeToBuy.price))
        {
            Debug.Log($"Player can buy recipe(s) : {playerController.money} >= {amount * recipeToBuy.price}");

            listSlots.UpdateMoney(playerController.money - (amount * recipeToBuy.price));
        }
        else
        {
            Debug.Log($"Player can NOT buy recipe(s)");
        }
    }

    private void Clear()
    {
        itemsPanel.SetActive(false);
        recipesPanel.SetActive(false);

        for (int i = 0; i < itemsParent.childCount; i++)
        {
            Destroy(itemsParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < recipesParent.childCount; i++)
        {
            Destroy(recipesParent.GetChild(i).gameObject);
        }
    }

    #endregion

    #region Shop "Animal Pen Manager"

    private void HandleAnimalPenManager()
    {
        Debug.Log($"Animal Pen Manager");
    }

    #endregion

    #endregion
}